using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    
    [Header("Time Settings")]
    [SerializeField] private float gameTimeScale = 60f; // 1 real second = 60 game seconds
    [SerializeField] private bool isPaused = false;
    
    [Header("Current Time")]
    [SerializeField] private int currentDay = 1;
    [SerializeField] private int currentHour = 8; // Start at 8 AM
    [SerializeField] private int currentMinute = 0;
    [SerializeField] private Season currentSeason = Season.Spring;
    [SerializeField] private int year = 1;
    
    [Header("Day/Night Cycle")]
    [SerializeField] private Transform sunLight; // A directional light or visual element
    [SerializeField] private Color dayAmbientColor = new Color(0.9f, 0.9f, 1f);
    [SerializeField] private Color nightAmbientColor = new Color(0.2f, 0.2f, 0.4f);
    [SerializeField] private int sunriseHour = 6;
    [SerializeField] private int sunsetHour = 18;
    
    // Events
    public event Action<int, int> OnTimeChanged; // hour, minute
    public event Action<int> OnNewDay;
    public event Action<Season> OnNewSeason;
    public event Action<TimeSpan> OnTimeScaleChanged;
    
    // Time-based triggers
    private Dictionary<TimeSpan, List<Action>> scheduledEvents = new Dictionary<TimeSpan, List<Action>>();
    
    private float accumulatedTime = 0f;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void Start()
    {
        // Initialize lighting
        UpdateLighting();
    }
    
    private void Update()
    {
        if (!isPaused)
        {
            UpdateGameTime(Time.deltaTime);
        }
    }
    
    public void UpdateGameTime(float deltaTime)
    {
        // Accumulate real time
        accumulatedTime += deltaTime * gameTimeScale;
        
        // Convert to in-game minutes
        int minutesToAdd = Mathf.FloorToInt(accumulatedTime / 60f);
        accumulatedTime %= 60f;
        
        if (minutesToAdd > 0)
        {
            AdvanceTimeByMinutes(minutesToAdd);
        }
    }
    
    private void AdvanceTimeByMinutes(int minutes)
    {
        int previousHour = currentHour;
        
        currentMinute += minutes;
        
        // Handle hour rollover
        if (currentMinute >= 60)
        {
            currentHour += currentMinute / 60;
            currentMinute %= 60;
            
            // Update lighting if hour changed
            if (previousHour != currentHour)
            {
                UpdateLighting();
            }
            
            // Handle day rollover
            if (currentHour >= 24)
            {
                currentDay += currentHour / 24;
                currentHour %= 24;
                
                OnNewDay?.Invoke(currentDay);
                
                // Handle season change (example: 28 days per season)
                if (currentDay > 28)
                {
                    AdvanceSeason();
                    currentDay = 1;
                }
            }
        }
        
        // Check for scheduled events
        CheckScheduledEvents();
        
        // Notify any listeners about time change
        OnTimeChanged?.Invoke(currentHour, currentMinute);
    }
    
    private void AdvanceSeason()
    {
        // Change to next season
        currentSeason = (Season)(((int)currentSeason + 1) % 4);
        
        // If completed a year cycle
        if (currentSeason == Season.Spring)
        {
            year++;
        }
        
        // Notify season change
        OnNewSeason?.Invoke(currentSeason);
    }
    
    private void UpdateLighting()
    {
        if (sunLight != null)
        {
            // Calculate sun position based on time
            float timeProgress = (currentHour + currentMinute / 60f - sunriseHour) / 
                                (sunsetHour - sunriseHour);
            
            // Clamp for day/night
            timeProgress = Mathf.Clamp01(timeProgress);
            
            // During day (0 to 1)
            if (currentHour >= sunriseHour && currentHour < sunsetHour)
            {
                // Sun rotation (assuming Z is up in 2D)
                float sunAngle = Mathf.Lerp(0, 180, timeProgress);
                sunLight.rotation = Quaternion.Euler(0, 0, sunAngle);
                
                // Adjust lighting intensity
                float intensity = Mathf.Sin(timeProgress * Mathf.PI);
                // Would adjust light intensity here if using actual lights
            }
            else
            {
                // Night time - sun is down
                sunLight.rotation = Quaternion.Euler(0, 0, 180);
                // Set light intensity low
            }
        }
        
        // Blend ambient colors
        float dayBlend = 0;
        
        // Morning transition
        if (currentHour >= sunriseHour - 1 && currentHour < sunriseHour + 1)
        {
            dayBlend = (currentHour + currentMinute / 60f - (sunriseHour - 1)) / 2f;
        }
        // Evening transition
        else if (currentHour >= sunsetHour - 1 && currentHour < sunsetHour + 1)
        {
            dayBlend = 1 - (currentHour + currentMinute / 60f - (sunsetHour - 1)) / 2f;
        }
        // Full day
        else if (currentHour >= sunriseHour + 1 && currentHour < sunsetHour - 1)
        {
            dayBlend = 1;
        }
        // Full night
        else
        {
            dayBlend = 0;
        }
        
        // Apply ambient light color (would use RenderSettings in a 3D project)
        Color ambientColor = Color.Lerp(nightAmbientColor, dayAmbientColor, dayBlend);
        // In 2D, we would apply this to global shader values or post-processing
    }
    
    // Schedule an event to occur at a specific time
    public void ScheduleEvent(int hour, int minute, Action eventAction)
    {
        var timeSpan = new TimeSpan(hour, minute);
        
        if (!scheduledEvents.ContainsKey(timeSpan))
        {
            scheduledEvents[timeSpan] = new List<Action>();
        }
        
        scheduledEvents[timeSpan].Add(eventAction);
    }
    
    // Cancel a scheduled event
    public void CancelScheduledEvent(int hour, int minute, Action eventAction)
    {
        var timeSpan = new TimeSpan(hour, minute);
        
        if (scheduledEvents.ContainsKey(timeSpan))
        {
            scheduledEvents[timeSpan].Remove(eventAction);
            
            if (scheduledEvents[timeSpan].Count == 0)
            {
                scheduledEvents.Remove(timeSpan);
            }
        }
    }
    
    private void CheckScheduledEvents()
    {
        var currentTimeSpan = new TimeSpan(currentHour, currentMinute);
        
        if (scheduledEvents.ContainsKey(currentTimeSpan))
        {
            foreach (var eventAction in scheduledEvents[currentTimeSpan])
            {
                eventAction?.Invoke();
            }
        }
    }
    
    // Pause/unpause time
    public void SetPaused(bool paused)
    {
        isPaused = paused;
    }
    
    // Set the time scale
    public void SetTimeScale(float scale)
    {
        gameTimeScale = Mathf.Max(0, scale);
        OnTimeScaleChanged?.Invoke(GetCurrentTimeSpan());
    }
    
    // Get current TimeSpan
    public TimeSpan GetCurrentTimeSpan()
    {
        return new TimeSpan(currentHour, currentMinute, 0);
    }
    
    // Public getters
    public float GetGameTimeScale() => gameTimeScale;
    public int GetCurrentDay() => currentDay;
    public int GetCurrentHour() => currentHour;
    public int GetCurrentMinute() => currentMinute;
    public Season GetCurrentSeason() => currentSeason;
    public int GetYear() => year;
    public bool IsDaytime() => currentHour >= sunriseHour && currentHour < sunsetHour;
    
    // Returns formatted time string (e.g., "08:30")
    public string GetTimeString()
    {
        return $"{currentHour:00}:{currentMinute:00}";
    }
    
    // Returns the day type (weekday/weekend)
    public DayType GetDayType()
    {
        // Example: day 6-7 are weekends
        int dayOfWeek = (currentDay - 1) % 7 + 1;
        return (dayOfWeek >= 6) ? DayType.Weekend : DayType.Weekday;
    }
    
    // Helper struct for scheduling
    // Update the TimeSpan struct to include a constructor with seconds
public struct TimeSpan
{
    public int hour;
    public int minute;
    public int second; // Add seconds field
    
    public TimeSpan(int hour, int minute)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = 0; // Default seconds to 0
    }
    
    public TimeSpan(int hour, int minute, int second)
    {
        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }
    
    public override bool Equals(object obj)
    {
        if (!(obj is TimeSpan)) return false;
        
        TimeSpan other = (TimeSpan)obj;
        return hour == other.hour && minute == other.minute && second == other.second;
    }
    
    public override int GetHashCode()
    {
        return hour * 3600 + minute * 60 + second;
    }
}
}

public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter,
    Any
}

public enum DayType
{
    Weekday,
    Weekend,
    Any
}

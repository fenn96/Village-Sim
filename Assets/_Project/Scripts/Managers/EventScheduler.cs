using System;
using System.Collections.Generic;
using UnityEngine;

public class EventScheduler : MonoBehaviour
{
    public static EventScheduler Instance { get; private set; }
    
    [SerializeField] private TimeManager timeManager;
    
    // Daily events - these repeat every day at the same time
    private Dictionary<string, List<ScheduledEvent>> dailyEvents = new Dictionary<string, List<ScheduledEvent>>();
    
    // One-time events
    private List<OneTimeEvent> oneTimeEvents = new List<OneTimeEvent>();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    private void Start()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance;
        }
        
        // Subscribe to time change events
        timeManager.OnTimeChanged += CheckEvents;
        timeManager.OnNewDay += OnNewDay;
    }
    
    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= CheckEvents;
            timeManager.OnNewDay -= OnNewDay;
        }
    }
    
    // Schedule a daily event at a specific time
    public void ScheduleDailyEvent(string eventId, int hour, int minute, Action action, DayType dayType = DayType.Any)
    {
        string timeKey = $"{hour:00}:{minute:00}";
        
        if (!dailyEvents.ContainsKey(timeKey))
        {
            dailyEvents[timeKey] = new List<ScheduledEvent>();
        }
        
        dailyEvents[timeKey].Add(new ScheduledEvent
        {
            id = eventId,
            hour = hour,
            minute = minute,
            dayType = dayType,
            action = action
        });
    }
    
    // Schedule a one-time event
    public void ScheduleOneTimeEvent(string eventId, int day, int hour, int minute, Action action, Season season = Season.Any)
    {
        oneTimeEvents.Add(new OneTimeEvent
        {
            id = eventId,
            day = day,
            hour = hour,
            minute = minute,
            season = season,
            action = action,
            year = timeManager.GetYear()
        });
    }
    
    // Schedule an event X minutes from now
    public void ScheduleEventFromNow(string eventId, int minutesFromNow, Action action)
    {
        int currentHour = timeManager.GetCurrentHour();
        int currentMinute = timeManager.GetCurrentMinute();
        
        // Calculate target time
        currentMinute += minutesFromNow;
        while (currentMinute >= 60)
        {
            currentHour++;
            currentMinute -= 60;
        }
        
        // Handle day rollover
        int targetDay = timeManager.GetCurrentDay();
        while (currentHour >= 24)
        {
            currentHour -= 24;
            targetDay++;
        }
        
        // Schedule one-time event
        ScheduleOneTimeEvent(eventId, targetDay, currentHour, currentMinute, action);
    }
    
    // Cancel a scheduled event by ID
    public void CancelScheduledEvent(string eventId)
    {
        // Check daily events
        foreach (var timeKey in dailyEvents.Keys)
        {
            dailyEvents[timeKey].RemoveAll(e => e.id == eventId);
        }
        
        // Check one-time events
        oneTimeEvents.RemoveAll(e => e.id == eventId);
    }
    
    // Check for scheduled events
    private void CheckEvents(int hour, int minute)
    {
        string timeKey = $"{hour:00}:{minute:00}";
        
        // Check daily events
        if (dailyEvents.ContainsKey(timeKey))
        {
            DayType currentDayType = timeManager.GetDayType();
            
            foreach (var scheduledEvent in dailyEvents[timeKey])
            {
                // Check if the event is for today (any day or matching day type)
                if (scheduledEvent.dayType == DayType.Any || scheduledEvent.dayType == currentDayType)
                {
                    scheduledEvent.action?.Invoke();
                }
            }
        }
        
        // Check one-time events
        int currentDay = timeManager.GetCurrentDay();
        Season currentSeason = timeManager.GetCurrentSeason();
        int currentYear = timeManager.GetYear();
        
        for (int i = oneTimeEvents.Count - 1; i >= 0; i--)
        {
            var oneTimeEvent = oneTimeEvents[i];
            
            if (oneTimeEvent.year == currentYear &&
                (oneTimeEvent.season == Season.Any || oneTimeEvent.season == currentSeason) &&
                oneTimeEvent.day == currentDay &&
                oneTimeEvent.hour == hour &&
                oneTimeEvent.minute == minute)
            {
                // Execute the event
                oneTimeEvent.action?.Invoke();
                
                // Remove the one-time event
                oneTimeEvents.RemoveAt(i);
            }
        }
    }
    
    // Handle new day
    private void OnNewDay(int newDay)
    {
        // Clean up expired one-time events
        int currentDay = timeManager.GetCurrentDay();
        Season currentSeason = timeManager.GetCurrentSeason();
        int currentYear = timeManager.GetYear();
        
        oneTimeEvents.RemoveAll(e => 
            e.year < currentYear || 
            (e.year == currentYear && e.season < currentSeason && e.season != Season.Any) ||
            (e.year == currentYear && (e.season == currentSeason || e.season == Season.Any) && e.day < currentDay)
        );
    }
    
    // Data structures for events
    [System.Serializable]
    public class ScheduledEvent
    {
        public string id;
        public int hour;
        public int minute;
        public DayType dayType;
        public Action action;
    }
    
    [System.Serializable]
    public class OneTimeEvent
    {
        public string id;
        public int day;
        public int hour;
        public int minute;
        public Season season;
        public int year;
        public Action action;
    }
}

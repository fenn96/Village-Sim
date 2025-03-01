using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI seasonText;
    [SerializeField] private Image dayNightCycleImage;
    [SerializeField] private Gradient dayNightCycleGradient;
    
    [Header("Time Control")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playNormalButton;
    [SerializeField] private Button playFastButton;
    
    private void Start()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance;
        }
        
        // Subscribe to time events
        timeManager.OnTimeChanged += UpdateTimeDisplay;
        timeManager.OnNewDay += UpdateDateDisplay;
        timeManager.OnNewSeason += UpdateSeasonDisplay;
        
        // Set up buttons
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseTime);
        
        if (playNormalButton != null)
            playNormalButton.onClick.AddListener(PlayTimeNormal);
        
        if (playFastButton != null)
            playFastButton.onClick.AddListener(PlayTimeFast);
        
        // Initialize displays
        UpdateTimeDisplay(timeManager.GetCurrentHour(), timeManager.GetCurrentMinute());
        UpdateDateDisplay(timeManager.GetCurrentDay());
        UpdateSeasonDisplay(timeManager.GetCurrentSeason());
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= UpdateTimeDisplay;
            timeManager.OnNewDay -= UpdateDateDisplay;
            timeManager.OnNewSeason -= UpdateSeasonDisplay;
        }
    }
    
    private void UpdateTimeDisplay(int hour, int minute)
    {
        if (timeText != null)
        {
            timeText.text = $"{hour:00}:{minute:00}";
        }
        
        UpdateDayNightCycle();
    }
    
    private void UpdateDateDisplay(int day)
    {
        if (dateText != null)
        {
            string dayType = timeManager.GetDayType() == DayType.Weekend ? "(Weekend)" : "(Weekday)";
            dateText.text = $"Day {day} {dayType}";
        }
    }
    
    private void UpdateSeasonDisplay(Season season)
    {
        if (seasonText != null)
        {
            seasonText.text = $"{season} - Year {timeManager.GetYear()}";
        }
    }
    
    private void UpdateDayNightCycle()
    {
        if (dayNightCycleImage == null || dayNightCycleGradient == null)
            return;
            
        // Calculate day progress (0-1)
        float dayProgress = (timeManager.GetCurrentHour() + timeManager.GetCurrentMinute() / 60f) / 24f;
        
        // Set the color based on time of day
        dayNightCycleImage.color = dayNightCycleGradient.Evaluate(dayProgress);
    }
    
    // Time control methods
    private void PauseTime()
    {
        timeManager.SetPaused(true);
    }
    
    private void PlayTimeNormal()
    {
        timeManager.SetPaused(false);
        timeManager.SetTimeScale(60f); // 1 sec = 1 minute
    }
    
    private void PlayTimeFast()
    {
        timeManager.SetPaused(false);
        timeManager.SetTimeScale(600f); // 1 sec = 10 minutes
    }
}
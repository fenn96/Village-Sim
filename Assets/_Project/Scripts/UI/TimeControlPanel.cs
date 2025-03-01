using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeControlPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private GameObject panelObject;
    
    [Header("Time Controls")]
    [SerializeField] private Slider timeScaleSlider;
    [SerializeField] private TextMeshProUGUI timeScaleText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button playFastButton;
    
    [Header("Time Jump")]
    [SerializeField] private Button addHourButton;
    [SerializeField] private Button addDayButton;
    [SerializeField] private Button addSeasonButton;
    [SerializeField] private Button addYearButton;
    
    [Header("Debug")]
    [SerializeField] private TextMeshProUGUI debugTimeText;
    
    private bool isPanelVisible = false;
    
    private void Start()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance;
        }
        
        // Hide panel initially
        panelObject.SetActive(true);
        
        // Set up controls
        SetupControls();
        
        // Subscribe to time events
        timeManager.OnTimeChanged += UpdateDebugTime;
    }
    
    private void Update()
    {
        // Toggle panel with F1 key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            TogglePanel();
        }
    }
    
    private void SetupControls()
    {
        // Time scale slider
        if (timeScaleSlider != null)
        {
            timeScaleSlider.minValue = 0;
            timeScaleSlider.maxValue = 1000;
            timeScaleSlider.value = timeManager.GetGameTimeScale();
            timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
            UpdateTimeScaleText(timeManager.GetGameTimeScale());
        }
        
        // Buttons
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseTime);
        
        if (playButton != null)
            playButton.onClick.AddListener(ResumeTime);

        if (playFastButton != null)
            playFastButton.onClick.AddListener(PlayTimeFast);
        
        if (addHourButton != null)
            addHourButton.onClick.AddListener(AddHour);
        
        if (addDayButton != null)
            addDayButton.onClick.AddListener(AddDay);
        
        if (addSeasonButton != null)
            addSeasonButton.onClick.AddListener(AddSeason);
        
        if (addYearButton != null)
            addYearButton.onClick.AddListener(AddYear);
    }
    
    private void OnTimeScaleChanged(float value)
    {
        timeManager.SetTimeScale(value);
        UpdateTimeScaleText(value);
    }
    
    private void UpdateTimeScaleText(float value)
    {
        if (timeScaleText != null)
        {
            timeScaleText.text = $"Time Scale: {value:F1}x";
        }
    }
    
    private void UpdateDebugTime(int hour, int minute)
    {
        if (debugTimeText != null)
        {
            string dayType = timeManager.GetDayType() == DayType.Weekend ? "Weekend" : "Weekday";
            debugTimeText.text = $"Year {timeManager.GetYear()}, {timeManager.GetCurrentSeason()}\n" +
                               $"Day {timeManager.GetCurrentDay()} ({dayType})\n" +
                               $"Time: {hour:00}:{minute:00}";
        }
    }
    
    private void TogglePanel()
    {
        isPanelVisible = !isPanelVisible;
        panelObject.SetActive(isPanelVisible);
    }
    
    // Time control methods
    private void PauseTime()
    {
        timeManager.SetPaused(true);
    }
    
    private void ResumeTime()
    {
        timeManager.SetPaused(false);
        timeManager.SetTimeScale(60f); // 1 sec = 10 minutes
    }

    private void PlayTimeFast()
    {
        timeManager.SetPaused(false);
        timeManager.SetTimeScale(600f); // 1 sec = 10 minutes
        Debug.Log("Set time scale to fast: " + timeManager.GetGameTimeScale());
    }
    
    private void AddHour()
    {
        // Add 60 minutes to advance by one hour
        int currentHour = timeManager.GetCurrentHour();
        int currentMinute = timeManager.GetCurrentMinute();
        
        // Create enough time to advance exactly one hour
        float timeToAdd = 60f * 60f / timeManager.GetGameTimeScale();
        timeManager.UpdateGameTime(timeToAdd);
    }

    private void AddDay()
    {
        // Add 24 hours (1440 minutes) to advance by one day
        float timeToAdd = 24f * 60f * 60f / timeManager.GetGameTimeScale();
        timeManager.UpdateGameTime(timeToAdd);
    }
    
    private void AddSeason()
    {
        // Add 28 days to force a season change
        timeManager.UpdateGameTime(28f * 24f * 60f * 60f / timeManager.GetGameTimeScale());
    }
    
    private void AddYear()
    {
        // Add 4 seasons to force a year change
        timeManager.UpdateGameTime(4f * 28f * 24f * 60f * 60f / timeManager.GetGameTimeScale());
    }
    
    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= UpdateDebugTime;
        }
    }
}
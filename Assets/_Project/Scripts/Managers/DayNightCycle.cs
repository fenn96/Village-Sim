using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private SpriteRenderer overlayRenderer;
    
    [Header("Settings")]
    [SerializeField] private Color daytimeColor = new Color(0, 0, 0, 0); // Transparent
    [SerializeField] private Color nightColor = new Color(0, 0, 0.5f, 0.5f); // Dark blue with alpha
    [SerializeField] private int sunriseHour = 6;
    [SerializeField] private int sunsetHour = 18;
    [SerializeField] private int transitionDuration = 2; // Hours of transition
    
    private void Start()
    {
        if (timeManager == null)
        {
            timeManager = TimeManager.Instance;
        }
        
        if (overlayRenderer == null)
        {
            Debug.LogError("Day/Night cycle overlay renderer not assigned.");
            return;
        }
        
        // Subscribe to time changes
        timeManager.OnTimeChanged += UpdateLighting;
        
        // Initial update
        UpdateLighting(timeManager.GetCurrentHour(), timeManager.GetCurrentMinute());
    }
    
    private void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnTimeChanged -= UpdateLighting;
        }
    }
    
    private void UpdateLighting(int hour, int minute)
    {
        // Calculate the blend factor for day/night transition
        float timeOfDay = hour + minute / 60f;
        float blendFactor;
        
        // Morning transition
        if (timeOfDay >= sunriseHour - transitionDuration/2f && timeOfDay <= sunriseHour + transitionDuration/2f)
        {
            blendFactor = 1f - (timeOfDay - (sunriseHour - transitionDuration/2f)) / transitionDuration;
        }
        // Evening transition
        else if (timeOfDay >= sunsetHour - transitionDuration/2f && timeOfDay <= sunsetHour + transitionDuration/2f)
        {
            blendFactor = (timeOfDay - (sunsetHour - transitionDuration/2f)) / transitionDuration;
        }
        // Daytime
        else if (timeOfDay > sunriseHour + transitionDuration/2f && timeOfDay < sunsetHour - transitionDuration/2f)
        {
            blendFactor = 0f;
        }
        // Nighttime
        else
        {
            blendFactor = 1f;
        }
        
        // Apply the color to the overlay
        overlayRenderer.color = Color.Lerp(daytimeColor, nightColor, blendFactor);
    }
}
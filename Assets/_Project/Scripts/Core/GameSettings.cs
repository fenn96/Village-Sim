// GameSettings.cs
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "LLM Village/Settings/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Game World Settings")]
    public int worldSizeX = 100;
    public int worldSizeY = 100;
    public float tileSize = 1.0f;
    
    [Header("Villager Settings")]
    public int maxVillagers = 30;
    public float villagerMoveSpeed = 3.0f;
    
    [Header("Time Settings")]
    public float defaultTimeScale = 60.0f;
    public int daysPerSeason = 30;
    public int startHour = 8;
    
    [Header("LLM Settings")]
    public int dailyBudgetPerVillager = 10;
    public int villageDailyBudget = 300;
    public string defaultApiEndpoint = "https://api.openai.com/v1/chat/completions";
    public string defaultModelName = "gpt-4";
    
    [Header("2D Specific Settings")]
    public float cameraSize = 10f;
    public float cameraZoomMin = 5f;
    public float cameraZoomMax = 15f;
    public bool usePixelPerfectCamera = true;
}

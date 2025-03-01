// TilemapManager.cs
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    public static TilemapManager Instance { get; private set; }
    
    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap objectTilemap;
    [SerializeField] private Tilemap buildingTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    
    [Header("Settings")]
    [SerializeField] private int mapWidth = 100;
    [SerializeField] private int mapHeight = 100;
    
    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }
    
    // Convert world position to grid cell position
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return groundTilemap.WorldToCell(worldPosition);
    }
    
    // Convert grid cell position to world position (centered in cell)
    public Vector3 CellToWorld(Vector3Int cellPosition)
    {
        return groundTilemap.GetCellCenterWorld(cellPosition);
    }
    
    // Check if a cell position is walkable (not blocked)
    public bool IsCellWalkable(Vector3Int cellPosition)
    {
        // A cell is walkable if it has no collision tile
        return !collisionTilemap.HasTile(cellPosition);
    }
    
    // Get nearest walkable cell to a given position
    public Vector3Int GetNearestWalkableCell(Vector3Int cellPosition)
    {
        // Start with the original position
        if (IsCellWalkable(cellPosition)) return cellPosition;
        
        // Spiral outward to find nearest walkable cell
        for (int radius = 1; radius < 10; radius++) // Limit search radius
        {
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    // Only check cells at the current radius
                    if (Mathf.Abs(x) == radius || Mathf.Abs(y) == radius)
                    {
                        Vector3Int checkPos = cellPosition + new Vector3Int(x, y, 0);
                        if (IsCellWalkable(checkPos))
                        {
                            return checkPos;
                        }
                    }
                }
            }
        }
        
        // If no walkable cell found, return original (though it's not walkable)
        return cellPosition;
    }
    
    // Place a tile at the specified position
    public void PlaceTile(TileBase tile, Vector3Int position, TilemapType type)
    {
        Tilemap targetTilemap = GetTilemapByType(type);
        targetTilemap.SetTile(position, tile);
    }
    
    // Remove a tile at the specified position
    public void RemoveTile(Vector3Int position, TilemapType type)
    {
        Tilemap targetTilemap = GetTilemapByType(type);
        targetTilemap.SetTile(position, null);
    }
    
    // Get tilemap by type
    private Tilemap GetTilemapByType(TilemapType type)
    {
        switch (type)
        {
            case TilemapType.Ground:
                return groundTilemap;
            case TilemapType.Object:
                return objectTilemap;
            case TilemapType.Building:
                return buildingTilemap;
            case TilemapType.Collision:
                return collisionTilemap;
            default:
                return groundTilemap;
        }
    }
}

public enum TilemapType
{
    Ground,
    Object,
    Building,
    Collision
}
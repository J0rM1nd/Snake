using Unity.VisualScripting;
using UnityEngine;

public class Map
{
    public int FoodNextId{ get; private set; }

    public int[,] _tiles;
    public TileStatus[,] _tileStatuses;

    public void Update(Vector2Int pos, int value, TileStatus status)
    {
        _tiles[pos.x, pos.y] = value;
        _tileStatuses[pos.x, pos.y] = status;
    }

    public (TileStatus, int) Get(Vector2Int pos) => IsOut(pos) ? 
        (TileStatus.Wall, -2) :
        (_tileStatuses[pos.x, pos.y], _tiles[pos.x, pos.y]);

    public void Set(Vector2Int pos, int val, TileStatus status)
    {
        (_tiles[pos.x, pos.y], _tileStatuses[pos.x, pos.y]) = (val, status);
        if (status == TileStatus.Food)
            FoodNextId++;
    }
    
    public bool IsOut(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0)
            return true;
        if (pos.x >= _tiles.GetLength(0) || pos.y >= _tiles.GetLength(1))
            return true;
        return false;
    }

    public Map(Vector2Int mapSize)
    {
        FoodNextId = 0;
        _tiles = new int[mapSize.x, mapSize.y];
        _tileStatuses = new TileStatus[mapSize.x, mapSize.y];
        for (int i = 0; i < mapSize.x; i++)
            for (int j = 0; j < mapSize.y; j++)
            {
                _tiles[i, j] = -1;
                _tileStatuses[i, j] = TileStatus.Empty;
            }
    }
}
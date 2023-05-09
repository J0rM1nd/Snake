using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    [SerializeField]
    private GameObject _bodyPrefab;
    [SerializeField]
    private GameObject _headPrefab;
    [SerializeField]
    private GameObject _foodPrefab;
    [SerializeField]
    private Material team10;
    [SerializeField]
    private Material team11;
    [SerializeField]
    private Material team20;
    [SerializeField]
    private Material team21;

    private Map _map;
    private List<Snake2> _snakes;
    private List<GameObject> _food;

    private float _foodTimer;
    private List<float> _snakeTimers;
    private List<List<int>> _grawthTickTimer;

    public float FoodSpawnRate { get; private set; }

    public Map Map { get { return _map; } }
    public int SnakesCount { get; private set; }
    public Vector2Int MapSize { get; private set; }


    private void Update()
    {
        for (int i = 0; i < _snakes.Count; i++)
        {
            _snakeTimers[i] += Time.deltaTime;
            if (_snakeTimers[i] > 1 / _snakes[i].Speed)
            {
                _snakeTimers[i] = 0;
                switch (_snakes[i].TryMoove().Item2)
                {
                    case TileStatus.Empty:
                        _snakes[i].Step(_snakes[i].Speed);
                        break;
                    case TileStatus.Food:
                        _snakes[i].AddEatenFood( _snakes[i].TryMoove().Item1, 
                            FoodByIdOrNull(_snakes[i].TryMoove().Item3) );
                        _grawthTickTimer[i].Add(_snakes[i].Length);
                        _snakes[i].Step(_snakes[i].Speed);
                        break;
                    case TileStatus.Body:
                    case TileStatus.Head:
                    case TileStatus.Wall:
                        SnakeDie(_snakes[i].ID);
                        break;
                }
            }
        }
        UpdateFood();
    }

    public void Awake()//RABOTAYET, NE TROZH
    {
        _food = new List<GameObject>();
        FoodSpawnRate = 2;
        _foodTimer = 0;
        MapSize = new(50, 50);
        _map = new(MapSize);
        SnakesCount = 4;
        _snakes = new List<Snake2>();
        _grawthTickTimer = new();
        _snakeTimers = new List<float>();
        for (int i = 0; i < SnakesCount; i++)
        {
            _snakeTimers.Add(0);
            _grawthTickTimer.Add(new List<int>());
        }
        Vector2Int[] poss = new Vector2Int[3];
        int y;
        List<Material> list = new List<Material> { team10, team20, team11, team21 };
        List<GameObject> prefabs;
        y = 0;
        for (int i = 0; i < SnakesCount / 2; i++)
        {
            prefabs = new List<GameObject> { Instantiate(_headPrefab) };
            for (int j = 1; j < poss.Length; j++)
                prefabs.Add(Instantiate(_bodyPrefab));
            y = 2 * i;
            (poss[0], poss[1], poss[2]) = (new(2, y), new(1, y), new(0, y));
            _snakes.Add(new(this, poss, prefabs.ToArray(), 2 * i, Direction2.Right,
                _bodyPrefab, list[i * 2]));
            prefabs = new List<GameObject> { Instantiate(_headPrefab) };
            for (int j = 1; j < poss.Length; j++)
                prefabs.Add(Instantiate(_bodyPrefab));
            y = MapSize.y - 1 - 2 * i;
            (poss[0], poss[1], poss[2]) =
                (new(MapSize.x - 3, y), new(MapSize.x - 2, y), new(MapSize.x - 1, y));
            _snakes.Add(new(this, poss, prefabs.ToArray(), 2 * i + 1, Direction2.Left,
                _bodyPrefab, list[i * 2 + 1]));
        }
        for (int i = 0; i < _snakes.Count; i++)
            _snakes[i].Speed = 5f;
    }


    public void UpdateMoove(int player, Direction2 direction)
    {
        for (int i = 0; i < _snakes.Count; i++)
            if (_snakes[i].ID == player)
                _snakes[i].UpdateDirection(direction);
    }

    private void UpdateFood()
    {
        _foodTimer += Time.deltaTime;
        if (_foodTimer > FoodSpawnRate)
        {
            _foodTimer = 0;
            Vector2Int pos = new(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y));
            if (_map.Get(pos).Item1 == TileStatus.Empty)
                SpawnFood(pos);
        }
    }

    public void SpawnFood(Vector2Int pos)
    {
        _food.Add(Instantiate(_foodPrefab, Vector2To3(pos) + new Vector3(0f, 0.19f, 0f),
                    Quaternion.identity));
        _food.Last().GetComponent<Rigidbody>().angularVelocity = new(0f, 2f, 0f);
        _food.Last().GetComponent<Food>().ID = _map.FoodNextId;
        _map.Set(pos, _food.Last().GetComponent<Food>().ID, TileStatus.Food);
    }


    private void SnakeDie(int id)
    {
        Snake2 s = _snakes[NumById(id)];
        s.Die();
        _snakes.Remove(s);
        if (_snakes.Count == 0 )
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private int NumById(int id)
    {
        for (int i = 0; i < _snakes.Count; i++)
            if (_snakes[i].ID == id)
                return i;
        return -1;
    }

    private GameObject FoodByIdOrNull(int id)
    {
        for (int i = 0; i < _food.Count; i++)
            if (_food[i].GetComponent<Food>().ID == id)
                return _food[i];
        throw new System.Exception();
    }

    private int? FoodNumByIdOrNull(int id)
    {
        for (int i = 0; i < _snakes.Count; i++)
            if (_food[i].GetComponent<Food>().ID == id)
                return i;
        return null;
    }

    public void RemooveFood(GameObject food)
    {
        _food.Remove(food);
        Destroy(food);

    }


    public Vector3 Vector2To3(Vector2Int vect) =>
        new(vect.x - MapSize.x / 2, 0.5f, vect.y - MapSize.y / 2);
    public Vector3 Velocity2To3(Vector2Int vect) =>
        new(vect.x, 0, vect.y);
}
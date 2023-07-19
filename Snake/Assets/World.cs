using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour
{
    [SerializeField]
    private GameObject _bodyPrefab_;
    [SerializeField]
    private GameObject _headPrefab_;
    [SerializeField]
    private GameObject _foodPrefab_;

    private float _timeAccelerationConst;
    private float _timeAccelerationConst1;
    private float _timeAccelerationTime;
    private float _timeAccelerationCoefficient;
    private float _timeAcceleration;

    private Map _map;
    private List<Snake> _snakes;
    private GameObject[,] _food;

    private GameMode _gameMode;

    public GameObject Food(Vector2Int pos) => _food[pos.x, pos.y];

    public InputSettings[] InputSettings;

    public bool StartGame;

    private float _foodTimer;

    public float FoodSpawnRate { get; private set; }

    public Map Map { get { return _map; } }
    public int SnakesCount { get; private set; }
    public Vector2Int MapSize { get; private set; }


    private void Update()
    {
        if (!StartGame) return;
        int l = _snakes.Count;
        _timeAccelerationTime += Time.deltaTime;
        if (_timeAccelerationTime > _timeAccelerationConst / _timeAccelerationCoefficient)
        {
            _timeAcceleration *= _timeAccelerationConst1;
            _timeAccelerationTime = 0f;
        }
        for (int i = 0; i < l; i++)
        {
            _snakes[i].TurnTimer -= Time.deltaTime;
            if (_snakes[i].TurnTimer < 0)
            {
                _snakes[i].TurnTimer = 1 / _snakes[i].Speed / _timeAcceleration;
                switch (_snakes[i].TryMoove().Item2)
                {
                    case TileStatus.Empty:
                        _snakes[i].Step(_snakes[i].Speed * _timeAcceleration);
                        break;
                    case TileStatus.Food:
                        _snakes[i].AddEatenFood(_snakes[i].TryMoove().Item1);
                        _snakes[i].Step(_snakes[i].Speed * _timeAcceleration);
                        break;
                    case TileStatus.Body:
                    case TileStatus.Wall:
                        SnakeDie(_snakes[i].ID);
                        i--;
                        l--;
                        break;
                    case TileStatus.Head:
                        SnakeDie(_snakes[i].TryMoove().Item3);
                        SnakeDie(_snakes[i].ID);
                        i--;
                        l -= 2;
                        break;
                }
            }
        }
        UpdateFood();
    }

    public void SetSettingsAndStart(int playersCount, GameMode gameMode, int timeAcceleration, int startSpeed, Vector2Int mapSize, InputSettings[] inputSettings, Material mat)
    {
        _gameMode = gameMode;
        GameObject _bodyPrefab = Instantiate(_bodyPrefab_);
        GameObject _headPrefab = Instantiate(_headPrefab_);
        GameObject _foodPrefab = Instantiate(_foodPrefab_);
        InputSettings = inputSettings;
        _timeAccelerationCoefficient = timeAcceleration;
        FoodSpawnRate = 1;
        _foodTimer = 0;
        MapSize = new(mapSize.x, mapSize.y);
        _map = new(MapSize);
        _food = new GameObject[MapSize.x, MapSize.y];
        _foodPrefab.transform.localScale /= MapScale;
        _bodyPrefab.transform.localScale /= MapScale;
        _headPrefab.transform.localScale /= MapScale;
        SnakesCount = playersCount * 2;
        _snakes = new List<Snake>();
        List<GameObject> prefabs;
        Material[] materials = new Material[3];
        materials[0] = new(mat); materials[1] = new(mat); materials[2] = new(mat);
        for (int i = 0; i < playersCount; i++)
        {
            materials[0] = new(mat); materials[1] = new(mat); materials[2] = new(mat);
            materials[0].mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Teams/Team" + (i + 1) + "/Mat1.png");
            materials[1].mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Teams/Team" + (i + 1) + "/Mat2.png");
            materials[2].mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Materials/Teams/Team" + (i + 1) + "/Mat2S.png");
            prefabs = new List<GameObject> { Instantiate(_headPrefab) };
            for (int j = 1; j < 3; j++)
                prefabs.Add(Instantiate(_bodyPrefab));
            var v = StartPositions(playersCount, 2 * i);
            _snakes.Add(new(this, i, v.Item1, prefabs.ToArray(), 2 * i, v.Item2, _bodyPrefab, materials[0], materials[0]));
            prefabs = new List<GameObject> { Instantiate(_headPrefab) };
            for (int j = 1; j < 3; j++)
                prefabs.Add(Instantiate(_bodyPrefab));
            v = StartPositions(playersCount, 2 * i + 1);
            _snakes.Add(new(this, i, v.Item1, prefabs.ToArray(), 2 * i + 1, v.Item2, _bodyPrefab, materials[1], materials[2]));
        }

        for (int i = 0; i < _snakes.Count; i++)
            _snakes[i].Speed = startSpeed;
        RescaleBounds();
        StartGame = true;
    }

    public void Awake()
    {
        _timeAcceleration = 1f;
        _timeAccelerationConst = 30f;
        _timeAccelerationConst1 = 1.1f;
        StartGame = false;
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
        GameObject f = Instantiate(_foodPrefab_, Vector2To3(pos) + new Vector3(0f, 0.19f / MapScale, 0f), Quaternion.identity);
        f.transform.localScale /= MapScale;
        f.transform.position = new(f.transform.position.x, f.transform.position.y / MapScale, f.transform.position.z);
        f.GetComponent<Rigidbody>().angularVelocity = new(0f, 0.5f * _snakes[0].Speed * _timeAcceleration, 0f);
        _food[pos.x, pos.y] = f;
        _map.Set(pos, 1, TileStatus.Food);
    }

    private (Vector2Int[], Direction2) StartPositions(int playersCount, int snakeNum)
    {
        Vector2Int[] res = new Vector2Int[3];
        switch (playersCount)
        {
            case 2:
                switch (snakeNum)
                {
                    case 0:
                        res[0] = new(3, 2); res[1] = new(2, 2); res[2] = new(1, 2);
                        return (res, Direction2.Right);
                    case 1:
                        res[0] = new(3, 4); res[1] = new(2, 4); res[2] = new(1, 4);
                        return (res, Direction2.Right);
                    case 2:
                        res[0] = new(MapSize.x - 4, MapSize.y - 3); res[1] = new(MapSize.x - 3, MapSize.y - 3); res[2] = new(MapSize.x - 2, MapSize.y - 3);
                        return (res, Direction2.Left);
                    case 3:
                        res[0] = new(MapSize.x - 4, MapSize.y - 5); res[1] = new(MapSize.x - 3, MapSize.y - 5); res[2] = new(MapSize.x - 2, MapSize.y - 5);
                        return (res, Direction2.Left);
                    default:
                        throw new System.Exception();
                }
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            case 6:
                break;
            case 7:
                break;
            case 8:
                break;
            case 9:
                break;
            case 10:
                break;
            case 11:
                break;
            case 12:
                break;
            default:
                throw new System.Exception("Players count is out of range");
        }
        throw new System.Exception("Need code");
    }


    private void SnakeDie(int id)
    {
        Snake s = _snakes[NumById(id)];
        s.Die();
        _snakes.Remove(s);
        if (_snakes.Count == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    private int NumById(int id)
    {
        for (int i = 0; i < _snakes.Count; i++)
            if (_snakes[i].ID == id)
                return i;
        return -1;
    }

    private void RescaleBounds()
    {
        Vector3 c = Vector2To3(new(0, 0));
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            switch (gameObject.transform.GetChild(i).name)
            {
                case "BoundL":
                    gameObject.transform.GetChild(i).transform.localScale = new Vector3(
                        gameObject.transform.GetChild(i).transform.localScale.x / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.y / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.z);
                    gameObject.transform.GetChild(i).transform.position = new Vector3(
                        c.x,
                        c.y,
                        gameObject.transform.GetChild(i).transform.position.z);
                    break;
                case "BoundR":
                    gameObject.transform.GetChild(i).transform.localScale = new Vector3(
                        gameObject.transform.GetChild(i).transform.localScale.x / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.y / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.z);
                    gameObject.transform.GetChild(i).transform.position = new Vector3(
                        -c.x,
                        c.y,
                        gameObject.transform.GetChild(i).transform.position.z);
                    break;
                case "BoundB":
                    gameObject.transform.GetChild(i).transform.localScale = new Vector3(
                        gameObject.transform.GetChild(i).transform.localScale.x / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.y / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.z);
                    gameObject.transform.GetChild(i).transform.position = new Vector3(
                        gameObject.transform.GetChild(i).transform.position.x,
                        c.y,
                        c.z);
                    break;
                case "BoundT":
                    gameObject.transform.GetChild(i).transform.localScale = new Vector3(
                        gameObject.transform.GetChild(i).transform.localScale.x / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.y / MapScale,
                        gameObject.transform.GetChild(i).transform.localScale.z);
                    gameObject.transform.GetChild(i).transform.position = new Vector3(
                        gameObject.transform.GetChild(i).transform.position.x,
                        c.y,
                        -c.z);
                    break;
            }
        }
    }

    public void RemooveFood(Vector2Int pos)
    {
        Destroy(Food(pos));
        _food[pos.x, pos.y] = null;
    }

    public int MapScale { get { return MapSize.x / 16; } }
    public Vector3 Vector2To3(Vector2Int vect) => Vector2To3(new Vector3(vect.x, vect.y));
    public Vector3 Vector2To3(Vector2 vect)
    {
        float k = 5f / MapScale;
        return new(2f * k * vect.x - 80f + k, k, 2f * k * vect.y - 45f + k);
    }
    public Vector3 Velocity2To3(Vector2Int vect) => new(10f * vect.x / MapScale, 0, 10f * vect.y / MapScale);
}
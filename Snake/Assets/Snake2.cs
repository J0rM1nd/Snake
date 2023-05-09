using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake2
{
    public GameObject BodyPrefab;
    private Material _material;

    private List<(Vector2Int, GameObject)> _eatenFood;

    public int SnakeId;

    private World _world;
    private Head _head;
    private Segment _tail;

    public float Speed { get; set; }
    public int Length { get; private set; }
    public int ID { get; private set; }

    public void UpdateDirection(Direction2 direction) => _head.Direction = direction;



    public (Vector2Int, TileStatus, int) TryMoove() => _head.TryMoove(_world);

    public void Die()
    {
        Segment previous = _tail;
        Segment next = _tail;
        for (int i = 0; i < _eatenFood.Count; i++)
            _world.RemooveFood(_eatenFood[0].Item2);
        for (int i = 0; i < Length; i++)
        {
            next = previous.Previous;
            previous.Die(_world);
            previous = next;
        }
    }

    public void AddEatenFood(Vector2Int pos, GameObject food)
    {
        food.GetComponent<Renderer>().material = _material;
        _eatenFood.Add( (pos, food) );
    }

    public void Step(float speed)
    {
        bool growth = false;
        Segment s;
        if (_eatenFood.Count > 0)
            if (_tail.Pos == _eatenFood[0].Item1)//growth
            {
                growth = true;
                _world.RemooveFood(_eatenFood[0].Item2);
                Segment newTail = _tail.CreateNext(BodyPrefab, _world, _tail.Pos, _material);
                _tail = newTail;
                s = _tail.Previous;
                _eatenFood.RemoveAt(0);
            }
            else
            {
                s = _tail;
            }
        else
            s = _tail;
        for (int i = 0; i < Length; i++)//moove
        {
            if (i == 0)
                s.Moove(speed, _world, true);
            else
                s.Moove(speed, _world);
            s = s.Previous;
        }
        if (growth)
            Length++;
    }

    public Snake2(World world, Vector2Int[] pos, GameObject[] gameObjects, int ID,
        Direction2 direction, GameObject bodyPrefab, Material material)
    {
        _eatenFood = new List<(Vector2Int, GameObject)>();
        BodyPrefab = bodyPrefab;
        _material = material;
        _world = world;
        this.ID = ID;
        _head = gameObjects[0].GetComponent<Head>();
        _head.Init(world, ID, pos[0], direction, material);
        world.Map.Set(pos[0], ID, TileStatus.Head);
        Segment previous = _head;
        for (int i = 1; i < pos.Length; i++)
        {
            world.Map.Set(pos[i], ID, TileStatus.Body);
            previous.Next = gameObjects[i].GetComponent<Segment>();
            previous.Next.Init(world, ID, pos[i], material, previous);
            previous = previous.Next;
        }
        _tail = previous;
        Length = pos.Length;
    }
}
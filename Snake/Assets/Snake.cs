using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class Snake
{
    private Vector2Int _head;
    private int _tail;
    private List<Vector2Int> _body;
    private Vector2Int _direction;
    private Vector2Int _lastDirection;


    public List<int> TargetIDs;
    public int ID;

    public List<Vector2Int> Body { get => _body; }
    public Vector2Int Head { get => _head; }

    public Direction2 Direction
    {
        get
        {
            if (_direction.x != 0)
                if (_direction.x > 0)
                    return Direction2.Right;
                else
                    return Direction2.Left;
            if (_direction.y > 0)
                return Direction2.Up;
            return Direction2.Down;
        }
        set
        {
            switch (value)
            {
                case Direction2.Up:
                    (_direction.x, _direction.y) = (0, 1);
                    break;
                case Direction2.Down:
                    (_direction.x, _direction.y) = (0, -1);
                    break;
                case Direction2.Right:
                    (_direction.x, _direction.y) = (1, 0);
                    break;
                case Direction2.Left:
                    (_direction.x, _direction.y) = (-1, 0);
                    break;
                default:
                    throw new System.Exception();
            }
        }
    }
    public void Step(out Vector2Int head, out Vector2Int? leaveOrNull)
    {
        leaveOrNull = _body[_tail];
        _body[_tail] = _head;
        if (_direction == -_lastDirection)
            _direction = _lastDirection;
        _head += _direction;
        _lastDirection = _direction;
        head = _head;
        _tail++;
        _tail %= _body.Count;
    }
    public void GrowthStep(out Vector2Int head, out Vector2Int? leaveOrNull)
    {
        List<Vector2Int> newBody = new List<Vector2Int> { _head };
        for (int i = 0; i < _body.Count; i++)
        {
            newBody.Add(_body[(_tail + i) % _body.Count]);
        }
        if (_direction == -_lastDirection)
            _direction = _lastDirection;
        _head += _direction;
        _lastDirection = _direction;
        (head, leaveOrNull) = (_head, null);
        _tail = 1;
        _body = newBody;
    }

    public Snake(Vector2Int[] points, int ID, List<int> targetIDs, Direction2 direction)
    {
        this.ID = ID;
        TargetIDs = targetIDs.ToList();
        _head = points[0];
        _body = new List<Vector2Int>();
        for (int i = 1; i < points.Length; i++)
            _body.Add(points[i]);
        _direction = new();
        Direction = direction;
        _lastDirection = _direction;
    }
}
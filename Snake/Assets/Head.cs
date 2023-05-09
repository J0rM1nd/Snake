using UnityEngine;

public class Head : Segment
{
    private Direction2 _oldDirection;
    private Direction2 _newDirection;

    public Direction2 Direction
    {
        get { return _newDirection; }
        set 
        { 
            if ( DirectionToVector(value) != - DirectionToVector(_oldDirection) )
                _newDirection = value; 
        }
    }

    public ( Vector2Int, TileStatus, int) TryMoove(World world) => 
        ( Pos + MooveVector(), 
        world.Map.Get(Pos + MooveVector()).Item1, 
        world.Map.Get(Pos + MooveVector()).Item2 );

    public override void Moove(float speed, World world, bool notUse = false)
    {
        _oldDirection = _newDirection;
        transform.position = world.Vector2To3(Pos);
        GetComponent<Rigidbody>().velocity =
            world.Velocity2To3( MooveVector() ) * speed;
        world.Map.Update(Pos, _snakeId, TileStatus.Body);
        Pos += MooveVector();
        world.Map.Update(Pos, _snakeId, TileStatus.Head);
    }

    private Vector2Int MooveVector()
    {
        Vector2Int d = new(0, 0);
        switch (Direction)
        {
            case Direction2.Up:
                d.y = 1;
                break;
            case Direction2.Down:
                d.y = -1;
                break;
            case Direction2.Left:
                d.x = -1;
                break;
            case Direction2.Right:
                d.x = 1;
                break;
        }
        return d;
    }
    private static Vector2Int DirectionToVector(Direction2 direction)
    {
        Vector2Int d = new(0, 0);
        switch (direction)
        {
            case Direction2.Up:
                d.y = 1;
                break;
            case Direction2.Down:
                d.y = -1;
                break;
            case Direction2.Left:
                d.x = -1;
                break;
            case Direction2.Right:
                d.x = 1;
                break;
        }
        return d;
    }

    public void Init(World world, int snakeId, Vector2Int pos,
        Direction2 direction, Material material, Segment previous = null, Segment next = null)
    {
        Init(world, snakeId, pos, material, previous, next);
        _oldDirection = direction;
        _newDirection = direction;
        world.Map.Update(pos, snakeId, TileStatus.Head);
    }
}
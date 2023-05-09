using UnityEngine;

public class Segment : MonoBehaviour, ISegment
{
    protected int _snakeId;

    public Vector2Int Pos;
    public Segment Previous;
    public Segment Next;

    public void Die(World world)
    {
        world.SpawnFood(Pos);
        Destroy(gameObject);
    }

    public Segment CreateNext(GameObject prefab, World world, Vector2Int pos, Material material)
    {
        if (Next == null)
        {
            Next = Instantiate(prefab).GetComponent<Segment>();
            Next.Init(world, _snakeId, pos, material, this);
        }
        return Next;
    }

    public virtual void Moove(float speed, World world, bool tail = false)
    {        Vector2Int iv = Previous.Pos - Pos;
        transform.position = world.Vector2To3(Pos);
        GetComponent<Rigidbody>().velocity =
            world.Velocity2To3(iv) * speed;
        if (tail)
            world.Map.Update(Pos, -1, TileStatus.Empty);
        Pos = Previous.Pos;
    }

    public void Init(World world, int snakeId, Vector2Int pos,
        Material material, Segment previous = null, Segment next = null)
    {
        Pos = pos;
        transform.position = world.Vector2To3(pos);
        Previous = previous;
        Next = next;
        _snakeId = snakeId;
        Renderer r = GetComponent<Renderer>();
        r.sharedMaterial = material;
        world.Map.Update(pos, snakeId, TileStatus.Body);
    }
}
using UnityEngine;

public class Food : MonoBehaviour
{
    public int ID;

    public void Start()
    {
        transform.rotation = Quaternion.Euler(45f, 0f, 45f);
    }
}
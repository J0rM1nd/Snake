using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameObject __world;

    private World _world;

    void Start()
    {
        _world = __world.GetComponent<World>();
    }

    void Update()
    {
        if (!_world.StartGame) return;
        for (int i = 0; i < _world.InputSettings.Length; i++)
            PlayerInput(_world.InputSettings[i].Up, _world.InputSettings[i].Left, _world.InputSettings[i].Down, _world.InputSettings[i].Right, i);
    }

    private void PlayerInput(KeyCode w, KeyCode a, KeyCode s, KeyCode d, int playerID)
    {
        if (Input.GetKeyDown(w))
            _world.UpdateMoove(playerID, Direction2.Up);
        if (Input.GetKeyDown(s))
            _world.UpdateMoove(playerID, Direction2.Down);
        if (Input.GetKeyDown(a))
            _world.UpdateMoove(playerID, Direction2.Left);
        if (Input.GetKeyDown(d))
            _world.UpdateMoove(playerID, Direction2.Right);
    }
}

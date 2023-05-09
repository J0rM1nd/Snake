using System.Collections.Generic;
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
        PlayerInput(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, 0);
        PlayerInput(KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, 2);
        PlayerInput(KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow, 1);
        PlayerInput(KeyCode.Keypad5, KeyCode.Keypad1, KeyCode.Keypad2, KeyCode.Keypad3, 3);
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

    //private void Draw()
    //{
    //    for (int i = 0; i < _objs.Count; i++)
    //        Destroy(_objs[i]);
    //    _objs = new();
    //    for (int i = 0; i < _world.MapSize.x; i++)
    //        for (int j = 0; j < _world.MapSize.y; j++)
    //            switch (_world.Map._tileStatuses[i, j])
    //            {
    //                case TileStatus.Body:
    //                    _objs.Add( Instantiate( _bodyPrefab, _world.Vector2To3(new(i, j)), 
    //                        Quaternion.identity) );
    //                    switch (_world.Map._tiles[i, j])
    //                    {
    //                        case 0:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor(
    //                                "_Color", new Color(0, 1, 0));
    //                            break;
    //                        case 1:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor(
    //                                "_Color", new Color(1, 1, 0));
    //                            break;
    //                        case 2:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor("_Color", 
    //                                new Color(0, 0, 1));
    //                            break;
    //                        case 3:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor("_Color", 
    //                                new Color(1, 0, 1));
    //                            break;
    //                    }
    //                    break;
    //                case TileStatus.Head:
    //                    _objs.Add( Instantiate(_headPrefab, _world.Vector2To3(new(i, j)), 
    //                        Quaternion.identity) );
    //                    switch (_world.Map._tiles[i, j])
    //                    {
    //                        case 0:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor(
    //                                "_Color", new Color(0, 1, 0));
    //                            break;
    //                        case 1:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor(
    //                                "_Color", new Color(1, 1, 0));
    //                            break;
    //                        case 2:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor("_Color",
    //                                new Color(0, 0, 1));
    //                            break;
    //                        case 3:
    //                            _objs.Last().GetComponent<Renderer>().material.SetColor("_Color",
    //                                new Color(1, 0, 1));
    //                            break;
    //                    }
    //                    break;
    //                case TileStatus.Food:
    //                    _objs.Add(Instantiate(_foodPrefab, _world.Vector2To3(new(i, j)),
    //                        Quaternion.identity));
    //                    break;
    //            }
    //}

    //private void UpdateStep()
    //{
    //    _timer += Time.deltaTime;
    //    if (_timer >= StepTime)
    //    {
    //        _timer = 0;
    //        _world.Step();
    //    }
    //}


}

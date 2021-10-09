using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public string roomCode = "*****";
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, Car> cars = new Dictionary<int, Car>();
    public static Dictionary<int, Object> objects = new Dictionary<int, Object>();
    public GameObject localPlayer;
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject[] carPrefabs;
    public GameObject[] objectPrefabs;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        Cursor.lockState = CursorLockMode.Confined;
    }
    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            localPlayer = _player;
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<PlayerManager>().Initialize(_id,_username);
        players.Add(_id, _player.GetComponent<PlayerManager>());
    }

    public static void SetRoomCode(string _code){
        GameManager.instance.roomCode = _code;
    }

    public void SpawnCar(int _id, int _type, float _health,int _wheelsNum,int _pass, Vector3 _pos, Quaternion _rot){
        GameObject go = GameObject.Instantiate(carPrefabs[_type], _pos,_rot);
        go.GetComponent<Car>().Initialize(_id,_type,_health,_wheelsNum,_pass);
        cars.Add(_id,go.GetComponent<Car>());
    }

    public void SpawnObject(int _id, int _type, Vector3 _pos, Quaternion _rot){
        GameObject go = GameObject.Instantiate(objectPrefabs[_type],_pos,_rot);
        go.GetComponent<Object>().SetId(_id,_type);
        objects.Add(_id,go.GetComponent<Object>());
    }
}
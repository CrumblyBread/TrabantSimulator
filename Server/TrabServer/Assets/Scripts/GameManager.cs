using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector]public static GameManager instance;
    [Tooltip("Joining code for this room")] [System.NonSerialized]
    public string codeId;
    public GameObject parkingLot;
    public List<Player> players = new List<Player>();
    public Dictionary<int,controller> cars = new Dictionary<int, controller>();
    public Dictionary<int,Object> objects = new Dictionary<int, Object>();
    [HideInInspector]public int h,j;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }

        h = 1;
        j = 1;
        controller[] carsFound = FindObjectsOfType<controller>();
        foreach (controller c in carsFound)
        {
            c.SetId(h);
            cars.Add(h,c);
            h++;
        }
        Object[] objectsFound = FindObjectsOfType<Object>();
        foreach (Object c in objectsFound)
        {
            c.SetId(j);
            objects.Add(j,c);
            j++;
        }
    }
    public void Initialize(string _code){
        codeId = _code;
    }

    public void AddPlayer(Player _player){
        players.Add(_player);
    }

    public void AddCar(controller car){
            car.SetId(h);
            cars.Add(h,car);
            h++;
    }

    public void RemoveCar(controller car){
            cars.Remove(car.identifier);
            Destroy(car.myGo);
    }

    public void AddObject(Object obj){
            obj.SetId(j);
            objects.Add(j,obj);
            j++;
    }

    public void RemoveObject(Object obj){
            cars.Remove(obj.identifier);
            Destroy(obj.myGo);
    }
}

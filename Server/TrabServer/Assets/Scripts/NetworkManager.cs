using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;

    public GameObject[] carPrefabs;

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
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        Server.Start(50, 25565);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, new Vector3(0f, 0.5f, 0f), Quaternion.identity).GetComponent<Player>();
    }

    public controller InstantiateCar(int carType,int playerId){
        controller myCar = Instantiate(carPrefabs[carType],GameManager.instance.parkingLot.transform.GetChild(0).GetChild(playerId - 1).transform.position,GameManager.instance.parkingLot.transform.GetChild(0).GetChild(playerId - 1).transform.rotation).GetComponent<controller>();
        GameManager.instance.AddCar(myCar);
        return myCar;
    }
}
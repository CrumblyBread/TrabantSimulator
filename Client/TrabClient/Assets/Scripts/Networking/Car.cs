using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public int identifier = 0;
    public int type;
    public float health;
    public int wheelNum;
    public int passNum;
    public GameObject wheelMeshes;
    public GameObject passengerMeshes;
    private Transform[] wheels;
    public GameObject[] parts;
    public GameObject[] upgrades;
    private PlayerManager[] passengers;
    public AudioSource enginestartSound;
    public AudioSource shiftSound;
    public AudioSource drillSound;


    public void Initialize(int _id, int _type,float _health, int _wheelsNum,int _passNum){
        identifier = _id;
        if(_type != type){
            Debug.LogError("This is the BAD car");
        }
        health = _health;
        wheelNum = _wheelsNum;
        passNum = _passNum;
        wheels = new Transform[wheelNum];
        passengers = new PlayerManager[passNum];
        for (int i = 0; i < wheelNum; i++)
        {
            wheels[i] = wheelMeshes.transform.GetChild(i);
        }
    }

    public void AddPassenger(PlayerManager _player, int _position){
        if(_position == -1){
            Debug.LogError("You don't fit in car, blyat");
            return;
        }
        passengers[_position] = _player;
        _player.myGfx.SetActive(false);
        passengerMeshes.transform.GetChild(_position).gameObject.SetActive(true);
        if(_position == 0){
            enginestartSound.Play();
            if(_player == GameManager.instance.localPlayer.GetComponent<PlayerManager>()){
                UIManager.instance.DrivingUIOn();
            }
        }
    }

    public void RemovePassenger(PlayerManager _player){
        int poss = -1;
        for (int i = 0; i < passNum; i++)
        {
            if(passengers[i] == _player){
                poss = i;
            }
        }
        if(poss < 0){
            return;
        }
        
        passengers[poss].myGfx.SetActive(true);
        passengerMeshes.transform.GetChild(poss).gameObject.SetActive(false);
        passengers[poss] = null;
        if(poss == 0 && _player == GameManager.instance.localPlayer.GetComponent<PlayerManager>()){
        UIManager.instance.DrivingUIOff();
        }
    }
    
    public void UpdateCarWheels(Vector3[] _poses, Quaternion[] _rotes){
        this.transform.position = _poses[0];
        this.transform.rotation = _rotes[0];
        for (int i = 0; i < _poses.Length - 1; i++)
        {
            wheels[i].transform.position = _poses[i+1];
            wheels[i].transform.rotation = _rotes[i+1];
        }
    }

    public void Damage(int level){
        return;
        /*
        switch(level){
            case 1:
                for (int i = 0; i < parts.Length; i++)
                {
                    if(i < parts.Length){
                        parts[i].SetActive(false);
                    }else{
                        parts[i].SetActive(true);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < parts.Length; i++)
                {
                    if(i < parts.Length){
                        parts[i].SetActive(false);
                    }else{
                        parts[i].SetActive(true);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < parts.Length; i++)
                {
                    if(i < parts.Length){
                        parts[i].SetActive(false);
                    }else{
                        parts[i].SetActive(true);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < parts.Length; i++)
                {
                    if(i < parts.Length){
                        parts[i].SetActive(false);
                    }else{
                        parts[i].SetActive(true);
                    }
                }
                break;
        }
        */
    }

    public void AddPart(int part){
        switch (part)
        {
            case 0: case 1: case 2: case 3:
                //Wheels 1-4
                wheelMeshes.transform.GetChild(part).gameObject.SetActive(!wheelMeshes.transform.GetChild(part).gameObject.activeSelf);
                return;
            case 4:
                //Door
                parts[2].SetActive(!parts[2].activeSelf);
                return;
            case 5:
                //Hood
                parts[0].SetActive(!parts[0].activeSelf);
                return;
            case 6:
                //Trunk
                parts[1].SetActive(!parts[1].activeSelf);
                return;
            case 7:
                //Roof
                parts[3].SetActive(!parts[3].activeSelf);
                return;
            case 8:
                //Steering Column
                parts[4].SetActive(!parts[4].activeSelf);
                return;
            case 9: case 10:
                //SparkPlugs
                parts[part-4].SetActive(!parts[part-4].activeSelf);
                return;
            case 11:
                //Oil Filter
                parts[7].SetActive(!parts[7].activeSelf);
                return;
            case 12:
                //Engine Block
                parts[8].SetActive(!parts[8].activeSelf);
                return;
            case 13:
                //Engine Block
                parts[9].SetActive(!parts[9].activeSelf);
                return;
            case 14:
                //Drivers Seat
                parts[10].SetActive(!parts[10].activeSelf);
                return;
            case 15:
                //Passenger Seat
                parts[11].SetActive(!parts[11].activeSelf);
                return;
            case 16:
                //Back Seat
                parts[12].SetActive(!parts[12].activeSelf);
                return;
            case 17:
                //Fuel Tank
                parts[13].SetActive(!parts[13].activeSelf);
                return;
            case 18:
                //Fuel Tank
                parts[14].SetActive(!parts[14].activeSelf);
                return;
            case 19:
                //Fuel Tank
                parts[15].SetActive(!parts[15].activeSelf);
                return;
        }
    }

    public void Upgrade(int grade){
        drillSound.Play();
        switch (grade)
        {
            case 0:
                upgrades[0].SetActive(false);
                upgrades[1].SetActive(false);
                break;
            case 1:
                upgrades[0].SetActive(true);
                upgrades[1].SetActive(false);
                break;
            case 2:
                upgrades[0].SetActive(false);
                upgrades[1].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void Load(int grade){
        drillSound.Play();
        switch (grade)
        {
            case 0:
                upgrades[0].transform.GetChild(0).gameObject.SetActive(false);
                upgrades[1].transform.GetChild(0).gameObject.SetActive(false);
                break;
            case 1:
                upgrades[0].transform.GetChild(0).gameObject.SetActive(true);
                upgrades[1].transform.GetChild(0).gameObject.SetActive(false);
                break;
            case 2:
                upgrades[0].transform.GetChild(0).gameObject.SetActive(false);
                upgrades[1].transform.GetChild(0).gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}

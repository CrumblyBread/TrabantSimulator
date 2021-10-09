using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    public TMP_Text usernameText;
    public bool isDriving;
    public int driveCarId;
    public GameObject myGfx;
    public GameObject myCam;

    public void Initialize(int _id, string _username){
        id = _id;
        username = _username;
        if(id != Client.instance.myId){
            usernameText.text = username;
        }
    }

    public void IsPassenger(int carId, int _poss){
        if(_poss >= 0){
            isDriving = true;
            driveCarId = carId;
            GameManager.cars[carId].AddPassenger(this,_poss);
        }else{
            isDriving = false;
            driveCarId = 0;
            GameManager.cars[carId].RemovePassenger(this);
        }
    }
}
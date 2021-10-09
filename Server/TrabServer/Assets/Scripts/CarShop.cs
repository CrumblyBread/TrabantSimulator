using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarShop : MonoBehaviour
{
    [Tooltip("Point where to park a car")]
    public GameObject pointer;
    [Tooltip("True if the shop is occupied")]
    public bool hasCar;
    public GameObject optionsObject;
    [HideInInspector]public controller shopCar;

    void Update() {
        RaycastHit hit;
        if(Physics.Raycast(pointer.transform.position, pointer.transform.up,out hit)){
            if(hit.transform.GetComponent<controller>() != null){
                hasCar = true;
                shopCar = hit.transform.GetComponent<controller>();
                optionsObject.SetActive(true);
            }else{
                hasCar = false;
                shopCar = null;
                optionsObject.SetActive(false);
            }
        }
    }

    public void RepairAll(){
        if(shopCar == null){
            return;
        }
        shopCar.Repair(shopCar.car.maxHealth);
    }

    public void RefuelAll(){
        if(shopCar == null){
            return;
        }
        shopCar.car.currentFuel = shopCar.car.maxFuel;
    }

    public void AddBagCarrier(){
        shopCar.upgrades[0].SetActive(true);
        shopCar.upgrades[1].SetActive(false);
        shopCar.grade = 1;
        shopCar.loaded = false;

        ServerSend.updateCars(shopCar,0,1);
    }

    public void AddLogHolder(){
        shopCar.upgrades[0].SetActive(false);
        shopCar.upgrades[1].SetActive(true);
        shopCar.grade = 2;
        shopCar.loaded = false;

        ServerSend.updateCars(shopCar,0,2);
    }
}
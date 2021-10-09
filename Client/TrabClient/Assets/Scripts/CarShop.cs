using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarShop : MonoBehaviour
{
    public GameObject pointer;
    public GameObject paperCan;

    private void Update(){
        RaycastHit hit;
        Debug.DrawRay(pointer.transform.position, pointer.transform.up);
        if(Physics.Raycast(pointer.transform.position, pointer.transform.up,out hit)){
            if(hit.transform.GetComponent<Car>() != null){
                paperCan.SetActive(true);
            }else{
                paperCan.SetActive(false);
            }
        }else{
            paperCan.SetActive(false);
        }
    }
}
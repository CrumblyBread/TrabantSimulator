using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadUnload : MonoBehaviour
{
    public GameObject checkerObject;
    public int comodity;
    public bool load; //loads comodity if true, uloads if false
    public int questId;
    public int storageCount;
    public int maxStorageCount;
    public controller loadCar;

    private void Update() {
        if((load && storageCount == 0) || (!load && maxStorageCount == storageCount) || comodity == 0){
            return;
        }
        RaycastHit info;
        if(Physics.Raycast(checkerObject.transform.position,checkerObject.transform.up,out info)){
            if(info.transform.GetComponent<controller>() != null){
                loadCar = info.transform.GetComponent<controller>();
                if(info.transform.GetComponent<Rigidbody>().velocity == Vector3.zero){
                    if(loadCar.grade == comodity){
                        if(load  && loadCar.loaded == false){
                            loadCar.loaded = true;
                            storageCount--;
                            if(storageCount == 0 && questId != 0){
                                QuestManager.instance.AdvanceQuest(questId);
                            }
                            ServerSend.updateCars(loadCar, 1,comodity);
                        }
                        if(!load  && loadCar.loaded == true){
                            loadCar.loaded = false;
                            storageCount++;
                            if(storageCount == maxStorageCount && questId != 0){
                                QuestManager.instance.AdvanceQuest(questId);
                            }
                            ServerSend.updateCars(loadCar, 1,0);
                        }
                    }
                }
            }
        }
    }


}

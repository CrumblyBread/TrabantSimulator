using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public int identifier = 0;
    public int type = 0;
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 previousScale;
    [HideInInspector]public GameObject myGo;

    public void SetId(int value){
        identifier = value;
    }

    private void Start() {
        myGo = this.gameObject;

        previousPosition = this.transform.position;
        previousRotation = this.transform.rotation;
        previousScale = this.transform.localScale;
    }

    private void Update() {
        if(this.transform.position != previousPosition){
            ServerSend.UpdateObject(this,this.transform.position,this.transform.rotation);
        }
        if(this.transform.rotation != previousRotation){
            ServerSend.UpdateObject(this,this.transform.position,this.transform.rotation);
        }
        if(this.transform.localScale != previousScale){
            ServerSend.UpdateObject(this,this.transform.position,this.transform.rotation);
        }

        previousPosition = this.transform.position;
        previousRotation = this.transform.rotation;
        previousScale = this.transform.localScale;
    }

    public void DestroyMe(){
        ServerSend.UpdateObject(this,this.transform.position,this.transform.rotation,true);
        Destroy(myGo);
    }
}

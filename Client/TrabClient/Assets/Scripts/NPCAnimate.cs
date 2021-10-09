using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimate : MonoBehaviour
{
    public Animator anim;
    private Vector3 lastPos;
    private float forward;

    void Start(){
        anim = this.GetComponent<Animator>();
        lastPos = this.transform.position;
    }

    void Update()
    {
        if(this.transform.position != lastPos){
            forward = 0.2f;
        }else{
            forward = 0f;
        }
        anim.SetFloat("Forward",forward);

        lastPos = this.transform.position;
    }
}

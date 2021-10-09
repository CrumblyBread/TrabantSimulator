using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    public int identifier;
    public int type = 0;
    public string objectName;

    public void SetId(int _id, int _type){
        identifier = _id;
        type = _type;
    }

    public void UpdateObj(Vector3 _pos, Quaternion _rot){
        this.transform.position = _pos;
        this.transform.rotation = _rot;
    }
}
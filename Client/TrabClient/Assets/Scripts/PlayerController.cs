using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Inputs[] _inputs;

    private void Start() {
        _inputs = new Inputs[8];
    }

    private void Update() {
        _inputs = new Inputs[]
        {
            new Inputs(Input.GetKey(KeyCode.W)),
            new Inputs(Input.GetKey(KeyCode.S)),
            new Inputs(Input.GetKey(KeyCode.A)),
            new Inputs(Input.GetKey(KeyCode.D)),
            new Inputs(Input.GetKeyDown(KeyCode.Space)),
            new Inputs(Input.GetKeyDown(KeyCode.E)),
            new Inputs(-Input.GetAxis("Mouse Y")),
            new Inputs(Input.GetAxis("Mouse X")),
        };

        ClientSend.PlayerMovement(_inputs);
    }
}

public class Inputs
{
    public int tp;
    public bool inpBool;
    public string inpString;
    public float inpFloat;
    public int inpInt;

    public Inputs(){
        tp = -1;
    }
    public Inputs (bool _input){
        tp = 0;
        inpBool = _input;
    }

    public Inputs (string _input){
        tp = 1;
        inpString = _input;
    }

    public Inputs (int _input){
        tp = 2;
        inpInt = _input;
    }
    public Inputs (float _input){
        tp = 3;
        inpFloat = _input;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    private Rigidbody rb;
    private CapsuleCollider col;
    public float gravity = -9.81f;
    public float moveSpeed = 3f;
    public float jumpSpeed = 30f;

    public GameObject fakeCam;

    private Inputs[] inputs;
    public bool canMove;
    public bool isDriving;            //means if is in car, don't ask me why I called it that
    public int passNumber;
    private Quaternion rotation;
    private Quaternion camRot;
    public controller drivingCar;
    private float verticalRotation;
    private float horizontalRotation;
    private bool carCam = false;
    private bool held;
    [HideInInspector] public bool holdingObject = false;
    [HideInInspector] public GameObject heldObject;
    [Header("Player Stats")]
    public float money;
    [HideInInspector] public List<controller> ownedCars;

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        inputs = new Inputs[8]{
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs()
        };
        canMove = true;
        isDriving = false;
        verticalRotation = fakeCam.transform.localEulerAngles.x;
        horizontalRotation = this.transform.eulerAngles.y;
        carCam = false;
        rb = this.GetComponent<Rigidbody>();
        col = this.GetComponent<CapsuleCollider>();
    }

    public void Initialize(int _id, string _username)
    {
        id = _id;
        username = _username;
        passNumber = -1;
    }

    public void StartDriving(controller car){
            col.enabled = false;
            rb.useGravity = false;
            drivingCar = car;
            drivingCar.StartEngine();
            drivingCar.StartDriving();
            passNumber = drivingCar.AddPassenger(this);
            isDriving = true;
            canMove = false;
            this.transform.position = drivingCar.transform.position;
            fakeCam.transform.position = drivingCar.cams.transform.GetChild(passNumber).position;
            fakeCam.transform.rotation = drivingCar.cams.transform.GetChild(passNumber).rotation;
            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
            ServerSend.IsPassenger(car, this);

    }

    public void StartDriving(int _id){
        controller car;
        if(GameManager.instance.cars.TryGetValue(_id, out car)){
            if(!car.HasSeat()){
                return;
            }

            col.enabled = false;
            rb.useGravity = false;
            drivingCar = car;
            drivingCar.StartEngine();
            drivingCar.StartDriving();
            passNumber = drivingCar.AddPassenger(this);
            isDriving = true;
            canMove = false;
            this.transform.position = drivingCar.transform.position;
            fakeCam.transform.position = drivingCar.cams.transform.GetChild(passNumber).position;
            fakeCam.transform.rotation = drivingCar.cams.transform.GetChild(passNumber).rotation;
            ServerSend.PlayerPosition(this);
            ServerSend.PlayerRotation(this);
            ServerSend.IsPassenger(car, this);
        }
    }

    public void StopDriving(){
        col.enabled = true;
        rb.useGravity = true;
        this.gameObject.transform.position = drivingCar.kickout.transform.position;
        fakeCam.transform.localPosition = Vector3.zero;
        drivingCar.StopEngine();
        drivingCar.StopDriving();
        drivingCar.RemovePassenger(this);
        ServerSend.IsPassenger(drivingCar, this);
        drivingCar = null;
        isDriving = false;
        canMove = true;
    }

    //Processes player input and moves the player
    public void FixedUpdate()
    {
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0].inpBool)
        {
            _inputDirection.y += 100;
        }
        if (inputs[1].inpBool)
        {
            _inputDirection.y -= 100;
        }
        if (inputs[2].inpBool)
        {
            _inputDirection.x -= 100;
        }
        if (inputs[3].inpBool)
        {
            _inputDirection.x += 100;
        }

        if(isDriving && passNumber == 0){
            Drive();
        }else if(!isDriving){
            Move(_inputDirection);
            Look(inputs[6].inpFloat, inputs[7].inpFloat);
        }
        if(isDriving){
            this.transform.position = drivingCar.transform.position;
            this.transform.rotation = drivingCar.transform.rotation;
            //CarLook(inputs[6].inpFloat, inputs[7].inpFloat);
        }

        if(holdingObject){
            RaycastHit hit;
            if(Physics.Raycast(fakeCam.transform.position,fakeCam.transform.forward,out hit,3f)){
                controller cr = hit.transform.GetComponent<controller>();
                if(cr != null)
                {
                    //TODO: Update Client Cursor
                    if(inputs[5].inpBool){
                        Object obj = heldObject.GetComponent<Object>();
                        if(cr.AddPart(obj.type,heldObject.transform.position,heldObject.transform.rotation.eulerAngles)){
                            obj.DestroyMe();
                            heldObject = null;
                            holdingObject = false;
                        }
                        held = true;
                    }
                }
            }
        }

        if(inputs[5].inpBool && !held){
            if(!isDriving){
                if(heldObject != null && holdingObject){
                    heldObject.transform.GetComponent<MeshCollider>().enabled = true;
                    heldObject.transform.GetComponent<Rigidbody>().isKinematic = false;
                    heldObject.layer = LayerMask.NameToLayer("Default");
                    heldObject.transform.parent = null;
                    holdingObject = false;
                    heldObject = null;
                    return;
                }
                RaycastHit hitInfo;
                if(Physics.Raycast(fakeCam.transform.position,fakeCam.transform.forward,out hitInfo,3f)){
                    if(hitInfo.transform.GetComponent<controller>() != null){
                        if(isDriving == false && holdingObject == false){
                           this.StartDriving(hitInfo.transform.GetComponent<controller>().identifier);
                        }
                    }
                    if(hitInfo.transform.GetComponent<Object>() != null){
                        if(heldObject == null && holdingObject == false){
                            hitInfo.transform.GetComponent<MeshCollider>().enabled = false;
                            hitInfo.transform.GetComponent<Rigidbody>().isKinematic = true;
                            hitInfo.transform.parent = fakeCam.transform;
                            holdingObject = true;
                            heldObject = hitInfo.transform.gameObject;
                            heldObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }
                    }
                    if(hitInfo.transform.tag == "ServiceTag"){
                        switch (hitInfo.transform.name)
                        {
                            case "RepairTag":
                                hitInfo.transform.parent.parent.GetComponent<CarShop>().RepairAll();
                                break;
                            case "RefuelTag":
                                hitInfo.transform.parent.parent.GetComponent<CarShop>().RefuelAll();
                                break;
                            case "CarrierTag":
                                hitInfo.transform.parent.parent.GetComponent<CarShop>().AddBagCarrier();
                                break;
                            case "HolderTag":
                                hitInfo.transform.parent.parent.GetComponent<CarShop>().AddLogHolder();
                                break;
                            default:
                                break;
                        }
                    }
                 }

            }else{
                this.StopDriving();
            }
        }else
        {
            held = false;
        }
        inputs = new Inputs[8]{
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs()
        };

    }

    private void Move(Vector2 _inputDirection){
        if (Physics.Raycast(this.transform.position,-this.transform.up * 1.1f,1f)) {
	        Vector3 targetVelocity = new Vector3(_inputDirection.x, 0, _inputDirection.y);
	        targetVelocity = transform.TransformDirection(targetVelocity);
	        targetVelocity *= moveSpeed;
 
	        Vector3 velocity = rb.velocity;
	        Vector3 velocityChange = (targetVelocity - velocity);
	        velocityChange.y = 0;
	        rb.AddForce(velocityChange, ForceMode.VelocityChange);
 
	        // Jump
	        if (inputs[4].inpBool) {
	            rb.velocity = new Vector3(velocity.x, Mathf.Sqrt(2 * jumpSpeed * -gravity) * 100, velocity.z);
	        }
	    }
 
	    rb.AddForce(new Vector3 (0, gravity * rb.mass, 0));
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
 	}

    public void Look(float _mouseVertical, float _mouseHorizontal){
        //float _mouseVertical = -Input.GetAxis("Mouse Y");
        //float _mouseHorizontal = Input.GetAxis("Mouse X");
        float clampAngle = 85f;

        verticalRotation += _mouseVertical * Time.deltaTime * 100f;
        horizontalRotation += _mouseHorizontal * Time.deltaTime * 100f;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        fakeCam.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        this.gameObject.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        ServerSend.PlayerRotation(this);
    }

    public void CarLook(float _mouseVertical, float _mouseHorizontal){
        float clampAngle = 45f;

        verticalRotation += _mouseVertical * Time.deltaTime * 100f;
        horizontalRotation += _mouseHorizontal * Time.deltaTime * 100f;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        fakeCam.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        this.gameObject.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    private void LateUpdate() {
        if(!isDriving){
            return;
        }
        if(inputs[4].inpBool){
            carCam = !carCam;
        }
        if(carCam){
            fakeCam.transform.position = drivingCar.cams.transform.GetChild(drivingCar.car.passNum).position;
            fakeCam.transform.rotation = drivingCar.cams.transform.GetChild(drivingCar.car.passNum).rotation;
        }else{
            fakeCam.transform.position = drivingCar.cams.transform.GetChild(passNumber).position;
            fakeCam.transform.rotation = drivingCar.cams.transform.GetChild(passNumber).rotation;
        }
        ServerSend.PlayerPosition(this);
        ServerSend.PlayerRotation(this);
    }

    public void Drive(){
        drivingCar.SetInputs(inputs);
    }

    /// <summary>Updates the player input with newly received input.</summary>
    /// <param name="_inputs">The new key inputs.</param>
    /// <param name="_rotation">The new rotation.</param>
    public void SetInput(Inputs[] _inputs)
    {
        inputs = _inputs;
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
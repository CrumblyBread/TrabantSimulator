using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class controller : MonoBehaviour
{

    public bool isDriven = false;
    internal enum driveType{
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField]private driveType drive = (driveType)0;


    [HideInInspector]public bool test; //engine sound boolean

    [Header("Variables")]
    public float handBrakeFrictionMultiplier = 2f;
    public float maxRPM , minRPM;
    public float[] gears;
    public float[] gearChangeSpeed;
    public AnimationCurve enginePower;

    public int gearNum = 0;
    [HideInInspector]public bool playPauseSmoke = false,hasFinished;
    [HideInInspector]public float KPH;
    public float engineRPM;
    [HideInInspector]public bool reverse = false;
    [HideInInspector]public float nitrusValue;
    [HideInInspector]public bool nitrusFlag =false;


    public GameObject wheelMeshes,wheelColliders;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    public GameObject centerOfMass;
    private new Rigidbody rigidbody;

    //car Shop Values
    public int carPrice ;
    public string carName;
    private float smoothTime = 0.09f;

    public bool handbrake;


	private WheelFrictionCurve  forwardFriction,sidewaysFriction;
    private float radius = 6, brakPower = 0, DownForceValue = 10f,wheelsRPM ,driftFactor, lastValue ,horizontal , vertical,totalPower;
    private bool flag=false;




    private void Awake() {

        if(SceneManager.GetActiveScene().name == "awakeScene")return;
        getObjects();
        StartCoroutine(timedLoop());

    }

    private void Update() {

        if(SceneManager.GetActiveScene().name == "awakeScene")return;

        if(!isDriven)return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if(Input.GetKeyDown(KeyCode.E)){
            gearNum++;
            if(gearNum > gears.Length-1){
                gearNum = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.Q)){
            gearNum--;
            if(gearNum < 0){
                gearNum = gears.Length-1;
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            handbrake = !handbrake;
        }

        lastValue = engineRPM;


        addDownForce();
        animateWheels();
        steerVehicle();
        calculateEnginePower();
        if(gameObject.tag == "AI")return;
        adjustTraction();
        Debug.Log(totalPower);
    }

    private void calculateEnginePower(){

        wheelRPM();
            if (vertical != 0 ){
                rigidbody.drag = 0.005f; 
            }
            if (vertical == 0){
                rigidbody.drag = 0.1f;
            }
            if(gearNum == 0){
                totalPower = 0;
            }else{
                totalPower = 7.8f * enginePower.Evaluate(engineRPM) * (vertical);
            }
        


        float velocity  = 0.0f;
        if (engineRPM >= maxRPM || flag ){
            engineRPM = Mathf.SmoothDamp(engineRPM, maxRPM - 500, ref velocity, 0.05f);

            flag = (engineRPM >= maxRPM - 450)?  true : false;
            test = (lastValue > engineRPM) ? true : false;
        }
        else {
            engineRPM = Mathf.SmoothDamp(engineRPM,minRPM + (Mathf.Abs(wheelsRPM) * 7.8f * gears[gearNum]), ref velocity , smoothTime);
            test = false;
        }
        if (engineRPM >= maxRPM + 1000) engineRPM = maxRPM + 1000; // clamp at max
        moveVehicle();
    }

    private void wheelRPM(){
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;
    }

    private bool checkGears(){
        if(KPH >= gearChangeSpeed[gearNum] ) return true;
        else return false;
    }
 
    private bool isGrounded(){
        if(wheels[0].isGrounded &&wheels[1].isGrounded &&wheels[2].isGrounded &&wheels[3].isGrounded )
            return true;
        else
            return false;
    }

    private void moveVehicle(){

        brakeVehicle();

        if(gearNum == gears.Length - 1){
            totalPower = -totalPower;
        }

        if (drive == driveType.allWheelDrive){
            for (int i = 0; i < wheels.Length; i++){
                wheels[i].motorTorque = totalPower / 4;
                wheels[i].brakeTorque = brakPower;
            }
        }else if(drive == driveType.rearWheelDrive){
            wheels[2].motorTorque = totalPower / 2;
            wheels[3].motorTorque = totalPower / 2;

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakPower;
            }
        }
        else{
            wheels[0].motorTorque = totalPower / 2;
            wheels[1].motorTorque = totalPower / 2;

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakPower;
            }
        }

        KPH = rigidbody.velocity.magnitude * 3.6f;


    }

    private void brakeVehicle(){

        if(handbrake){
            brakPower = 2000;
            return;
        }

        if (vertical < 0){
            brakPower =(KPH >= 10)? 500 : 0;
        }
        else if (vertical == 0 &&(KPH <= 10 || KPH >= -10)){
            brakPower = 30;
        }
        else{
            brakPower = 0;
        }


    }
  
    private void steerVehicle(){


        //acerman steering formula
		//steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
         
        if (horizontal > 0 ) {
				//rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
        } else if (horizontal < 0 ) {                                                          
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
			//transform.Rotate(Vector3.up * steerHelping);

        } else {
            wheels[0].steerAngle =0;
            wheels[1].steerAngle =0;
        }

    }

    private void animateWheels ()
	{
		Vector3 wheelPosition = Vector3.zero;
		Quaternion wheelRotation = Quaternion.identity;

		for (int i = 0; i < 4; i++) {
			wheels [i].GetWorldPose (out wheelPosition, out wheelRotation);
			wheelMesh [i].transform.position = wheelPosition;
			wheelMesh [i].transform.rotation = wheelRotation;
		}
	}
   
    private void getObjects(){
        //else aIcontroller = GetComponent<AIcontroller>();
        rigidbody = GetComponent<Rigidbody>();
        wheels[0] = wheelColliders.transform.GetChild(0).GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.GetChild(1).GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.GetChild(2).GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.GetChild(3).GetComponent<WheelCollider>();

        wheelMesh[0] = wheelMeshes.transform.GetChild(0).gameObject;
        wheelMesh[1] = wheelMeshes.transform.GetChild(1).gameObject;
        wheelMesh[2] = wheelMeshes.transform.GetChild(2).gameObject;
        wheelMesh[3] = wheelMeshes.transform.GetChild(3).gameObject;

        rigidbody.centerOfMass = centerOfMass.transform.localPosition;   
    }

    private void addDownForce(){

        rigidbody.AddForce(-transform.up * DownForceValue * rigidbody.velocity.magnitude );

    }

    private void adjustTraction(){
            //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

            for (int i = 0; i < 4; i++) {
                wheels [i].brakeTorque = brakPower;
            }

            //checks the amount of slip to control the drift
		for(int i = 2;i<4 ;i++){

            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);
                //smoke
            if(wheelHit.sidewaysSlip >= 0.3f || wheelHit.sidewaysSlip <= -0.3f ||wheelHit.forwardSlip >= .3f || wheelHit.forwardSlip <= -0.3f)
                playPauseSmoke = true;
            else
                playPauseSmoke = false;
                        

			if(wheelHit.sidewaysSlip < 0 )	driftFactor = (1 + -horizontal) * Mathf.Abs(wheelHit.sidewaysSlip) ;

			if(wheelHit.sidewaysSlip > 0 )	driftFactor = (1 + horizontal )* Mathf.Abs(wheelHit.sidewaysSlip );
		}	
		
	}

	private IEnumerator timedLoop(){
		while(true){
			yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;
            
		}
	}

    
}

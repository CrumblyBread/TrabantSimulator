using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    public Car car;
    public int identifier;

    public bool isDriven = false;
    public bool engineRunning = false;
    public int gearNum = 0;
    public float KPH;
    public float engineRPM;

    public int damageLevel;
    public GameObject myGo;
    public GameObject cams;
    public Player owner;
    public GameObject kickout;
    public Player[] passengers;
    public bool[] parts;
    public GameObject[] upgrades;
    public GameObject[] helpers;
    public int grade;
    public bool loaded;

    public Player driver;
    public GameObject wheelColliders;
    private WheelCollider[] wheels;
    public GameObject centerOfMass;
    private new Rigidbody rigidbody;
    public Inputs[] inputs;
    public bool handbrake;
    [HideInInspector] public bool toBeRemoved;

    private float radius = 6, brakPower = 0,wheelsRPM , lastValue ,horizontal , vertical,totalPower;




    private void Awake() {
        wheels = new WheelCollider[car.wheelsNum];
        myGo = this.gameObject;
        getObjects();
        StartCoroutine(timedLoop());
        inputs = new Inputs[6]{
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs()
        };
        parts = new bool[16];
        for (int i = 0; i < parts.Length; i++)  { parts[i] = false; }
    }

    public void SetId(int _id){
        identifier = _id;
        passengers = new Player[car.passNum];
        car.currentHealth = car.maxHealth;
    }

    private void Update() {

        animateWheels();

        lastValue = engineRPM;

        if(isDriven){
            ConvertInputs();
        }else{
            horizontal = 0;
            vertical = 0;
        }
        steerVehicle();
        calculateEnginePower();

        if(toBeRemoved){
            for (int i = 0; i < passengers.Length; i++)
            {
                if(passengers[i] != null){
                    return;
                }
            }
            GameManager.instance.RemoveCar(this);
        }

        inputs = new Inputs[6]{
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs(),
            new Inputs()
        };
    }

    public void StartEngine(){
        engineRunning = true;
    }

    public void StopEngine(){
        engineRunning = false;
    }

    public void Shift(int _shiftNum){
        gearNum = _shiftNum;
    }

    public void ShiftUp(){
        gearNum++;
        if(gearNum > car.gears.Length-1){
            gearNum = 0;
        }
    }

    public void ShiftDown(){
        gearNum--;
        if(gearNum < 0){
            gearNum = car.gears.Length-1;
        }
    }

    public void Handbrake(){
        handbrake = !handbrake;
    }

    public void Handbrake(bool _brake){
        handbrake = _brake;
    }
    
    public void SetInputs(Inputs[] _inputs){
        this.inputs = _inputs;
    }
    public void ConvertInputs(){
        horizontal = 0;
        vertical = 0;
        if(inputs[0].inpBool){
            vertical += 1;
        }
        if(inputs[1].inpBool){
            vertical -= 1;
        }
        if(inputs[2].inpBool){
            horizontal -= 1;
        }
        if(inputs[3].inpBool){
            horizontal += 1;
        }
    }

    public void StartDriving(){
        isDriven = true;
    }

    public void StopDriving(){
        isDriven = true;
    }

    public bool HasSeat(){
        int val = -1;
        int i = 0;
        while(i < passengers.Length && val == -1)
        {
            if(passengers[i] == null){
                val = i;
            }
            i++;
        }

        Debug.Log(val);

        switch (val)
        {
            case -1: 
                return false;
            case 0:
                return parts[10];
            case 1:
                return parts[11];
            case 2-100:
                return parts[12];
        }
        return false;
    }

    public int AddPassenger(Player _player){
        for (int i = 0; i < car.passNum; i++)
        {
            if(passengers[i] == null){
                passengers[i] = _player;
                if(i == 0 && parts[10]){
                    StartDriving();
                    driver = _player;
                }
                return i;
            }
        }
        return -1;
    }


    public void RemovePassenger(Player _player){
        if(driver == _player){
            driver = null;
        }
        passengers[_player.passNumber] = null;
        _player.passNumber = -1;
    }

    void EvaluateDamage(){
        float step = car.maxHealth/5;
        
        if(car.currentHealth > step * 4){
            damageLevel = 0;
        }else if(car.currentHealth > step * 3){
            damageLevel = 1;
        }else if(car.currentHealth > step * 2){
            damageLevel = 2;
        }else if(car.currentHealth > step){
            damageLevel = 3;
        }else if(car.currentHealth > 0){
            damageLevel = 4;
            BoxCollider bx = this.GetComponent<BoxCollider>();
            bx.size = new Vector3(0f,1.2f,0.06f);
            bx.center = new Vector3(2.89f, 1.36f, 6.67f);
        }else if(car.currentHealth < 0){
            damageLevel = 4; //Death (car death)
        }
    }

    public void Damage(float _value){
        this.car.currentHealth -= _value;
        if(car.currentHealth < 0){
            car.currentHealth = 0;
        }
        EvaluateDamage();
    }
    public void Repair(float _value){
        this.car.currentHealth += _value;
        if(car.currentHealth > car.maxHealth){
            car.currentHealth = car.maxHealth;
        }
        EvaluateDamage();
    }

    private void calculateEnginePower(){

        if(parts[5] && parts[6] && parts[7] && parts[8] && parts[9]){

        wheelRPM();

        if (vertical != 0 ){
            rigidbody.drag = 0.005f; 
        }
        if (vertical == 0){
            rigidbody.drag = 0.1f;
        }

        float velocity  = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM,car.minRPM + (Mathf.Abs(wheelsRPM) * 7.8f * car.gears[gearNum]), ref velocity , car.smoothTime);
        Mathf.Clamp(engineRPM, car.minRPM, car.maxRPM);

        totalPower = 7.8f * car.enginePower.Evaluate(engineRPM) * (vertical);

        moveVehicle();

        }
    }

    private void wheelRPM(){
        float sum = 0;
        int R = 0;
        for (int i = 0; i < car.wheelsNum; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;
    }
 
    private bool isGrounded(){
        if(wheels[0].isGrounded &&wheels[1].isGrounded &&wheels[2].isGrounded && wheels[3].isGrounded )
            return true;
        else
            return false;
    }

    private void moveVehicle(){

        if(gearNum == 0 || !engineRunning){
            totalPower = 0;
        }
        if(gearNum == car.gears.Length - 1){
            if(totalPower < 0f){
                totalPower = 0f;
                brakPower = 500f;
            }
            else{
                totalPower = -totalPower;
            }
        }else{
            if(totalPower < 0){
                totalPower = 0;
                brakPower = 500;
            }else
            {
                brakPower = 0;
            }
        }
        if(handbrake){
            brakPower = 2000;
        }
        if (car.drive == driveType.allWheelDrive){
            for (int i = 0; i < wheels.Length; i++){
                wheels[i].motorTorque = totalPower / car.wheelsNum;
                wheels[i].brakeTorque = brakPower;
            }
        }else if(car.drive == driveType.rearWheelDrive){
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

        KPH = rigidbody.velocity.magnitude;


    }

    private void brakeVehicle(){

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

    public bool CanSteer(){
        for (int i = 0; i < wheels.Length; i++)
        {
            if(!wheels[i].enabled){
                return false;
            }
        }

        if(!parts[4] || !parts[13]){
            return false;
        }

        return true;
    }
  
    private void steerVehicle(){


        if(CanSteer()){

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

    }

    private void animateWheels ()
	{
		Vector3 wheelPosition = Vector3.zero;
		Quaternion wheelRotation = Quaternion.identity;
        Vector3[] poss = new Vector3[car.wheelsNum+1];
        Quaternion[] rots = new Quaternion[car.wheelsNum+1];
        poss[0] = this.gameObject.transform.position;
        rots[0] = this.gameObject.transform.rotation;
		for (int i = 0; i < car.wheelsNum; i++) {
			wheels [i].GetWorldPose (out wheelPosition, out wheelRotation);
			poss [i+1] = wheelPosition;
			rots [i+1] = wheelRotation;
		}
        ServerSend.UpdateCarWheels(this ,poss, rots);

	}

    public bool AddPart(int partNum,Vector3 pos, Vector3 rot){
        
        //Check if this is a trabant
        if(partNum > 17 && (car.type != 1 || car.type != 2)){
            return false;
        }
        float dis;    
        
        switch (partNum)
        {              
            //If the part is a rusty trabant wheel
            case 1:
                for (int i = 0; i < wheels.Length; i++)
                {
                    dis = Vector3.Distance(wheels[i].transform.position,pos);
                    if(!wheels[i].enabled && dis <= 0.3f){
                        wheels[i].enabled = true;
                        ServerSend.updateCars(this,2,i);
                        return true;
                    }
                }
                return false;

            //If the part is a rusty door and a roof is attatched
            case 2:
                dis = Vector3.Distance(helpers[3].transform.position,pos);
                if (!parts[0] && parts[3] && dis <= 1)
                {
                    parts[0] = true;
                    ServerSend.updateCars(this,2,4);
                    return true;
                }
                return false;

            //If the part is a rusty hood
            case 3:
                dis = Vector3.Distance(helpers[2].transform.position,pos);
                if (!parts[1] && dis <= 1f)
                {
                    parts[1] = true;
                    ServerSend.updateCars(this,2,5);
                    return true;
                }
                return false;
            
            //If the part is a rusty trunk
            case 4:
                dis = Vector3.Distance(helpers[4].transform.position,pos);
                if (!parts[2] && dis <= 0.4f)
                {
                    parts[2] = true;
                    ServerSend.updateCars(this,2,6);
                    return true;
                }
                return false;
            
            //If the part is a rusty roof
            case 5:
                dis = Vector3.Distance(helpers[3].transform.position,pos);
                if (!parts[3] && dis <= 1.5f)
                {
                    parts[3] = true;
                    ServerSend.updateCars(this,2,7);
                    return true;
                }
                return false;
            
            //If the part is a rusty steering column
            case 6:
                dis = Vector3.Distance(helpers[2].transform.position,pos);
                if (!parts[4] && dis <= 0.5f)
                {
                    parts[4] = true;
                    ServerSend.updateCars(this,2,8);
                    return true;
                }
                return false;

            //If the part is a rusty spark plug
            case 7:
                for (int i = 0; i < 2; i++)
                {
                    dis = Vector3.Distance(helpers[i].transform.position,pos);
                    if(!helpers[i].activeSelf && dis <= 0.1f && parts[9] && parts[8]){
                        parts[5+i] = true;
                        ServerSend.updateCars(this,2,9+i);
                        return true;
                    }
                }    
                return false;
            
            //If the part is a rusty oil filter
            case 8:
                dis = Vector3.Distance(helpers[2].transform.position,pos);
                if (!parts[7] && parts[8] && parts[9] && dis >= 0.3f)
                {
                    parts[7] = true;
                    ServerSend.updateCars(this,2,11);
                    return true;
                }
                return false;

            //If the part is a rusty engine block
            case 9:
                dis = Vector3.Distance(helpers[2].transform.position,pos);
                if (!parts[8] && dis >= 0.3f)
                {
                    parts[8] = true;
                    ServerSend.updateCars(this,2,12);
                    return true;
                }
                return false;

            //If the part is a rusty engine head
            case 10:
                dis = Vector3.Distance(helpers[2].transform.position,pos);
                if (!parts[9] && parts[8] && dis >= 0.3f)
                {
                    parts[9] = true;
                    ServerSend.updateCars(this,2,13);
                    return true;
                }
                return false;

            //If the part is a rusty drivers seat
            case 11:
                dis = Vector3.Distance(helpers[3].transform.position,pos);
                if (!parts[10] && dis >= 1f)
                {
                    parts[10] = true;
                    ServerSend.updateCars(this,2,14);
                    return true;
                }
                return false;

            //If the part is a rusty passenger seat
            case 12:
                dis = Vector3.Distance(helpers[3].transform.position,pos);
                if (!parts[11] && dis >= 1f)
                {
                    parts[11] = true;
                    ServerSend.updateCars(this,2,15);
                    return true;
                }
                return false;

            //If the part is a rusty back seat
            case 13:
                dis = Vector3.Distance(helpers[3].transform.position,pos);
                if (!parts[12] && dis >= 1.5f)
                {
                    parts[12] = true;
                    ServerSend.updateCars(this,2,16);
                    return true;
                }
                return false;

            //If the part is a rusty fuel tank
            case 14:
                dis = Vector3.Distance(helpers[4].transform.position,pos);
                if (!parts[13] && dis >= 2f)
                {
                    parts[13] = true;
                    ServerSend.updateCars(this,2,17);
                    return true;
                }
                return false;

            //If the part is a rusty battery
            case 15:
                dis = Vector3.Distance(helpers[2].transform.position,pos);
                if (!parts[14] && dis >= 1.5f)
                {
                    parts[14] = true;
                    ServerSend.updateCars(this,2,18);
                    return true;
                }
                return false;

            //If the part is a rusty steering wheel
            case 16:
                dis = Vector3.Distance(helpers[3].transform.position,pos);
                if (!parts[15] && dis >= 1.5f)
                {
                    parts[15] = true;
                    ServerSend.updateCars(this,2,19);
                    return true;
                }
                return false;

            default:
                return false;
        }
    }
   
    private void getObjects(){
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;
        for (int i = 0; i < car.wheelsNum; i++)
        {
            wheels[i] = wheelColliders.transform.GetChild(i).GetComponent<WheelCollider>();
        }  
    }

    private void OnCollisionEnter(Collision col){
        Vector3 impact = col.relativeVelocity * Mathf.Abs (Vector3.Dot (col.relativeVelocity.normalized, col.contacts[0].normal));
        float impactSpeed = Mathf.Abs(impact.x + impact.y + impact.z);
        if(impactSpeed > 5 && col.collider.transform.tag != "Player"){
            Damage(impactSpeed);
        } 
    }

	private IEnumerator timedLoop(){
		while(true){
			yield return new WaitForSeconds(.7f);
            radius = 6 + KPH / 20;
            
		}
	}

    
}

public enum driveType{
    frontWheelDrive,
    rearWheelDrive,
    allWheelDrive
}

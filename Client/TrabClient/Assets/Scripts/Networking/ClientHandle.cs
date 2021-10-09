using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {_msg}");
        Client.instance.myId = _myId;
        UIManager.instance.timeIt = false;
        Cursor.visible = false;
        UIManager.instance.loadingScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        ClientSend.WelcomeReceived();


        // Now that we have the client's id, connect UDP
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Vector3 _camPos = _packet.ReadVector3();

        GameManager.players[_id].transform.position = _position;
        Vector3 wantedPosition;
        wantedPosition = Vector3.Lerp(GameManager.players[_id].myCam.transform.position, _camPos, 0.8f);
        GameManager.players[_id].myCam.transform.position = wantedPosition;
    }

    public static void PlayerRotation(Packet _packet)
    {   
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _camRot = _packet.ReadQuaternion();

        GameManager.players[_id].transform.rotation = _rotation;
        Quaternion wantedRotation;
        wantedRotation = Quaternion.Lerp(GameManager.players[_id].myCam.transform.rotation, _camRot, 0.8f);
        GameManager.players[_id].myCam.transform.rotation = wantedRotation;
    }

    public static void SendInfoToPlayer(Packet _packet)
    {
        string code = _packet.ReadString();
        GameManager.SetRoomCode(code);
    }

    public static void SpawnCar(Packet _packet){
        int _id = _packet.ReadInt();
        int _type = _packet.ReadInt();
        float _health = _packet.ReadFloat();
        int _wheels = _packet.ReadInt();
        int _pass = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        GameManager.instance.SpawnCar(_id,_type,_health,_wheels,_pass,_pos,_rot);
    }

    public static void UpdateCarWheels(Packet _packet){
        int _id = _packet.ReadInt();
        int _wheelNum = _packet.ReadInt();
        Vector3[] _poses = new Vector3[_wheelNum];
        Quaternion[] _rotes = new Quaternion[_wheelNum];
        for (int i = 0; i < _wheelNum; i++)
        {
            _poses[i] = _packet.ReadVector3();
            _rotes[i] = _packet.ReadQuaternion();
        }
        int damageLevel = _packet.ReadInt();
        Car c;
        if(GameManager.cars.TryGetValue(_id, out c)){
            GameManager.cars[_id].UpdateCarWheels(_poses,_rotes);
            GameManager.cars[_id].Damage(damageLevel);
        }
    }

    public static void IsPassenger(Packet _packet){
        int playerId = _packet.ReadInt();
        int carId = _packet.ReadInt();
        int passNumber = _packet.ReadInt();

        GameManager.players[playerId].IsPassenger(carId,passNumber);
    }

    public static void UpdateCars(Packet _packet){
        int carId = _packet.ReadInt();
        int level = _packet.ReadInt();

        switch (level)
        {
            case 0:
                GameManager.cars[carId].Upgrade(_packet.ReadInt());
                break;
            case 1:
                GameManager.cars[carId].Load(_packet.ReadInt());
                break;  
            case 2:
                GameManager.cars[carId].AddPart(_packet.ReadInt());
                break; 
            default:
                break;
        }
    }

    public static void SpawnObject(Packet _packet){
        int _id = _packet.ReadInt();
        int _type = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        GameManager.instance.SpawnObject(_id,_type,_pos,_rot);
    }

    public static void UpdateObject(Packet _packet){
        int _id = _packet.ReadInt();
        GameManager.objects[_id].UpdateObj(_packet.ReadVector3(),_packet.ReadQuaternion());
        if(_packet.ReadBool()){
            Destroy(GameManager.objects[_id].gameObject);
        }
    }

    public static void Quest(Packet _packet){
        int questId = _packet.ReadInt();
        switch (questId)
        {
            case 1:
                QuestManager.instance.MechanicQuestStage(_packet.ReadInt());
                break;
            case 2:
                QuestManager.instance.LumberJackQuestStage(_packet.ReadInt());
                break;
            default:
                break;
        }
    }
}
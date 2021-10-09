using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();
        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        Inputs[] _inputs = new Inputs[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadInputs();
        }

        Server.clients[_fromClient].player.SetInput(_inputs);
    }

    public static void CarShift(int _fromClient, Packet _packet){
        int carId = _packet.ReadInt();

        if(GameManager.instance.cars[carId].driver.id != _fromClient){
            return;
        }

        int shiftChange = _packet.ReadInt();
        switch(shiftChange){
            case 0:
                GameManager.instance.cars[carId].Shift(_packet.ReadInt());
                break;
            case 1:
                GameManager.instance.cars[carId].ShiftUp();
                break;
            case 2:
                GameManager.instance.cars[carId].ShiftDown();
                break;
        }
    }
}
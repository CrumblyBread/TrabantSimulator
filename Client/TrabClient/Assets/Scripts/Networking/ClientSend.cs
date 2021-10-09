using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to the server via UDP.</summary>
    /// <param name="_packet">The packet to send to the sever.</param>
    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region Packets
    /// <summary>Lets the server know that the welcome message was received.</summary>
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(UIManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    /// <summary>Sends player input to the server.</summary>
    /// <param name="_inputs"></param>
    public static void PlayerMovement(Inputs[] _inputs)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (Inputs _input in _inputs)
            {
                _packet.Write(_input);
            }

            SendUDPData(_packet);
        }
    }

    public static void CarShift(Car car, int shiftChange, int shiftNum)
    {
        using (Packet _packet = new Packet((int)ClientPackets.carShift))
        {
            _packet.Write(car.identifier);
            _packet.Write(shiftChange);
            if(shiftChange == 0){
                _packet.Write(shiftNum);
            }

            SendTCPData(_packet);
        }
    }
    public static void CarShift(int carId, int shiftChange, int shiftNum)
    {
        using (Packet _packet = new Packet((int)ClientPackets.carShift))
        {
            _packet.Write(carId);
            _packet.Write(shiftChange);
            if(shiftChange == 0){
                _packet.Write(shiftNum);
            }

            SendTCPData(_packet);
        }
    }
    public static void CarShift(int carId, int shiftChange)
    {
        if(shiftChange == 0 || carId == 0){
            return;
        }
        using (Packet _packet = new Packet((int)ClientPackets.carShift))
        {
            _packet.Write(carId);
            _packet.Write(shiftChange);

            SendTCPData(_packet);
        }
    }
    #endregion
}
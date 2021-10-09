using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    /// <summary>Sends a packet to a client via UDP.</summary>
    /// <param name="_toClient">The client to send the packet the packet to.</param>
    /// <param name="_packet">The packet to send to the client.</param>
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via TCP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }

    /// <summary>Sends a packet to all clients via UDP.</summary>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }
    /// <summary>Sends a packet to all clients except one via UDP.</summary>
    /// <param name="_exceptClient">The client to NOT send the data to.</param>
    /// <param name="_packet">The packet to send.</param>
    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }

    #region Packets
    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="_toClient">The client to send the packet to.</param>
    /// <param name="_msg">The message to send.</param>
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            Debug.Log("WelcomeSent");
            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="_toClient">The client that should spawn the player.</param>
    /// <param name="_player">The player to spawn.</param>
    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
        }
    }

    /// <summary>Sends a player's updated position to all clients.</summary>
    /// <param name="_player">The player whose position to update.</param>
    public static void PlayerPosition(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.fakeCam.transform.position);

            SendUDPDataToAll(_packet);
        }
    }

    /// <summary>Sends a player's updated rotation to all clients except to himself (to avoid overwriting the local player's rotation).</summary>
    /// <param name="_player">The player whose rotation to update.</param>
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.fakeCam.transform.rotation);

            SendUDPDataToAll(_packet);
        }
    }
    public static void UpdateCarWheels(controller car ,Vector3[] positions, Quaternion[] rotations){
        using (Packet _packet = new Packet((int)ServerPackets.updateCarWheels))
        {
            _packet.Write(car.identifier);
            _packet.Write(car.car.wheelsNum + 1);
            for (int i = 0; i < positions.Length; i++)
            {
                _packet.Write(positions[i]);
                _packet.Write(rotations[i]);   
            }
            _packet.Write(car.damageLevel);

            SendUDPDataToAll(_packet);
        }
    }
    public static void SpawnCar(controller car ,Vector3 position, Quaternion rotation, Player player){
        using (Packet _packet = new Packet((int)ServerPackets.spawnCar))
        {
            _packet.Write(car.identifier);
            _packet.Write(car.car.type);
            _packet.Write(car.car.maxHealth);
            _packet.Write(car.car.wheelsNum);
            _packet.Write(car.car.passNum);
            _packet.Write(position);
            _packet.Write(rotation);

            SendTCPData(player.id,_packet);
        }
    }

    public static void SpawnObject(Object obj ,Vector3 position, Quaternion rotation, Player player){
        using (Packet _packet = new Packet((int)ServerPackets.spawnObject))
        {
            _packet.Write(obj.identifier);
            _packet.Write(obj.type);
            _packet.Write(position);
            _packet.Write(rotation);

            SendTCPData(player.id,_packet);
        }
    }

    public static void UpdateObject(Object obj ,Vector3 position, Quaternion rotation,bool destroy = false){
        using (Packet _packet = new Packet((int)ServerPackets.updateObject))
        {
            _packet.Write(obj.identifier);
            _packet.Write(position);
            _packet.Write(rotation);
            _packet.Write(destroy);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SendInfoToPlayer(Player _player, string _roomCode)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendInfoToPlayer))
        {
            _packet.Write(_roomCode);

            SendTCPData(_player.id, _packet);
        }
    }

    public static void updateCars(controller _car, int _level, int _value)
    {
        using (Packet _packet = new Packet((int)ServerPackets.updateCars))
        {
            _packet.Write(_car.identifier);
            _packet.Write(_level);
            _packet.Write(_value);

            SendTCPDataToAll(_packet);
        }
    }

    public static void IsPassenger(controller car, Player player){
        using (Packet _packet = new Packet((int)ServerPackets.isPassanger))
        {
            _packet.Write(player.id);
            _packet.Write(car.identifier);
            _packet.Write(player.passNumber);

            SendTCPDataToAll(_packet);
        }
    }

    public static void Quest(int _id, int _stage){
        using (Packet _packet = new Packet((int)ServerPackets.quest))
        {
            _packet.Write(_id);
            _packet.Write(_stage);

            SendTCPDataToAll(_packet);
        }
    }
    #endregion
}
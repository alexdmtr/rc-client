using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using Socket_Helper;
using System.Runtime.Serialization.Formatters.Binary;

public static class GameNetworking
{
    public static object SendAndReceive(object o)
    {
        TcpClient client = new TcpClient("188.24.183.189", 8001);
        SocketHelper.SendObject(client.GetStream(), o);
        return SocketHelper.ReceiveObject(client.GetStream());
    }
    public static void SendMessage(object message)
    {
        TcpClient client = new TcpClient("188.24.183.189", 8001);
        SocketHelper.SendObject(client.GetStream(), message);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// 客户端测试类
/// </summary>
public class ClientTestClass : NetworkDataAdapter
{
    public ClientTestClass()
    {
        NetworkClient client = new NetworkClient(this);
        client.Launch(
            NetworkConfig.Instance().
            AddressFamily(AddressFamily.InterNetwork).
            SocketType(SocketType.Stream).
            Protocol(ProtocolType.Tcp).
            IP(NetworkConfig.GetIPAdress).
            Port(8324).
            TimeoutSend(1000).
            TimeoutRecive(1000)
        );
    }

    public void OnReciveString(string content)
    {
        Console.WriteLine((++seek) + ". client recive:" + content);
    }

    public void Client_BeforeDestroy(NetworkClient server)
    {

    }

    public void OnSended(byte[] sended)
    {

    }

    public void Client_Launch(NetworkClient client)
    {
        Log("客户端启动:" + client.clientSocket.LocalEndPoint.ToString());
    }

    public byte[] dataTransfer(byte[] reciveData)
    {
        return NetWorkCenter.S2B("client");
    }

    public void Log(string msg)
    {
        Console.WriteLine("Log:" + msg);
    }

    public void Server_BeforeDestroy(NetworkServer server)
    {

    }

    public void Server_Launch(NetworkServer server)
    {

    }

    public void Server_OnClientConnected(Socket client)
    {
        Console.WriteLine("Client_Launch:" + client.RemoteEndPoint.ToString());
    }

    public void Server_OnClientDisconnected(NetworkReciver reciver)
    {
    }

    public void Server_OnClientReciverCreated(NetworkReciver reciver)
    {
    }

    public void Server_OnClientReciverRemoved(NetworkReciver reciver)
    {
    }
    public int seek = 0;
}

/// <summary>
/// 服务器测试类
/// </summary>
public class ServerTestClass : NetworkDataAdapter
{
    public int seek = 0;
    public ServerTestClass()
    {
        NetworkServer server = new NetworkServer(this);
        server.Launch(
            NetworkConfig.Instance().
            AddressFamily(AddressFamily.InterNetwork).
            SocketType(SocketType.Stream).
            Protocol(ProtocolType.Tcp).
            IP(NetworkConfig.GetIPAdress).
            Port(8324).
            TimeoutSend(1000).
            TimeoutRecive(1000)
        );
    }

    public void OnReciveString(string content)
    {
        //Console.Clear();
        Console.WriteLine((++seek) + ". server recive:" + content);
    }

    public void Client_BeforeDestroy(NetworkClient server)
    {

    }

    public void OnSended(byte[] sended)
    {

    }

    public void Client_Launch(NetworkClient client)
    {

    }

    public byte[] dataTransfer(byte[] reciveData)
    {
        return NetWorkCenter.S2B("server");
    }

    public void Log(string msg)
    {
        Console.WriteLine("Log:" + msg);
    }

    public void Server_BeforeDestroy(NetworkServer server)
    {

    }

    public void Server_Launch(NetworkServer server)
    {
        Console.WriteLine("开启服务器:" + server.sReception.LocalEndPoint.ToString());
    }

    public void Server_OnClientConnected(Socket client)
    {
        Console.WriteLine("Server_OnClientConnected:" + client.RemoteEndPoint.ToString());
    }

    public void Server_OnClientDisconnected(NetworkReciver reciver)
    {
        Console.WriteLine("Server_OnClientDisconnected:" + reciver.rSocketInstance.RemoteEndPoint.ToString());
    }

    public void Server_OnClientReciverCreated(NetworkReciver reciver)
    {
        Console.WriteLine("连接服务器:" + reciver.connectionID + ":" + reciver.rSocketInstance.RemoteEndPoint.ToString());
    }

    public void Server_OnClientReciverRemoved(NetworkReciver reciver)
    {
        Console.WriteLine("Server_OnClientReciverRemoved:" + reciver.rSocketInstance.RemoteEndPoint.ToString());
    }
}

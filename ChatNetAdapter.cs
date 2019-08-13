using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
class ChatNetAdapterServer : NetworkDataAdapter
{
    public NetworkServer netServer;
    public ChatManager serverChatManager;
    //public ChatNetAdapter chatNetAdapter;

    public ChatNetAdapterServer()
    {
        netServer = new NetworkServer(this);
        serverChatManager = new ChatManager("server");

        InstanceState state = netServer.Launch(
                NetworkConfig.Instance().
                AddressFamily(AddressFamily.InterNetwork).
                SocketType(SocketType.Stream).
                Protocol(ProtocolType.Tcp).
                IP(NetworkConfig.GetIPAdress).
                Port(8324).
                TimeoutSend(100000).
                TimeoutRecive(100000).
                BackLog(100)
            ).state;
        Console.WriteLine((state == InstanceState.launch) ? "启动成功,本机IP:"+netServer.sReception.LocalEndPoint.ToString() : "启动失败");

        //chatNetAdapter = new ChatNetAdapter();
    }
    public void Client_BeforeDestroy(NetworkClient server)
    {

    }

    public void Client_Launch(NetworkClient client)
    {

    }

    public void OnSended(byte[] sended)
    {
        string json = Encoding.UTF8.GetString(sended);
        MsgsTransport transport = BaseData.Instance<MsgsTransport>(json);
        serverChatManager.OnSendFinished(transport.id);
    }

    public byte[] dataTransfer(byte[] reciveData)
    {
        if (reciveData == null)
        {
            return null;
        }
        string json = Encoding.UTF8.GetString(reciveData);
        MsgsTransport trans = BaseData.Instance<MsgsTransport>(json);
       
        serverChatManager.OnReciveMsg(trans);
        return serverChatManager.GetSendMsgs(true, trans.msglist[0].user).ToBytes();
    }

    public void Log(string msg)
    {

    }

    public void OnReciveString(string content)
    {
        //Console.WriteLine(content);
    }

    public void Server_BeforeDestroy(NetworkServer server)
    {

    }

    public void Server_Launch(NetworkServer server)
    {
        //Console.WriteLine(ChatManager.TimeStamp);
    }

    public void Server_OnClientConnected(Socket client)
    {
        Console.WriteLine("客户加入:"+client.RemoteEndPoint.ToString());
    }

    public void Server_OnClientDisconnected(NetworkReciver reciver)
    {
        Console.WriteLine("客户离开:" + reciver.rSocketInstance.RemoteEndPoint.ToString());
    }

    public void Server_OnClientReciverCreated(NetworkReciver reciver)
    {

    }

    public void Server_OnClientReciverRemoved(NetworkReciver reciver)
    {

    }
}

class ChatNetAdapter : NetworkDataAdapter
{
    public NetworkClient netClient;
    public ChatManager chatManager;
    
    public ChatNetAdapter()
    {
        netClient = new NetworkClient(this);
        chatManager = new ChatManager(NetworkConfig.GetIPAdress);
        InstanceState state = netClient.Launch(
                NetworkConfig.Instance().
                AddressFamily(AddressFamily.InterNetwork).
                SocketType(SocketType.Stream).
                Protocol(ProtocolType.Tcp).
                IP(NetworkConfig.GetIPAdress).
                Port(8324).
                TimeoutSend(1000).
                TimeoutRecive(1000)
            ).state;
        if(netClient.state == InstanceState.launch)
            chatManager.user = netClient.clientSocket.LocalEndPoint.ToString();
        Console.WriteLine((state == InstanceState.launch)?"启动成功":"启动失败");
    }
    public void Client_BeforeDestroy(NetworkClient server)
    {
        Console.WriteLine("客户端关闭");
    }

    public void OnSended(byte[] sended)
    {
        //Console.WriteLine("On sended");
        string json = Encoding.UTF8.GetString(sended);
        MsgsTransport transport = BaseData.Instance<MsgsTransport>(json);
        chatManager.OnSendFinished(transport.id);
    }

    public void Client_Launch(NetworkClient client)
    {

    }

    public byte[] dataTransfer(byte[] reciveData)
    {
        string json = Encoding.UTF8.GetString(reciveData);
        chatManager.OnReciveMsg(BaseData.Instance<MsgsTransport>(json));
        MsgsTransport send = chatManager.GetSendMsgs();
        return send.ToBytes();
    }

    public void Log(string msg)
    {
        Console.WriteLine(msg);
    }

    public void OnReciveString(string content)
    {
       // Console.WriteLine(content);
    }

    public void Server_BeforeDestroy(NetworkServer server)
    {
    }

    public void Server_Launch(NetworkServer server)
    {
    }

    public void Server_OnClientConnected(Socket client)
    {
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
}
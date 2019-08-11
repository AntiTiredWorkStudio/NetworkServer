using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.IO;
using System.Linq;
using System;
/// <summary>
/// 网络中心控制器
/// </summary>
public static class NetWorkCenter{
    public static T StartInstance<T>(NetworkDataAdapter dAdapter) where T:NetworkInstance
    {
        T instance = new NetworkInstance(dAdapter) as T;
        return instance;
    }

    /// <summary>
    /// 文本转数据
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static byte[] S2B(string content)
    {
        return Encoding.UTF8.GetBytes(content);
    }

    /// <summary>
    /// 数据转文本
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string B2S(byte[] data)
    {
        return Encoding.UTF8.GetString(data);
    }
}
/// <summary>
/// 配置类
/// </summary>
public class NetworkConfig
{
    public static string GetIPAdress
    {
        get
        {
           string name = Dns.GetHostName();
           IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
           foreach (IPAddress ipa in ipadrlist)
           {
                if (ipa.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                    return ipa.ToString();
                }
           }
           return "0.0.0.0";
        }
    }

    private NetworkConfig() { }
    public static NetworkConfig Instance()
    {
        return new NetworkConfig();
    }
    public NetworkConfig AddressFamily(AddressFamily _afamily)
    {
        afamily = _afamily;
        return this;
    }
    public AddressFamily afamily;
    public NetworkConfig SocketType(SocketType _stype)
    {
        stype = _stype;
        return this;
    }
    public SocketType stype;
    public NetworkConfig Protocol(ProtocolType _ptype)
    {
        ptype = _ptype;
        return this;
    }
    public ProtocolType ptype;
    public NetworkConfig IP(string _ipAddress)
    {
        ipAddress = _ipAddress;
        return this;
    }
    public string ipAddress;
    public NetworkConfig Port(int _port)
    {
        port = _port;
        return this;
    }
    public int port;
    public NetworkConfig BackLog(int _backlog)
    {
        backlog = _backlog;
        return this;
    }
    public int backlog;
    public NetworkConfig TimeoutRecive(int _reciveTimeOut)
    {
        reciveTimeOut = _reciveTimeOut;
        return this;
    }
    public int reciveTimeOut;
    public NetworkConfig TimeoutSend(int _sendTimeOut)
    {
        sendTimeOut = _sendTimeOut;
        return this;
    }
    public int sendTimeOut;
}
/// <summary>
/// 网络模块状态定义
/// </summary>
public enum InstanceState
{
    sleep,
    launch,
    Reciving,
    Recived,
    Sending,
    Sended,
}
/// <summary>
/// 网络对象实例
/// </summary>
public class NetworkInstance
{
    /// <summary>
    /// 尝试发送
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="datas"></param>
    /// <returns></returns>
    public bool TrySendDataBySocket(Socket sender,byte[] datas)
    {
        if (sender.Poll(10, SelectMode.SelectRead))
        {
            try
            {
                throw new Exception("sender not read");
            }catch(Exception e)
            {
                dataAdapter.Log(e.ToString());
            }
            return false;
        }
        sender.Send(datas);
        dataAdapter.OnSended(datas);
        return true;
    }

    public byte[] TryReciveDataFromSocket(Socket reciver)
    {
        if (reciver.Poll(10, SelectMode.SelectRead))
        {
            try
            {
                throw new Exception("reciver not read");
            }
            catch (Exception e)
            {
               // dataAdapter.Log(e.ToString());
                //File.WriteAllText(System.Environment.CurrentDirectory + "/error_recive.txt", e.ToString());
            }
            return new byte[0];
        }
        if(reciver == null)
        {
            return null;
        }
        byte[] reciverDatas = new byte[1024*256];
        try
        {
            int length = reciver.Receive(reciverDatas);
            List<byte> content = new List<byte>();
            content.AddRange(reciverDatas.Take(length));
            reciverDatas = content.ToArray();
            if (reciverDatas == null)
            {
                return null;
            }
            //ArraySegment<byte> segment = new ArraySegment<byte>(reciverDatas, 0, length);
            return reciverDatas;
        }
        catch(Exception e)
        {
           // dataAdapter.Log(e.ToString());
            return null;
        }
    }

    public NetworkConfig dataConfig;
    public NetworkDataAdapter dataAdapter;
    public InstanceState state;
    protected Thread MainThread = null;

    public virtual void DestroyInstance()
    {
        state = InstanceState.sleep;
        if (MainThread != null)
        {
            try{MainThread.Abort();}catch(Exception e) { /*dataAdapter.Log(e.ToString());*/ }
        }
    }

    public virtual NetworkInstance Launch(NetworkConfig config){state = InstanceState.launch; dataConfig = config; return this; }

    public NetworkInstance(NetworkDataAdapter dAdapter)
    {
        dataAdapter = dAdapter;
    }

    public void Setting()
    {

    }

    public T To<T>() where T :NetworkInstance
    {
        return this as T;
    }
}
/// <summary>
/// 网络适配器接口
/// </summary>
public interface NetworkDataAdapter
{
    /// <summary>
    /// 数据转换反馈,收到数据需反序列化,反馈到的数据需序列化
    /// </summary>
    /// <param name="reciveData">接收到的数据</param>
    /// <returns>需要反馈给服务器的数据</returns>
    byte[] dataTransfer(byte[] reciveData);
    /// <summary>
    /// 当收到的数据可被转换为字符串时
    /// </summary>
    /// <param name="content">转为字符串</param>
    void OnReciveString(string content);

    /// <summary>
    /// 发送成功时回调
    /// </summary>
    void OnSended(byte[] sended);

    /// <summary>
    /// 当服务器启动时
    /// </summary>
    /// <param name="server">返回服务器对象</param>
    void Server_Launch(NetworkServer server);
    /// <summary>
    /// 当检测到客户端连接时
    /// </summary>
    /// <param name="client">返回客户端socket</param>
    void Server_OnClientConnected(Socket client);
    /// <summary>
    /// 当客户端的接收器(reciver)被创建时
    /// </summary>
    /// <param name="reciver">返回接收器</param>
    void Server_OnClientReciverCreated(NetworkReciver reciver);
    /// <summary>
    /// 当接收器被移除时
    /// </summary>
    /// <param name="reciver">返回接收器</param>
    void Server_OnClientReciverRemoved(NetworkReciver reciver);
    /// <summary>
    /// 当客户端断开
    /// </summary>
    /// <param name="reciver">返回接收器</param>
    void Server_OnClientDisconnected(NetworkReciver reciver);
    /// <summary>
    /// 当服务器关闭前
    /// </summary>
    /// <param name="server">返回服务器对象</param>
    void Server_BeforeDestroy(NetworkServer server);

    /// <summary>
    /// 当客户端启动时
    /// </summary>
    /// <param name="client">返回客户端对象</param>
    void Client_Launch(NetworkClient client);
    /// <summary>
    /// 当客户端销毁前
    /// </summary>
    /// <param name="server">返回客户端对象</param>
    void Client_BeforeDestroy(NetworkClient server);
    /// <summary>
    /// 消息输出
    /// </summary>
    /// <param name="msg">返回消息</param>
    void Log(string msg);
}
/// <summary>
/// 服务器线程池
/// </summary>
public class NetworkServerSocketPool
{
    public Dictionary<string, NetworkReciver> reciverList;
    NetworkServer targetServer;
    public NetworkServerSocketPool(NetworkServer server)
    {
        reciverList = new Dictionary<string, NetworkReciver>();
        targetServer = server;
    }

    public int ReciverCount
    {
        get
        {
            return reciverList.Count;
        }
    }

    public NetworkReciver[] Recivers
    {
        get
        {
            return (new List<NetworkReciver>(reciverList.Values)).ToArray();
        }
    }

    public static string GenerateID()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString()+ Convert.ToInt64(ts.TotalMilliseconds).ToString();
    }

    public void AddSocket(Socket clientSocket)
    {
        string connectionID = GenerateID();
        while (reciverList.ContainsKey(connectionID))
        {
            connectionID = (int.Parse(connectionID)+1).ToString();
        }
        NetworkReciver tReciver = new NetworkReciver(connectionID, this, clientSocket, targetServer.dataAdapter);
        reciverList.Add(connectionID, tReciver);
        targetServer.dataAdapter.Server_OnClientReciverCreated(tReciver);
    }

    public void RemoveSocket(string id)
    {
        if (reciverList.ContainsKey(id))
        {
            NetworkReciver finishedReciver = reciverList[id];
            targetServer.dataAdapter.Server_OnClientReciverRemoved(finishedReciver);
            reciverList.Remove(id);
            targetServer.dataAdapter.Log("keys count:"+reciverList.Keys.Count.ToString());
            //File.WriteAllText(System.Environment.CurrentDirectory + "/remove.txt", reciverList.Count.ToString());
            if (finishedReciver.state != InstanceState.sleep)
            {
                finishedReciver.DestroyInstance();
            }
        }
        else
        {
            throw new Exception("no id:"+id);
        }
    }

    public void Clear()
    {
        foreach(string key in reciverList.Keys)
        {
            reciverList[key].DestroyInstance();
        }
        reciverList.Clear();
    }
}

/// <summary>
/// 服务端=>侦测器
/// </summary>
public class NetworkServer:NetworkInstance
{
    public Socket sReception = null;

    public NetworkServerSocketPool socketPool;

    public NetworkServer(NetworkConfig config, NetworkDataAdapter dAdapter):base(dAdapter)
    {
        Launch(config);
    }

    public NetworkServer(NetworkDataAdapter dAdapter) : base(dAdapter) { }

    public override NetworkInstance Launch(NetworkConfig config)
    {
        sReception = new Socket(config.afamily, config.stype, config.ptype);
        IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Parse(config.ipAddress), config.port);
        try
        {
            sReception.Bind(targetEndPoint);
        }
        catch (Exception e)
        {
            dataAdapter.Log(e.ToString());
            sReception = null;
            return null;
        }
        sReception.Listen(config.backlog);
        NetworkInstance current = base.Launch(config);
        MainThread = new Thread(ConnectReception);
        MainThread.Start();
        socketPool = new NetworkServerSocketPool(this);
        dataAdapter.Server_Launch(this);
        return current;
    }

    public void ConnectReception()
    {
        while(state != InstanceState.sleep)
        {
            Socket client = sReception.Accept();
            
            socketPool.AddSocket(client);
            dataAdapter.Server_OnClientConnected(client);
        }
    }

    public string CheckReciver()
    {
        return socketPool.ReciverCount.ToString();
    }


    public override void DestroyInstance()
    {
        dataAdapter.Server_BeforeDestroy(this);
        socketPool.Clear();
        sReception.Close();
        base.DestroyInstance();
    }
}

/// <summary>
/// 服务端=>接收器
/// </summary>
public class  NetworkReciver:NetworkInstance
{
    NetworkServerSocketPool parentPool;
    public Socket rSocketInstance = null;
    public string connectionID;

    public NetworkReciver(string id,NetworkServerSocketPool tPool, Socket rSocket,NetworkDataAdapter dAdapter) : base(dAdapter)
    {
        connectionID = id;
        parentPool = tPool;
        rSocketInstance = rSocket;
        MainThread = new Thread(ReciveAndSend);
        MainThread.Start();
        rSocketInstance.SendTimeout = 1000;
        rSocketInstance.ReceiveTimeout = 1000;
    }

    protected void ReciveAndSend()
    {
        while (!rSocketInstance.Poll(10, SelectMode.SelectRead))
        {
            byte[] buffer = null;
            // rSocketInstance = rSocketInstance.Accept();
            state = InstanceState.Reciving;
            buffer = TryReciveDataFromSocket(rSocketInstance);
            if(buffer == null)
            {
                continue;
            }

            state = InstanceState.Recived;
            try
            {
                string tContent = Encoding.UTF8.GetString(buffer);
                if (tContent != null)
                {
                    dataAdapter.OnReciveString(tContent);
                }
            }
            catch { }
            byte[] senddata = dataAdapter.dataTransfer(buffer);
            state = InstanceState.Sending;
            if (!TrySendDataBySocket(rSocketInstance, senddata))
            {
                ReleaseSelf();
                break;
            }
            state = InstanceState.Sended;
        }
        ReleaseSelf();
    }

    public override void DestroyInstance()
    {
        rSocketInstance.Close();
        base.DestroyInstance();
    }

    public void ReleaseSelf()
    {
        dataAdapter.Server_OnClientDisconnected(this);
        parentPool.RemoveSocket(connectionID);
        DestroyInstance();
    }
}

/// <summary>
/// 客户端
/// </summary>
public class NetworkClient : NetworkInstance
{
    public Socket clientSocket = null;
    public string reciveDatas = "enter";
    public NetworkClient(NetworkConfig config, NetworkDataAdapter dAdapter) : base(dAdapter)
    {
        Launch(config);
    }

    public NetworkClient(NetworkDataAdapter dAdapter) : base(dAdapter) { }

    public override NetworkInstance Launch(NetworkConfig config)
    {
        //设定服务器IP地址  
        IPEndPoint targetEndPoint = new IPEndPoint(IPAddress.Parse(config.ipAddress), config.port);
        clientSocket = new Socket(config.afamily, config.stype, config.ptype);
        try
        {
            clientSocket.Connect(targetEndPoint); //配置服务器IP与端口  
        }catch(Exception e)
        {
            dataAdapter.Log(e.ToString());
            return this;
        }
        MainThread = new Thread(SendAndRecive);
        MainThread.Start();
        clientSocket.SendTimeout = config.sendTimeOut;
        clientSocket.ReceiveTimeout = config.reciveTimeOut;
        dataAdapter.Client_Launch(this);
        return base.Launch(config);
    }

    public void SendAndRecive()
    {
        Thread.Sleep(1000);//延迟1秒进行长链，等待服务器线程创建完毕
        while (!clientSocket.Poll(10,SelectMode.SelectRead))
        {
            try
            {
                byte[] senddata = dataAdapter.dataTransfer(Encoding.UTF8.GetBytes(reciveDatas));
               // byte[] buffer = Encoding.UTF8.GetBytes(NetworkServerSocketPool.GenerateID() + "-" + clientSocket.LocalEndPoint.ToString());
                state = InstanceState.Sending;
                if (!TrySendDataBySocket(clientSocket, senddata))
                {
                    continue;
                }
                state = InstanceState.Sended;

                state = InstanceState.Reciving;
                byte[] reciverBuffer = TryReciveDataFromSocket(clientSocket);

                if(reciveDatas == null || reciveDatas.Length<=0)
                {
                    dataAdapter.Log("recive data is null");
                    continue;
                }
                state = InstanceState.Recived;
                try
                {
                    reciveDatas = Encoding.UTF8.GetString(reciverBuffer);
                }catch(Exception e)
                {
                    continue;
                }
                if (reciveDatas != null)
                {
                    dataAdapter.OnReciveString(reciveDatas);
                }
                Thread.Sleep(100);
            }catch(Exception e)
            {
                dataAdapter.Log(e.ToString());
                //File.WriteAllText(System.Environment.CurrentDirectory + "/error_Client.txt", e.ToString());
            }
        }
        DestroyInstance();
    }

    public override void DestroyInstance()
    {
        dataAdapter.Client_BeforeDestroy(this);
        clientSocket.Close();
        base.DestroyInstance();
    }
}
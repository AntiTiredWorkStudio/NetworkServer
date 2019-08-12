using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ChatManager
{
    public static Int64 TimeStamp
    {
        get
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }
    }

    public string user;
    Dictionary<Int64, MsgData> msgList;
    public Dictionary<Int64, MsgData> MsgList
    {
        get
        {
            if (msgList == null) {
                msgList = new Dictionary<Int64, MsgData>();
            }
            return msgList;
        }
    }
        
    public ChatManager(string _user) {
        user = _user;
        sendTimeLine = 0;
    }

    public string ChatMsg(string msg) {
       MsgData mdata = MsgData.Instance<MsgData>().Set(TimeStamp,msg,user).Called("msg").As<MsgData>();
       MsgList.Add(mdata.timestamp, mdata);
       ShowMsg();
       return msg;
    }


    /// <summary>
    /// 显示消息
    /// </summary>
    public void ShowMsg()
    {
        //Console.Clear();
        foreach (Int64 key in MsgList.Keys)
        {
            if (MsgList[key].user != user)
            {
                Console.WriteLine("\t\t\t\t\t" + MsgList[key].user + ": " + MsgList[key].msg);
            }else
            Console.WriteLine(MsgList[key].user + ": " + MsgList[key].msg);
        }
        //Console.WriteLine(GetSendMsgs().ToString());
    }

    /// <summary>
    /// 收到的消息的时间范围
    /// </summary>
    /// <returns></returns>
    public Int64 GetReciveTimeSeek()
    {
        List<Int64> timeArray = new List<long>();
        foreach (Int64 key in MsgList.Keys)
        {
            if (MsgList[key].user != user)
            {
                timeArray.Add(key);
            }
        }
        if (timeArray.Count == 0)
        {
            return 0;
        }
        return timeArray.Max();
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    /// <param name="msgTrans"></param>
    public void OnReciveMsg(MsgsTransport msgTrans) {
        if (msgTrans.IsNull) {
            return;
        }
        foreach (MsgData data in msgTrans.msglist)
        {
            if(data.msg == "nan")
            {
                continue;
            }
            if (MsgList.ContainsKey(data.timestamp)) {
                Console.WriteLine("键值重复:"+data.timestamp);
                throw new Exception("简直重复");
                continue;
            }
            MsgList.Add(data.timestamp, data);
        }
        MsgList.OrderBy(x => x.Key);
        ShowMsg();
    }

    /// <summary>
    /// 消息运输
    /// </summary>
    Dictionary<string, MsgsTransport> msgtransportList;
    public Dictionary<string, MsgsTransport> MsgTransportList
    {
        get
        {
            if (msgtransportList == null)
            {
                msgtransportList = new Dictionary<string, MsgsTransport>();
            }
            return msgtransportList;
        }
    }

    /// <summary>
    /// 消息发送时间线
    /// </summary>
    public Int64 sendTimeLine;
    /// <summary>
    /// 发送消息方法
    /// </summary>
    /// <param name="isServer">是否为服务器</param>
    /// <param name="reciverUser">服务器根据用户创建</param>
    /// <returns></returns>
    public MsgsTransport GetSendMsgs(bool isServer = false, string reciverUser = "")
    {
        List<MsgData> sendDatas = new List<MsgData>(MsgList.Values.Where(x => x.timestamp > sendTimeLine && ((!isServer && x.user == user) || (isServer && x.user != reciverUser))));
        Int64 currentTime = ChatManager.TimeStamp;
        if (sendDatas.Count == 0)
        {
            sendDatas.Add(BaseData.Instance<MsgData>().Set(currentTime,"nan",user));
        }
        MsgsTransport transport = BaseData.Instance<MsgsTransport>().Set(sendDatas, currentTime).Called("trans_" + ChatManager.TimeStamp.ToString()).As<MsgsTransport>();
        MsgTransportList.Add(transport.id, transport);
        return transport;
    }

    /// <summary>
    /// 发送成功回调
    /// </summary>
    /// <param name="transportKey"></param>
    public void OnSendFinished(string transportKey)
    {
        if (!MsgTransportList.ContainsKey(transportKey))
        {
            return;
        }
        sendTimeLine = MsgTransportList[transportKey].timestamp;
    }
}

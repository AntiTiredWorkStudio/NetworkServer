using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;
/// <summary>
/// 数据中心
/// </summary>
public class DataCenter
{
    public Queue<BaseData> tDatas;
    public DataCenter()
    {
        tDatas = new Queue<BaseData>();
        
    }

    public void PushData<T>(T data) where T:BaseData
    {
        tDatas.Enqueue(data);
    }

    public BaseData GetData()
    {
        if (tDatas.Count == 0)
        {
            return new EmptyData();
        }
        else
        {
            return tDatas.Dequeue();
        }
    }
}


/// <summary>
/// 基本data数据
/// </summary>
public abstract class BaseData
{
    public static BaseData Instance() {
        return new EmptyData();
    }
    public static T Instance<T>() where T:BaseData,new(){
        T ins = new T();
        return ins;
        //return new BaseData() as T;
    }
    public static T Instance<T>(string json) where T:BaseData,new()
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
    public static T Instance<T>(byte[] buff) where T : BaseData, new()
    {
        string json = Encoding.UTF8.GetString(buff);
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <summary>
    /// 数据标题
    /// </summary>
    public string id = "";
    /// <summary>
    /// 数据类型
    /// </summary>
    public string type = "";
    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseData()
    {
        type = GetType().Name;
        id = TypeDefine;
    }
    /// <summary>
    /// 叫做
    /// </summary>
    /// <param name="titleName"></param>
    public BaseData Called(string titleName) {
        id = titleName;
        return this;
    }
    /// <summary>
    /// 转换为byte的方法
    /// </summary>
    /// <returns></returns>
    public byte[] ToBytes()
    {
        string sources = ToString();
        return Encoding.UTF8.GetBytes(sources);
    }
    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
    /// <summary>
    /// id定义
    /// </summary>
    public abstract string IdDefine { get; }
    /// <summary>
    /// 定义类型
    /// </summary>
    public abstract string TypeDefine { get; }
    /// <summary>
    ///  类型转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T As<T>() where T:BaseData{
        return this as T;
    }
    //public abstract BaseData Setting(params object[] pars);
}

/// <summary>
/// 空数据
/// </summary>
public class EmptyData : BaseData
{
    public string text = "empty data";
    public EmptyData():base()
    {
        id = "empty";
        text = "empty data";
    }
    public override string TypeDefine
    {
        get { return "empty"; }
    }

    public override string IdDefine
    {
        get { return "empty"; }
    }
}



public class Vector3Data : BaseData {
    public float x;
    public float y;
    public float z;
    public Vector3Data():base()
    {
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
    }
    public override string TypeDefine
    {
        get { return "vector3"; }
    }
    /// <summary>
    /// 设置
    /// </summary>
    /// <param name="_x"></param>
    /// <param name="_y"></param>
    /// <param name="_z"></param>
    /// <returns></returns>
    public Vector3Data Set(float _x,float _y,float _z) {
        x = _x;
        y = _y;
        z = _z;
        return this;
    }

    public override string IdDefine
    {
        get { return "vector"; }
    }
}

/*
/// <summary>
/// 数据包承载容器
/// </summary>
public class MsgsTransport<T> : BaseData
{
    public Int64 timestamp = 0;
    public List<T> msglist;
    public override string IdDefine
    {
        get { return "msgs"; }
    }

    public override string TypeDefine
    {
        get { return "msgs"; }
    }
    public MsgsTransport Set(List<T> msgs, Int64 _timeStamp)
    {
        msglist = msgs;
        timestamp = _timeStamp;
        return this;
    }

    public bool IsNull
    {
        get
        {
            return msglist == null || msglist.Count == 0;
        }
    }

    public MsgsTransport<T> Join(params T[] msgs)
    {
        foreach (T data in msgs)
        {
            msglist.Add(data);
        }
        return this<T>;
    }
}*/



/// <summary>
/// 收到的消息列表
/// </summary>
public class MsgsTransport : BaseData
{
    public Int64 timestamp = 0;
    public List<MsgData> msglist;
    public override string IdDefine
    {
        get { return "msgs"; }
    }

    public override string TypeDefine
    {
        get { return "msgs"; }
    }
    public MsgsTransport Set(List<MsgData> msgs,Int64 _timeStamp)
    {
        msglist = msgs;
        timestamp = _timeStamp;
        return this;
    }
    public MsgsTransport Set(Int64 _timeStamp)
    {
        if (msglist == null)
        {
            msglist = new List<MsgData>();
        }
        timestamp = _timeStamp;
        return this;
    }
    

    public bool IsNull {
        get
        {
            return msglist == null || msglist.Count == 0;
        }
    }

    public MsgsTransport Join(params MsgData[] msgs) {
        if (msglist == null) {
            msglist = new List<MsgData>();
        }
        foreach (MsgData data in msgs)
        {
            msglist.Add(data);
        }
        return this;
    }
}

/// <summary>
/// 发送的消息
/// </summary>
public class MsgData : BaseData
{
    public Int64 timestamp = 0;
    public string msg = "";
    public string user = "";
    public override string TypeDefine
    {
        get { return "msg"; }
    }
    /// <summary>
    /// 设置参数
    /// </summary>
    /// <param name="_time"></param>
    /// <param name="_msg"></param>
    /// <param name="_user"></param>
    public MsgData Set(Int64 _time, string _msg, string _user)
    {
        timestamp = _time;
        msg = _msg;
        user = _user;
        return this;
    }

    public override string IdDefine
    {
        get { return "msg"; }
    }
}
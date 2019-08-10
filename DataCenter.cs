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
/// 子数据模板
/// </summary>
public class SubData
{
    public string id;
    public string type;
    public string tsub;
    public string tsubObject {
        get
        {
            return "{" + "\"sublist\":" + tsub + "}";
        }
    }
}
public class SubDataList
{
    public SubData[] sublist;
}
/// <summary>
/// 基本data数据
/// </summary>
public class BaseData
{
    public static BaseData Instance() {
        return new BaseData();
    }
    public static T Instance<T>() where T:BaseData,new(){
        T ins = new T();
        return ins;
        //return new BaseData() as T;
    }

    public static BaseData Instance(string json)
    {
        SubData subData = JsonConvert.DeserializeObject<SubData>(json);
        //Console.WriteLine(subData.tsubObject);

        SubDataList subDataList = JsonConvert.DeserializeObject<SubDataList>(subData.tsubObject);
        
        Console.WriteLine(subDataList.sublist[0].tsubObject);
        return JsonConvert.DeserializeObject<BaseData>(json);
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
    /// 子数据
    /// </summary>
    [NonSerialized]
    public List<BaseData> sub;
    /// <summary>
    /// 子字符
    /// </summary>
    public string tsub {
        get
        {
            return JsonConvert.SerializeObject(sub);
        }
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
    /// 叫做
    /// </summary>
    /// <param name="titleName"></param>
    public BaseData Called(string titleName) {
        id = titleName;
        return this;
    }

    public BaseData BondNew(params BaseData[] targets){
        if (sub == null) { 
            sub = new List<BaseData>();
        }
        foreach (BaseData target in targets)
        {
            sub.Add(target);
        }
        return this;
    }

    public BaseData()
    {
        type = GetType().Name;
        try
        {
            if (sub == null)
            {
                sub = new List<BaseData>();
            }
            id = "base data";
        }
        catch (Exception e)
        {
            Console.WriteLine("error:"+e.ToString());
        }
        
    }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
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

}



public class Vector3Data : BaseData {
    public float x;
    public float y;
    public float z;
    public Vector3Data()
    {
        x = 0.0f;
        y = 0.0f;
        z = 0.0f;
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
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MRServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerTestClass stclass = new ServerTestClass();
            //ClientTestClass clclass = new ClientTestClass();
            while (true)
            {
                string key = Console.ReadLine();
                if(key == "quit")
                {
                    break;
                }
                if(key == "client")
                {
                    new ClientTestClass();
                }
            }
            Console.WriteLine("end");
        }
    }

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
                IP("192.168.1.102").
                Port(8324).
                TimeoutSend(1000).
                TimeoutRecive(1000)
            );
        }

        public void OnReciveString(string content)
        {
            Console.WriteLine("client recive:"+content);
        }

        public void Client_BeforeDestroy(NetworkClient server)
        {

        }

        public void Client_Launch(NetworkClient client)
        {

        }

        public byte[] dataTransfer(byte[] reciveData)
        {
            return null;
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

    public class ServerTestClass : NetworkDataAdapter{
        public ServerTestClass()
        {
            NetworkServer server = new NetworkServer(this);
            server.Launch(
                NetworkConfig.Instance().
                AddressFamily(AddressFamily.InterNetwork).
                SocketType(SocketType.Stream).
                Protocol(ProtocolType.Tcp).
                IP("192.168.1.102").
                Port(8324).
                TimeoutSend(1000).
                TimeoutRecive(1000)
            );
        }

        public void OnReciveString(string content)
        {
            Console.Clear();
            Console.WriteLine("server recive:" + content);
        }

        public void Client_BeforeDestroy(NetworkClient server)
        {

        }

        public void Client_Launch(NetworkClient client)
        {

        }

        public byte[] dataTransfer(byte[] reciveData)
        {
            return null;
        }

        public void Log(string msg)
        {   
            Console.WriteLine("Log:" + msg);
        }

        public void Server_BeforeDestroy(NetworkServer server)
        {
            throw new NotImplementedException();
        }

        public void Server_Launch(NetworkServer server)
        {
            Console.WriteLine("开启服务器:" + server.sReception.LocalEndPoint.ToString());
        }

        public void Server_OnClientConnected(Socket client)
        {
            
        }

        public void Server_OnClientDisconnected(NetworkReciver reciver)
        {
        }

        public void Server_OnClientReciverCreated(NetworkReciver reciver)
        {
            Console.WriteLine("连接服务器:"+reciver.connectionID + ":" + reciver.rSocketInstance.RemoteEndPoint.ToString());
        }

        public void Server_OnClientReciverRemoved(NetworkReciver reciver)
        {
        }
    }
}

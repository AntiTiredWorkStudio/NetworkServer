using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;
namespace MRServer
{
    class Program
    {
        static void OnServer()
        {
            ChatNetAdapterServer NetAdapter = new ChatNetAdapterServer();
            while (NetAdapter.chatNetAdapter.chatManager.ChatMsg(Console.ReadLine()) != "exit")
            {

            }
        }
        static void OnClient()
        {
            ChatNetAdapter NetAdapter = new ChatNetAdapter();
            while (NetAdapter.chatManager.ChatMsg(Console.ReadLine()) != "exit")
            {

            }
        }

        static void Main(string[] args)
        {
             //while (true)
             //{
                 Console.WriteLine("'server'=>作为服务端启动,'client'=>作为服务端启动,'exit'=>退出");
                 switch (Console.ReadLine())
                 {
                     case "server":
                         Console.Clear();
                         OnServer();
                         break;
                     case "client":
                         Console.Clear();
                         OnClient();
                         break;
                     case "exit":
                         System.Environment.Exit(0);
                         return;
                     default:
                         Console.Clear();
                         Console.WriteLine("无此指令");
                         break;
                 }
             //}
    }
    }
}

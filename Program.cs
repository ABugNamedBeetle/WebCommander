// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Net.WebSockets;
using System.Text;

using System.Threading;
using System.Threading.Tasks;
using WebCommander.App;
using WebCommander.App.Controller;

namespace WebCommander
{
    class Program
    {
        
        
        // private const string url="wss://simple-web-socket-abnb.onrender.com/desktop";
        
        // private const string url = "ws://localhost:5000/desktop";
        

        static void Main(string[] args)
        {
            // Console.Title += "[HELLO]";
            Console.WriteLine(Path.GetFullPath("init.json"));
            
            Data init = Data.getDataFromJSON(Path.GetFullPath("init.json"));
           //Console.WriteLine(init);
            
            Thread.Sleep(1000);
            while (true)
            {
                WebSocketCommander.Connect(init).Wait();
                
                Thread.Sleep(5000);
            }
           
          


        }
    }

       
}


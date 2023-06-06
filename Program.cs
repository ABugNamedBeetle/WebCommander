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

namespace WebCommander
{
    class Program
    {
        private static object consoleLock = new object();
        private const int sendChunkSize = 256;
        private const int receiveChunkSize = 64;
        private const bool verbose = true;

        private static bool isLastHealthSent = false;

        // private const string url="wss://simple-web-socket-abnb.onrender.com/desktop";
        
        // private const string url = "ws://localhost:5000/desktop";
        private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(1000);
        private static ClientWebSocket? webSocket = null;
        private static Data? init = null;

        static void Main(string[] args)
        {
            // Console.Title += "[HELLO]";
            Console.WriteLine(Path.GetFullPath("init.json"));
            
            Data init = Data.getDataFromJSON(Path.GetFullPath("init.json"));
           //Console.WriteLine(init);
            
            Thread.Sleep(1000);
            while (true)
            {

                Connect(init.Websocket!).Wait();
                Thread.Sleep(5000);
            }
           
          


        }

       

        public static async Task Connect(string uri)
        {
            

            try
            {

                webSocket = new ClientWebSocket();
                
                webSocket.Options.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36");
                webSocket.Options.SetRequestHeader("Connection", "Upgrade");

                Console.WriteLine("WebSocket Commander : " + uri);
                Console.ForegroundColor = ConsoleColor.White;
                
                Console.Write($"{DateTime.Now.ToShortTimeString()} - Initiating the connection to Websocket: ");
                Console.ResetColor();
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
                 
                if (webSocket.State == WebSocketState.Open)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Connected");
                    Console.ResetColor();
                    Console.WriteLine("Health service started");

                    var t = HealthStatus(webSocket);
                    await Task.WhenAll(Receive(webSocket));
                    if(t.Result == false){
                        throw new Exception("Websocked Failed, Health Service Stopped");
                    }
                }



            }
            catch (Exception ex)
            {
                Console.WriteLine("[ Connect ] Exception: {0}", ex.Message);
            }
            finally
            {
                if (webSocket != null)
                    webSocket.Dispose();
                Console.WriteLine();

                lock (consoleLock)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("WebSocket closed.");
                    Console.ResetColor();


                }

            }
        }

        private static async Task Send(ClientWebSocket webSocket)
        {
            var random = new Random();
            byte[] buffer = new byte[sendChunkSize];

            while (webSocket.State == WebSocketState.Open)
            {
                random.NextBytes(buffer);

                await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
                LogStatus(false, buffer, buffer.Length);

                await Task.Delay(delay);
            }
        }

        private static async Task SendMessage(ClientWebSocket webSocket, String msg)
        {

            if (webSocket.State == WebSocketState.Open)
            {

                var encoded = Encoding.ASCII.GetBytes(msg);
                var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                await webSocket.SendAsync(buffer, WebSocketMessageType.Binary, true, CancellationToken.None);
                // LogStatus(false, encoded, encoded.Length);

                await Task.Delay(delay);
            }
        }

        private static async Task Receive(ClientWebSocket webSocket)
        {
            byte[] buffer = new byte[receiveChunkSize];
            while (webSocket.State == WebSocketState.Open)
            {

                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                }
                else
                {
                    LogStatus(true, buffer, result.Count);
                }
            }
            Console.WriteLine("Receive Service Stopped");
        }

        private static void LogStatus(bool receiving, byte[] buffer, int length)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = receiving ? ConsoleColor.Green : ConsoleColor.Gray;


                if (verbose)
                {

                    String msg = Encoding.ASCII.GetString(buffer, 0, length);


                    if (msg.Equals("-ok-") && isLastHealthSent)
                    {
                        ///System.Diagnostics.Process.Start(@"D:\Arpit Sharma\Downloads\Compressed\remote-tools\start SSH webrtc Tunnel 5022.bat");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("<<<< HEALTH OK");
                        isLastHealthSent = false;  //ack the health sent
                        
                    }
                    else
                    {
                        Console.WriteLine("{0} {1} bytes... ", receiving ? "Received" : "Sent", length);
                        Console.WriteLine(msg);

                        if (msg.Equals("start attack"))
                        {
                            System.Diagnostics.Process.Start(@"D:\Arpit Sharma\Downloads\Compressed\remote-tools\start VNC webrtc Tunnel 5900.bat");
                            Console.WriteLine("💥");
                        }
                    }
                }

                Console.ResetColor();
            }
        }

        /// <summary>
        /// Health Service: return bool false means stopped.
        /// </summary>
        private static async Task<bool> HealthStatus(ClientWebSocket webSocket)
        {
            
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
            int tickCount = 0; 
            int maxTickCount = 6;  //1 min -> 6 ticks of 10 sec each 
            string formattedTime = "0";
            isLastHealthSent = false; //when ever new connection

            
            while (webSocket.State == WebSocketState.Open) 
            {
                if(isLastHealthSent && (tickCount == 0)){ //healt is not ack and tick is 0
                    break;
                }

                if(!isLastHealthSent && (tickCount == 0)){ //LHS is true it is not ack
                    
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($">>>> Sending Health OK [ {DateTime.Now.ToShortTimeString()} ]");
                    isLastHealthSent = true;
                    await SendMessage(webSocket, "-ok-").ContinueWith(prev =>
                    {

                        int minutesToAdd = 1; // Example: add 30 minutes

                        DateTime currentTime = DateTime.Now;
                        DateTime futureTime = currentTime.AddMinutes(minutesToAdd);

                        formattedTime = futureTime.ToString("hh:mm tt");
                        Console.Title = $"Web Commander : : next Health ping at {formattedTime} : : [ {tickCount}/{maxTickCount} ]";
                        
                    });
                }
                

                await timer.WaitForNextTickAsync();
                tickCount = (tickCount == maxTickCount-1) ? 0 : tickCount+1;
                Console.Title = $"Web Commander : : next Health ping at {formattedTime} : : [ {tickCount}/{maxTickCount} ]";


            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Health Service Quited, WebSocked Aborted");
            Console.ResetColor();
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);

                       
            return false;



        }


        // Console.WriteLine("in health");
        // while (webSocket.State == WebSocketState.Open){
        //     Console.WriteLine("in scok open");
        //  while (await timer.WaitForNextTickAsync()){
        //     if (webSocket.State != WebSocketState.Open){
        //         break;
        //     }
        //     Console.WriteLine($"{DateTime.Now} Timer Health -ok- sent");
        //     await SendMessage(webSocket, "-ok-").ContinueWith((prev)=>{


        //     });
        // }
        // }
        // TIMER_WHILE_LOOP:
        // while (true)
        // {
        //     await SendMessage(webSocket, "-ok-");
        //     if(webSocket.State == WebSocketState.Open){
        //         await timer.WaitForNextTickAsync();
        //     }else{
        //         TIME_WHILE_LOOP:break;
        //     }
        // }
    }
}


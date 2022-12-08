using CpMove;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpServerTicTacToe
{
    public class Program
    {
        private static readonly int _port = 8888;
        private static readonly string _ipAddress = "127.0.0.1";

        private static TcpListener _server;
        private static BinaryFormatter _formatter;
        private static NetworkStream _networkStreamClient1;
        private static NetworkStream _networkStreamClient2;
        private static EventWaitHandle _waitHandle;
        private static int[] _field;
        private static int _mark1;
        private static int _mark2;

        private async static Task Main(string[] args)
        {
            _formatter = new BinaryFormatter();
            _waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            _server = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            _mark1 = 1;
            _mark2 = 2;

            _server.Start();

            Task.Run(ListenClient2);
            Task.Delay(500);
            await Task.Run(ListenClient1);
        }

        private static void ListenClient1()
        {
            var client = _server.AcceptTcpClient();
            _networkStreamClient1 = client.GetStream();

            try
            {
                while (true)
                {
                    using (var streamReader = new StreamReader(_networkStreamClient1, Encoding.UTF8))
                    {
                        var currentMove = GetCurrentMove(streamReader, _mark1);
                        _waitHandle.Set();
                        _waitHandle.WaitOne();
                    }
                    SendAnswer(_networkStreamClient1);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.StackTrace}\r\n{ex.Message}");
            }
        }

        private static void ListenClient2()
        {
            var client = _server.AcceptTcpClient();
            _networkStreamClient2 = client.GetStream();
            _waitHandle.WaitOne();
            Move currentMove;

            try
            {
                while (true)
                {
                    using (var streamReader = new StreamReader(_networkStreamClient2, Encoding.UTF8))
                    {
                        currentMove = GetCurrentMove(streamReader, _mark2);
                    }
                    SendAnswer(_networkStreamClient2);
                    _field = currentMove.Field;
                    _waitHandle.Set();
                    _waitHandle.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.StackTrace}\r\n{ex.Message}");
            }
        }

        private static void SendAnswer(NetworkStream networkStream)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    _formatter.Serialize(memoryStream, _field);
                    networkStream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    memoryStream.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private static Move GetCurrentMove(StreamReader streamReader, int mark)
        {
            var currentMove = _formatter.Deserialize(streamReader.BaseStream) as Move;
            int index = currentMove.Field[9];
            _field = currentMove.Field;
            _field[index] = mark;
            return currentMove;
        }
    }
}

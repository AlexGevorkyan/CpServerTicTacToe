using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CpServerTicTacToe
{
    internal class Program
    {
        private const int QUANTITY_OF_CLIENTS = 2;

        private static string _address = "127.0.0.1";
        private static int _port = 8888;
        private static TcpListener _server;
        private static Dictionary<string, string> _logedUsers;//key-nameOfUser, value - chosed character
        private static Semaphore _semaphore;
        private static string[] _characters;

        static async Task Main(string[] args)
        {
            _logedUsers = new Dictionary<string, string>();
            _semaphore = new Semaphore(QUANTITY_OF_CLIENTS, QUANTITY_OF_CLIENTS);
            _server = new TcpListener(IPAddress.Parse(_address), _port);
            _server.Start();

            while (true)
            {
                _semaphore.WaitOne();
                await Task.Run(Listen);
            }
        }

        private static async void Listen()
        {
            var client = _server.AcceptTcpClient();
            var networkStream = client.GetStream();
            try
            {
                while (true)
                {
                    var streamReader = new StreamReader(networkStream, Encoding.UTF8);
                    var data = (new BinaryFormatter()).Deserialize(streamReader.BaseStream);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private static void Answer()
        {

        }
    }
}

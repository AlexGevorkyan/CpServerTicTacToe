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
    internal class Program
    {
        private const int _COUNT_USERS = 2;

        private static int _port = 8888;
        private static string _ipAddress = "127.0.0.1";
        private static TcpListener _server;
        private static int[] _field;
        private static Move _currentMove;
        private static List<string> _users;
        private static BinaryFormatter _formatter;
        private static NetworkStream _networkStream;
        private static Semaphore _semaphore;
        private static int _curentCountUsers = 0;


        private async static Task Main(string[] args)
        {
            _server = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            _semaphore = new Semaphore(_COUNT_USERS, _COUNT_USERS);
            _server.Start();
            await Task.Run(Listen);
        }

        private static void Listen()
        {
            var client = _server.AcceptTcpClient();
            _semaphore.WaitOne();
            _curentCountUsers++;
            WaitForAllConnect();            

            var _networkStream = client.GetStream();

            while (true)
            {
                using (var streamReader = new StreamReader(_networkStream, Encoding.UTF8))
                {
                    _currentMove = _formatter.Deserialize(streamReader.BaseStream) as Move;
                    _field = _currentMove.Field;

                    if (!_users.Contains(_currentMove.PlayerName))
                        _users.Add(_currentMove.PlayerName);
                }
                SendAnswer();
            }
        }

        private static void WaitForAllConnect()
        {
            while (true)
            {
                if (_curentCountUsers == 2)
                    break;
            }
        }

        private static void SendAnswer()
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    _formatter.Serialize(memoryStream, _field);
                    _networkStream.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    memoryStream.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        //private static bool IsGameOver()
        //{

        //    foreach (var value in _field)
        //    {
        //        if (IsWonVertical() || IsWonHorizontal() ||
        //           IsWonDiagonal())
        //            return true;

        //        if (value == 0)
        //            return false;
        //    }
        //    return true;
        //}

        //private static bool IsWonDiagonal()
        //{
        //    return CheckIsWonFirstDiagonal() ||
        //           CheckIsWonSecondDiagonal();
        //}

        //private static bool IsWonVertical()
        //{
        //    return CheckIsWonVertical(0) ||
        //           CheckIsWonVertical(1) ||
        //           CheckIsWonVertical(2);
        //}

        //private static bool IsWonHorizontal()
        //{
        //    return CheckIsWonHorizontal(0) ||
        //           CheckIsWonHorizontal(3) ||
        //           CheckIsWonHorizontal(6);
        //}

        //private static bool CheckIsWonFirstDiagonal()
        //{
        //    return _field[0] == _field[4] &&
        //           _field[4] == _field[8];
        //}

        //private static bool CheckIsWonSecondDiagonal()
        //{
        //    return _field[2] == _field[4] &&
        //           _field[4] == _field[6];
        //}

        //private static bool CheckIsWonHorizontal(int position)
        //{
        //    int character = _field[position];
        //    for (int i = position + 1; i < _field.Length; i++)
        //    {
        //        if (position + 3 > i)
        //            break;

        //        if (character != _field[i])
        //            return false;
        //    }
        //    return true;
        //}

        //private static bool CheckIsWonVertical(int position)
        //{
        //    int character = _field[position];
        //    for (int i = position + 3; i < _field.Length; i += 3)
        //    {
        //        if (character != _field[i])
        //            return false;
        //    }
        //    return true;
        //}
    }
}

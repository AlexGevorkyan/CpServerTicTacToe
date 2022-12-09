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
        private const int _COUNT_USERS = 2;

        private static int _port = 8888;
        private static string _ipAddress = "127.0.0.1";
        private static TcpListener _server;
        private static int[] _field;
        private static Move _currentMove;
        private static List<string> _users;
        private static BinaryFormatter _formatter;
        private static NetworkStream _networkStream1;
        private static NetworkStream _networkStream2;
        private static EventWaitHandle wh;
        private static int _curentCountUsers = 0;
        private static int mark1 = 1, mark2 = 2;

        private async static Task Main(string[] args)
        {
            _server = new TcpListener(IPAddress.Parse(_ipAddress), _port);
            _server.Start();
            wh = new EventWaitHandle(false, EventResetMode.AutoReset);
            _formatter = new BinaryFormatter(); 
            Task.Run(Listen2);
            Task.Delay(500);
            await Task.Run(Listen1);

        }

        private static void Listen1()
        {
            Console.WriteLine("Start listening for first player....");
            var client = _server.AcceptTcpClient();
            Console.WriteLine("First player connected!");
            _networkStream1 = client.GetStream();
            Move currentMove = null;

            while (true)
            {
                //using (var streamReader = new StreamReader(_networkStream1, Encoding.UTF8))
                //{
                    var streamReader = new StreamReader(_networkStream1, Encoding.UTF8);
                    currentMove = _formatter.Deserialize(streamReader.BaseStream) as Move;
                    _field = currentMove.Field;
                    int index = currentMove.Field[9];
                    _field[index] = mark1;
                    wh.Set();
                    wh.WaitOne();
                //}
                SendAnswer1(currentMove);
            }
        }

        private static void Listen2()
        {
            Console.WriteLine("Start listening for second player....");
            var client = _server.AcceptTcpClient();
            _networkStream2 = client.GetStream();
            Console.WriteLine("Second player connected!");
            wh.WaitOne();
            Move currentMove = null;

            while (true)
            {
                //using (var streamReader = new StreamReader(_networkStream2, Encoding.UTF8))
                //{
                    var streamReader = new StreamReader(_networkStream2, Encoding.UTF8);
                    currentMove = _formatter.Deserialize(streamReader.BaseStream) as Move;
                    _field = currentMove.Field;
                    int index = currentMove.Field[9];
                    _field[index] = mark2;
                //}
                SendAnswer2(currentMove);
                _field = currentMove.Field;
                wh.Set();
                wh.WaitOne();
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

        private static void SendAnswer1(Move move)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    move.Field = _field;
                    _formatter.Serialize(memoryStream, move);
                    //memoryStream.Seek(0, SeekOrigin.Begin);
                    _networkStream1.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    _networkStream1.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private static void SendAnswer2(Move move)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    move.Field = _field;
                    _formatter.Serialize(memoryStream, move);
                    //memoryStream.Seek(0, SeekOrigin.Begin);
                    _networkStream2.Write(memoryStream.ToArray(), 0, (int)memoryStream.Length);
                    _networkStream2.Flush();
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

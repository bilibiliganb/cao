using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PPserver.Correspondence;

namespace pp
{
    namespace Correspondence
    {

       public class Socket
        {
            class SocketObject
            {
                public System.Net.Sockets.Socket socketWorker;
                public const int allocate = 1024 * 1024 * 2;
                public byte[] buffer = null;
                public StringBuilder str = new StringBuilder();
                public ManualResetEvent allDone = null;
            }

            private class Entity
            {
                public Command command;
                public DateTime datetime;
            }


            System.Net.Sockets.Socket socketSend;
            string ip;
            string point;
            Entity entity = new Entity();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="socket"></param>
            public Socket(System.Net.Sockets.Socket socket)
            {
                this.socketSend = socket;
                string[] str = socket.RemoteEndPoint.ToString().Split(':');
                ip = str[0];
                point = str[1];
            }


            public Socket(string ip, string point)
            {
                socketSend = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.ip = ip; this.point = point;
            }

            public void connect()
            {
                if (!socketSend.Connected)
                {
                    IPEndPoint i = new IPEndPoint(IPAddress.Parse(ip), Int32.Parse(point));
                    socketSend.Connect(i);

                    Thread th = new Thread(Receive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }
            }

            private void Receive(object o)
            {
                System.Net.Sockets.Socket socketSend = (System.Net.Sockets.Socket)o;
                while (true)
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    int r=socketSend.Receive(buffer);
                    if (r == 0) break;
                    entity.command = Interpreter.deserialize(Encoding.UTF8.GetString(buffer,0,r));  //解析需要传递个数
                    entity.datetime = DateTime.Now;
                }
            }

            public void send(Command comd)                                              //将command对象发送出去
            {
                try
                {
                    string str = Interpreter.getSerialization(comd);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    socketSend.Send(buffer);
                }
                catch { }
            }

            //public void asyncConnect(ManualResetEvent allDone)
            //{
            //    try
            //    {
            //        SocketObject sb = new SocketObject();
            //        if (allDone != null)
            //            sb.allDone = allDone;
            //        sb.socketWorker = socketWatch;
            //        sb.socketWorker.BeginAccept(new AsyncCallback(acceptCallBack), sb);
            //    }
            //    catch
            //    {

            //    }
            //}

            //private void acceptCallBack(IAsyncResult iar)
            //{
            //    try
            //    {
            //        SocketObject sb = (SocketObject)iar.AsyncState;
            //        Socket socketSend = sb.socketWorker.EndAccept(iar);
            //        socketSends.Add(socketSend.RemoteEndPoint.ToString(), socketSend);
            //        Console.WriteLine(socketSend.RemoteEndPoint.ToString() + "连接成功");
            //        if (sb.allDone != null)
            //        {
            //            sb.allDone.Set();
            //        }
            //    }
            //    catch
            //    {

            //    }
            //}

            //public void asyncSend(Command comd, string socket, ManualResetEvent allDone)
            //{
            //    try
            //    {
            //        SocketObject sb = new SocketObject();
            //        sb.socketWorker = socketSends[socket];
            //        if (allDone != null)
            //        {
            //            sb.allDone = allDone;
            //        }
            //        byte[] buffer = Encoding.UTF8.GetBytes(Interpreter.getSerialization(comd));
            //        sb.socketWorker.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(sendCallBack), sb);
            //    }
            //    catch
            //    {

            //    }
            //}

            //private void sendCallBack(IAsyncResult iar)
            //{
            //    try
            //    {
            //        SocketObject sb = (SocketObject)iar.AsyncState;
            //        int b = sb.socketWorker.EndSend(iar);
            //        Console.WriteLine("服务器发送了{0}字节", b);
            //        if (sb.allDone != null)
            //        {
            //            sb.allDone.Set();
            //        }
            //    }
            //    catch
            //    {

            //    }
            //}
            //public void asyncReceive(string socket, ManualResetEvent allDone)
            //{
            //    try
            //    {
            //        SocketObject sb = new SocketObject();
            //        if (allDone != null)
            //            sb.allDone = allDone;
            //        sb.socketWorker = socketSends[socket];
            //        sb.buffer = new byte[SocketObject.allocate];
            //        sb.socketWorker.BeginReceive(sb.buffer, 0, SocketObject.allocate, 0, new AsyncCallback(receiveCallBack), sb);
            //    }
            //    catch
            //    {

            //    }
            //}

            //private void receiveCallBack(IAsyncResult iar)
            //{
            //    try
            //    {
            //        SocketObject sb = (SocketObject)iar.AsyncState;
            //        int b = sb.socketWorker.EndReceive(iar);
            //        if (b > 0)
            //        {
            //            sb.str.Append(Encoding.UTF8.GetString(sb.buffer));
            //            entity.command = Interpreter.deserialize(sb.str.ToString());
            //            entity.datetime = DateTime.Now;
            //        }
            //        if (sb.allDone != null)
            //        {
            //            sb.allDone.Set();
            //        }
            //    }
            //    catch
            //    {

            //    }
            //}

            public void close()
            {
                socketSend.Shutdown(SocketShutdown.Both);
                socketSend.Close();
            }
            public bool receivedNew()
            {
                DateTime dt = DateTime.Now.AddSeconds(-30);                               //30s内接收的消息为新消息
                if (entity.command == null || dt.CompareTo(entity.datetime) > 0)
                {
                    return false;
                }
                return true;
            }

            public bool received()
            {
                if (entity.command == null)
                {
                    return false;
                }
                return true;
            }

            public Command getData()
            {
                return entity.command;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using PPserver.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace PPserver
{
    namespace Correspondence  //���ƿռ�
    {

        [Serializable]
        public enum _Type { request, respond, alive, notify, msg };//���󣬻�Ӧ�û�����,����,֪ͨ,��Ϣ
        [Serializable]
        public class Command
        {

            public _Type command_type { get; }
            public Command(_Type a)
            {
                command_type = a;
            }
        }
        /// <summary>
        /// �����
        /// </summary>
        [Serializable]
        public class Request : Command
        {

            public UserData user_data;
            public UserState user_state;

            public Request(UserData user_data,UserState user_state) : base(_Type.request)
            {
                this.user_data = user_data;
                this.user_state = user_state;
            }
        }
        [Serializable]
        public class Alive : Command
        {

            public string userid;
            public Alive(string userid)
                : base(_Type.alive)
            {
                this.userid = userid;
            }
        }
        [Serializable]
        public class Respond : Command
        {

            public UserData user_data;

            public UserFrid user_frid;

            public MsgBox msg_box;

            public NoteBox note_box;

            public UserState fridstate;
            public Respond(UserData user_data, UserFrid user_frid, MsgBox msg_box, NoteBox note_box, UserState fridstate)
                : base(_Type.respond)
            {
                this.user_data = user_data;
                this.user_frid = user_frid;
                this.msg_box = msg_box;
                this.note_box = note_box;
                this.fridstate = fridstate;
            }
        }
        [Serializable]
        public class Message : Command
        {

            public string from_user;

            public string to_user;

            public string msg;

            public DateTime time;

            public bool style;             //���ͣ�0Ϊ�������û���1ΪȺ����Ϣ
            public Message(string from_user, string to_user, string msg, DateTime time, bool style)
                : base(_Type.msg)
            {
                this.from_user = from_user;
                this.to_user = to_user;
                this.msg = msg;
                this.time = time;
                this.style = style;
            }

        }
        [Serializable]
        public class Notify : Command
        {

            public UserState fridstate;
            public Notify(UserState fridstate)
                : base(_Type.notify)
            {
                this.fridstate = fridstate;
            }
        }

        public class Interpreter     //���Կͻ��˷��͵�command�������л������յ��ķ����л�������
        {
            public static string getSerialization(Command command)              //��Command�������л�
            {
                string jsonStr = @"D:\runtime\" + DateTime.Now.ToString("yyMMddhhmmss") + ".dat";             //�����л����ļ�����Ϊ���л��Ľ��
                FileStream fileStream = new FileStream(jsonStr, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                BinaryFormatter b = new BinaryFormatter();
                b.Serialize(fileStream, command);
                fileStream.Flush();
                fileStream.Close();
                fileStream.Dispose();
                return jsonStr;
            }

            public static Command deserialize(string jsonStr)    //��jsonstrת����Command���󣬾�̬���з���
            {
                FileStream fileStream = new FileStream(jsonStr, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryFormatter b = new BinaryFormatter();
                Command rs = b.Deserialize(fileStream) as Command;

                File.Delete(jsonStr);
                   
                return rs;
            }
        }

        /// <summary>
        /// ��������MySocket
        /// </summary>
        class MySocket                 
        {
            class SocketObject
            {
                public Socket socketWorker;
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


            Socket socketWatch;                                         //�����׽���
            string ip;
            string point;
            Dictionary<string, Socket> socketSends = new Dictionary<string, Socket>();
            Entity entity = new Entity();

            public  MySocket(string point)
            {
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint i = new IPEndPoint(IPAddress.Any, Int32.Parse(point));
                socketWatch.Bind(i);
                socketWatch.Listen(10);

                this.ip = IPAddress.Any.ToString();
                this.point = point;
            }
            public MySocket(string ip, string point)
            {
                socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress p = IPAddress.Parse(ip);
                IPEndPoint i = new IPEndPoint(p, Int32.Parse(point));
                socketWatch.Bind(i);
                socketWatch.Listen(10);
                this.ip = ip; this.point = point;
            }

            public void accept()
            {
                Thread th = new Thread(Listen);
                th.IsBackground = true;
                th.Start(socketWatch);
            }

            public void Listen(object o)
            {
                while (true)
                {
                    Socket socket = socketWatch.Accept();
                    socketSends.Add(socket.RemoteEndPoint.ToString(), socket);
                    Console.WriteLine(socket.RemoteEndPoint.ToString() + "���ӳɹ�");

                    Thread th = new Thread(Listen);
                    th.IsBackground = true;
                    th.Start(socket);
                }
            }

            private void Receive(object o)
            {
                Socket socketSend = (Socket)o;
                while (true)
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];
                    socketSend.Receive(buffer);
                    entity.command = Interpreter.deserialize(Encoding.UTF8.GetString(buffer));
                    entity.datetime = DateTime.Now;
                }
            }
            public void send(Command comd,string socket)                                              //��command�����ͳ�ȥ
            {
                try
                {
                    string str = Interpreter.getSerialization(comd);
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
                    socketSends[socket].Send(buffer);
                }
                catch { }
            }

            public void asyncAccept(ManualResetEvent allDone)
            {
                try
                {
                    SocketObject sb = new SocketObject();
                    if (allDone != null)
                        sb.allDone = allDone;
                    sb.socketWorker = socketWatch;
                    sb.socketWorker.BeginAccept(new AsyncCallback(acceptCallBack),sb);
                }
                catch
                {

                }
            }

            private void acceptCallBack(IAsyncResult iar)
            {
                try
                {
                    SocketObject sb = (SocketObject)iar.AsyncState;
                    Socket socketSend = sb.socketWorker.EndAccept(iar);
                    socketSends.Add(socketSend.RemoteEndPoint.ToString(), socketSend);
                    Console.WriteLine(socketSend.RemoteEndPoint.ToString() + "���ӳɹ�");
                    if (sb.allDone != null)
                    {
                        sb.allDone.Set();
                    }
                }
                catch
                {

                }
            }

            public void asyncSend(Command comd, string socket, ManualResetEvent allDone)
            {
                try
                {
                    SocketObject sb = new SocketObject();
                    sb.socketWorker = socketSends[socket];
                    if (allDone != null)
                    {
                        sb.allDone = allDone;
                    }
                    byte[] buffer = Encoding.UTF8.GetBytes(Interpreter.getSerialization(comd));
                    sb.socketWorker.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(sendCallBack), sb);
                }
                catch
                {

                }
            }

            private void sendCallBack(IAsyncResult iar)
            {
                try
                {
                    SocketObject sb=(SocketObject)iar.AsyncState;
                    int b=sb.socketWorker.EndSend(iar);
                    Console.WriteLine("������������{0}�ֽ�", b);
                    if (sb.allDone != null)
                    {
                        sb.allDone.Set();
                    }
                }
                catch
                {

                }
            }
            public void asyncReceive(string socket, ManualResetEvent allDone)
            {
                try
                {
                    SocketObject sb = new SocketObject();
                    if (allDone != null)
                        sb.allDone = allDone;
                    sb.socketWorker = socketSends[socket];
                    sb.buffer = new byte[SocketObject.allocate];
                    sb.socketWorker.BeginReceive(sb.buffer, 0, SocketObject.allocate, 0, new AsyncCallback(receiveCallBack), sb);
                }
                catch
                {

                }
            }

            private void receiveCallBack(IAsyncResult iar)
            {
                try
                {
                    SocketObject sb = (SocketObject)iar.AsyncState;
                    int b = sb.socketWorker.EndReceive(iar);
                    if (b > 0)
                    {
                        sb.str.Append(Encoding.UTF8.GetString(sb.buffer,0,b));
                        entity.command = Interpreter.deserialize(sb.str.ToString());
                        entity.datetime = DateTime.Now;
                    }
                    if (sb.allDone != null)
                    {
                        sb.allDone.Set();
                    }
                }
                catch
                {

                }
            }

            public void disconnect(string socket)
            {
                if (socketSends.ContainsKey(socket))
                {
                    Socket so = socketSends[socket];
                    socketSends.Remove(socket);
                    so.Shutdown(SocketShutdown.Both);
                    so.Close();
                }
            }

            public void close()
            {
                foreach (KeyValuePair<string,Socket> i in socketSends)
                {
                    disconnect(i.Value.RemoteEndPoint.ToString());
                }
                socketWatch.Shutdown(SocketShutdown.Both);
                socketWatch.Close();
            }
            public bool receivedNew()
            {
                DateTime dt = DateTime.Now.AddSeconds(-30);                               //30s�ڽ��յ���ϢΪ����Ϣ
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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using PPserver.Correspondence;
using System.Data.SqlClient;
using PPserver.Data;

namespace PPserver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //点击开始监听 在服务端创建一个负责监视IP地址和端口号的Socket
                Socket socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Any;
                //创建端口对象
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32(txtport.Text));
                //监听
                socketWatch.Bind(point);
                ShowMsg("监听成功");
                socketWatch.Listen(10);
                Thread th = new Thread(Listen);
                th.IsBackground = true;
                th.Start(socketWatch);
            }
            catch {  }

        }

        Socket socketSend;

        /// 等待客户端连接，并且创建与之通信的socket
        void Listen(object o)
        {
            Socket socketWatch = o as Socket;
            //等待客户端连接
            while (true)
            {
                try
                {
                    socketSend = socketWatch.Accept();
                    //192.168.0.6:连接成功

                    ShowMsg(socketSend.RemoteEndPoint.ToString() + ":" + "连接成功");
                    //开启新线程接收客户端不断发来的消息
                    Thread th = new Thread(Recive);
                    th.IsBackground = true;
                    th.Start(socketSend);
                }
                catch { }



            }
        }

        /// <summary>
        /// 服务器端不停接收客户端发送的数据
        /// </summary>
        /// <param name="o"></param>
        void Recive(object o)
        {
            Socket socketSend = o as Socket;
            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024 * 1024 * 2];

                    //实际接收的有效字节数
                    int r = socketSend.Receive(buffer);
                    if (r == 0) break;
                    string str = Encoding.UTF8.GetString(buffer, 0, r);
                    Command command = Interpreter.deserialize(str);
                    if (command.command_type == _Type.request)
                    {
                        Request rs = (Request)command;
                        ShowMsg(rs.user_data.userid + "发送了登录请求");
                        if (rs.user_data != null && rs.user_state != null)
                        {
                            
                            Dao dao = new Dao();
                            dao.connect();
                            string sql = "select * from dbo.t_userbase where id='" + rs.user_data.userid + "' and psw='" + rs.user_data.psw + "'";
                            SqlDataReader reader = dao.read(sql);
                            if (reader.Read())
                            {
                                UserData user_data = new UserData();
                                user_data.userid = reader["id"].ToString();
                                user_data.psw = reader["psw"].ToString();
                                user_data.name = reader["name"].ToString();
                                user_data.sex = reader["sex"] . ToString();
                                user_data.phone = reader["phone"].ToString();
                                user_data.image = null;
                                user_data.birth = (DateTime)reader["birth"];
                                Respond respond = new Respond(user_data, null, null, null, null);
                                string ss = Interpreter.getSerialization(respond);
                                byte[] b = Encoding.UTF8.GetBytes(ss);
                                socketSend.Send(b);
                            }
                            else
                            {
                                Respond respond = new Respond(null, null, null, null, null);
                                socketSend.Send(Encoding.UTF8.GetBytes(Interpreter.getSerialization(respond)));
                            }
               
                        }
                        else
                        {
                            Respond respond = new Respond(null, null, null, null, null);
                            socketSend.Send(Encoding.UTF8.GetBytes(Interpreter.getSerialization(respond)));
                        }
                    }

                }
                catch { }
            }
        }

        void ShowMsg(string str)
        {
            txtlog.AppendText(str + "\r\n");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string str = txtMsg.Text.Trim();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(str);
            socketSend.Send(buffer);
            ShowMsg("我:" + str);
            txtMsg.Text = "";
        }
    }
}

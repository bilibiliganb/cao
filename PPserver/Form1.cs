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
using System.IO;

namespace PPserver
{
    public partial class Form1 : Form
    {
        Random random = new Random();
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
                    Command command = Interpreter.deserialize(buffer);

                    //执行登录请求动作
                    doLogin(command);

                    //执行找回密码动作
                    doFindBack(command);

                    //执行注册账号
                    DoRegister(command);

                    //执行转发消息

                }
                catch { }
            }
        }
        /// <summary>
        /// 注册账号
        /// </summary>
        /// <param name="command"></param>
        private void DoRegister(Command command)
        {
            if (command.command_type == _Type.request)
            {
                Request rs = (Request)command;
                if (rs.user_data.userid == "")
                    ShowMsg("新用户发送了注册请求");
                if (rs.user_data.phone != ""&&rs.user_data.sex!="")
                {
                    
                    Dao dao = new Dao();
                    double ID = random.Next(99999, 1000000);
                    string sql = $"insert into t_userbase values('{ID}','{rs.user_data.psw}','{rs.user_data.name}','{rs.user_data.sex}','{rs.user_data.birth}','{rs.user_data.phone}','123.jpg')";
                    dao.Execute(sql);
                    rs.user_data.userid = ID.ToString();
                    UserData user_data = new UserData();
                    user_data.userid = rs.user_data.userid;
                    user_data.psw = rs.user_data.psw;
                    user_data.name = rs.user_data.name;
                    user_data.sex = rs.user_data.sex;
                    user_data.phone = rs.user_data.phone;
                    user_data.image = "123.jpg";
                    user_data.birth = rs.user_data.birth;
                    Respond respond = new Respond(user_data, null, null, null, null);
                    byte[] b = Interpreter.getSerialization(respond);
                    socketSend.Send(b);
              
                }
                else
                {

                }

            }
        }

        void ShowMsg(string str)
        {
            txtlog.AppendText(str + "\r\n");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnStart.PerformClick();
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

        private void doLogin(Command command)
        {
            if (command.command_type == _Type.request)
            {
                Request rs = (Request)command;
                if (rs.user_data != null && rs.user_state != null && rs.user_data.psw!=null)
                {
                    ShowMsg(rs.user_data.userid + "发送了登录请求");
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
                        user_data.sex = reader["sex"].ToString();
                        user_data.phone = reader["phone"].ToString();
                        user_data.image = null;
                        user_data.birth = (DateTime)reader["birth"];
                        Respond respond = new Respond(user_data, null, null, null, null);
                        byte[] b = Interpreter.getSerialization(respond);
                        socketSend.Send(b);
                        ShowMsg("用户登录成功");
                    }
                    else
                    {
                        Respond respond = new Respond(null, null, null, null, null);                //验证失败
                        socketSend.Send(Interpreter.getSerialization(respond));
                    }
                    dao.close();
                }
                
            }
        }

        private void doFindBack(Command command)
        {
            try
            {
                if (command.command_type == _Type.request)
                {
                    Request rs = (Request)command;
                    if(rs.user_data!=null && rs.user_state != null && rs.user_data.psw==null)                  //psw为空说明为找回密码包
                    {
                        ShowMsg(rs.user_data.userid + "请求找回密码");
                        Dao dao = new Dao();
                        dao.connect();
                        string sql = "select * from dbo.t_userbase where id='" + rs.user_data.userid + "'";
                        SqlDataReader reader = dao.read(sql);
                        if (reader.Read())
                        {
                            UserData user_data = new UserData();
                            user_data.userid = reader["id"].ToString();
                            user_data.psw = reader["psw"].ToString();
                            user_data.phone = reader["phone"].ToString();
                            Respond respond = new Respond(user_data, null, null, null, null);
                            byte[] b = Interpreter.getSerialization(respond);
                            socketSend.Send(b);
                        }
                        else
                        {
                            Respond respond = new Respond(null, null, null, null, null);
                            socketSend.Send(Interpreter.getSerialization(respond));
                            ShowMsg("用户不存在");
                        }
                        dao.close();
                    }
                }
            }
            catch
            {

            }
        }
    }
}

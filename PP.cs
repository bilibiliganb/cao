using pp.Correspondence;
using PPserver.Correspondence;
using PPserver.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace pp
{
    public partial class PP : Form
    {
        public static string User_id = "";
        public static Socket mysocket = new Socket("127.0.0.1","9090"); //输入ip和point
        public PP()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                login();
            }
            else
            {
                MessageBox.Show("输入有空项，请重新输入");
            }
        }

        public void login()
        {
            int count = 3;


            mysocket.connect();

            UserData user_data = new UserData();
            user_data.userid = textBox1.Text;
            user_data.psw = textBox2.Text;
            UserState user_state = new UserState();
            user_state.user_id = textBox1.Text;
            user_state.user_state = PPserver.UserManagement._UserState.offline;

            Request rs = new Request(user_data, user_state);    //传入user_data,传入user_state
            mysocket.send(rs);

            while (!mysocket.received()) ;                 //判断接收到消息
            Command command = mysocket.getData();          //接收到服务器传来的数据

            if (command.command_type == _Type.respond)     //应该是respond包
            {
                Respond respond = (Respond)command;
                if (respond.user_data != null)
                {
                    MessageBox.Show("登录成功");
                    count = 3;
                }
                else
                {
                    count--;
                    MessageBox.Show("登录失败");
                    if (count <= 0)
                    {
                        count = 3;
                        MessageBox.Show("您已失败三次，请尝试找回密码");              //弹出失败动作
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                textBox2.PasswordChar = '\0';
            else
                textBox2.PasswordChar = '*';
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FindBack findBack = new FindBack();
            this.Hide();
            findBack.ShowDialog();
            this.Show();
            textBox1.Text = User_id;
        }
    }
}

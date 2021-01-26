using PPserver.Correspondence;
using PPserver.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pp
{
    public partial class FindBack : Form
    {
        private static string  _PhoneNumber="";
        private static string _PSW = "";
        public FindBack()
        {
            InitializeComponent();
        }



        private void FindBack_Load(object sender, EventArgs e)
        {
            tabPage1.Text = "输入要找回的号码";
            tabPage2.Text = "输入密保找回密码";
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
          

            //发送号码
            string PNB = txtPPNB.Text;
            PP.mysocket.connect();
            UserData userData = new UserData();
            userData.userid = PNB;
            UserState userState = new UserState();
            userState.user_id = PNB;
            userState.user_state = PPserver.UserManagement._UserState.offline;
            Request request = new Request(userData,userState);
            PP.mysocket.send(request);

            //接收服务器respond
            while (!PP.mysocket.receivedNew()) ;
            Command command = PP.mysocket.getData();
            if (command.command_type == _Type.respond)
            {
                Respond respond = command as Respond;
                if (respond.user_data != null)
                {
                    _PhoneNumber = respond.user_data.phone;
                    _PSW = respond.user_data.psw;
                }
            }

            string ss = _PhoneNumber.Substring(3, 4);
            string phone = _PhoneNumber.Replace(ss, "****");



            labPNB.Text = phone;
            tabControl1.SelectedTab = tabPage2;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(_PhoneNumber==txtPnb.Text)
            {
                MessageBox.Show("你的密码为" + _PSW);
            }
            else
            {
                MessageBox.Show("验证失败，请重新输入");
                txtPnb.Text = "";
            }
            PP.User_id = txtPPNB.Text;
            this.Close();    
        }
    }
}

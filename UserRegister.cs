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
    public partial class UserRegister : Form
    {
        public UserRegister()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserData user_data = new UserData();
            user_data.name = textBox1.Text;
            user_data.psw = textBox3.Text;
            if (radioButton1.Checked)
            {
                user_data.sex = "男";
            }
            else
            {
                user_data.sex = "女";
            }
            user_data.birth = Convert.ToDateTime(textBox5.Text);
            user_data.phone = textBox6.Text;

            UserState user_state = new UserState();
            user_state.user_id = textBox1.Text;
            user_state.user_state = PPserver.UserManagement._UserState.offline;

            Request rs = new Request(user_data, null);    //传入user_data,传入user_state
            PP.mysocket.send(rs);

            while (!PP.mysocket.received()) ;                 //判断接收到消息
            Command command = PP.mysocket.getData();          //接收到服务器传来的数据
            if (command.command_type == _Type.respond)
            {
                Respond respond = (Respond)command;
                if (respond.user_data != null)
                {
                    MessageBox.Show("注册成功!!您的ID为"+respond.user_data.userid);
                    PP.User_id = respond.user_data.userid;
                    PP.User_psw = respond.user_data.psw;

                }
                else
                {
                    MessageBox.Show("注册失败");
                }
            }
            this.Close();
        }
    }
}

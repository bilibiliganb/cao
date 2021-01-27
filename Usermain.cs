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
    public partial class Usermain : Form
    {
        
        public Usermain()
        {
            InitializeComponent();
        }
        public Usermain(string User_id)
        {
            InitializeComponent();
            Userid = User_id;
        }

        private string _userid;



        /// <summary>
        /// 存储好友的ID
        /// </summary>
        List<string> listFID = new List<string>();

        List<string> listFNM = new List<string>();

        public string Userid { get => _userid; set => _userid = value; }

        private void Usermain_Load(object sender, EventArgs e)
        {
            label1.Text = Data.UName;
            string path = @"123.jpg";
            Image image = Image.FromFile(path);
            pictureBox1.Image=image;
            try
            {
                Dao dao = new Dao();
                string sql = $"select * from t_userfriend where id={Userid}";
                IDataReader dc = dao.read(sql);
                while(dc.Read())
                {
                    listBox1.Items.Add(dc["friendname"].ToString());
                    listFNM.Add(dc["friendname"].ToString());
                    listFID.Add(dc["friend"].ToString());
                }
                
                dao.Daoclose();
            }
            catch { }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ////这里实现发送好友ID查询IP地址
            //string ID = listFID[listBox1.SelectedIndex];
            //string FNM = listFNM[listBox1.SelectedIndex];

            ////在实现通过IP地址初始化聊天器
            
            //Usertalk usertalk = new Usertalk(ID,FNM);
            //usertalk.Show();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //这里实现发送好友ID查询IP地址
            string ID = listFID[listBox1.SelectedIndex];
            string FNM = listFNM[listBox1.SelectedIndex];

            //在实现通过IP地址初始化聊天器

            Usertalk usertalk = new Usertalk(ID, FNM);
            usertalk.Show();
        }
    }
}

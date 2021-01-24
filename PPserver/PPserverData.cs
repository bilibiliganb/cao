using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PPserver.UserManagement;

namespace PPserver
{
    namespace Data
    {
        

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class UserData                              //�û���Ϣ��
        {
            public string userid { get; set; }
            public string psw { get; set; }
            public string name { get; set; }
            public string sex { get; set; }
            public string image { get; set; }
            public DateTime birth { get; set; }
            public string phone { get; set; }        //���ڵ�¼��֤

        }

        [Serializable]
        public class UserFrid                               //�û�������
        {
            public string userid { get; set; }
            public string fridid { get; set; }
            public string fridname { get; set; }    //�൱�ں��ѱ�ע��Ĭ���Ǻ��ѵ�name
        }

        public class Msg                                     //�����û��������������Ϣ
        {
            public string fromuser { get; set; }
            public string touser { get; set; }
            public string msg { get; set; }
            public DateTime datetime { get; set; }
            public bool tag { get; set; }                   //tag=true��ʾ������false��ʾȺ��
        }

        [Serializable]
        public class MsgBox                                 //��Ϣ�����࣬���ڷ������������û�����ʷ��Ϣ��¼
        {
            public string userid { get; set; }
            public string fridid { get; set; }
            public string msg { get; set; }
            public DateTime time { get; set; }
            public bool tag { get; set; }                  //tag=true��ʾ������Ϣ��false��ʾȺ��Ϣ
        }

        [Serializable]
        public class NoteBox                                //��־��
        {
            public string userid { get; set; }
            public string note { get; set; }
            public DateTime time { get; set; }
        }

        [Serializable]
        public class UserState                             //�û�״̬��
        {
            public string user_id { get; set; }
            public _UserState user_state { get; set; }
        }

        [Serializable]
        public class authority                             //�û�Ȩ���б�
        {
            public string userid { get; set; }
            public bool login { get; set; }
            public bool fridctl { get; set; }
            public bool talk { get; set; }
        }
    }
}
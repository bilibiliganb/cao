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

        public class User
        {
            private UserData _userData;
            public UserData UserData { get => _userData; set => _userData = value; }
            public UserInfo UserInfo { get => _userInfo; set => _userInfo = value; }

            private UserInfo _userInfo;
        }
        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class UserData                              //用户信息类
        {
            public const string _UserID = "UserID";
            public const string _Name = "Name";
            public const string _Groups = "Groups";
            public const string _Friends = "Friends";
            public const string _CommentNames = "CommentNames";

            public string UserID { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public string Friends { get; set; }
            public string CommentNames { set; get; }
            public string Groups { set; get; }
            //用于登录验证

            UserData() { }

            public UserData(string userID, string password, string name, string friends, string commentNames, string groups)
            {
                UserID = userID;
                Password = password;
                Name = name;
                Friends = friends;
                CommentNames = commentNames;
                Groups = groups;
            }


        }
        [Serializable]
        public class UserInfo
        {
            public const string _UserID = "UserID";
            public const string _Sex = "Sex";
            public const string _Birth = "Birth";
            public const string _Image = "Image";

            public UserInfo(string userID, string sex, string birth, byte[] image)
            {
                UserID = userID;
                Sex = sex;
                Birth = birth;
                Image = image;
            }

            public string UserID { set; get; }
            public string Sex { set; get; }
            public string Birth { set; get; }
            public byte[] Image { set; get; }


        }


        [Serializable]
        public class Group
        {
            public const string _GroupID = "GroupID";
            public const string _Name = "Name";
            public const string _CreatorID = "CreatorID";
            public const string _Announce = "Announce";
            public const string _Members = "Members";
            public const string _CreateTime = "CreateTime";
            public const string _Version = "Version";


            public string GroupID { get; set; }
            public string Name { get; set; }
            public string CreatorID { get; set; }
            public string Announce { get; set; }
            public string Members { get; set; }
            public string CreateTime { get; set; }
            public string Version { get; set; }


        }


        [Serializable]
        public class ChatMessageRecord
        {
            #region Force Static Check
            public const string TableName = "ChatMessageRecord";
            public const string _AutoID = "AutoID";
            public const string _SpeakerID = "SpeakerID";
            public const string _AudienceID = "AudienceID";
            public const string _IsGroupChat = "IsGroupChat";
            public const string _Content = "Content";
            public const string _OccureTime = "OccureTime";
            #endregion

            public ChatMessageRecord() { }
            public ChatMessageRecord(string speaker, string audience, byte[] _content, bool groupChat)
            {
                this.speakerID = speaker;
                this.audienceID = audience;
                this.Content = _content;
                this.isGroupChat = groupChat;
            }

            #region AutoID
            private long autoID = 0;
            /// <summary>
            /// 自增ID，编号。
            /// </summary>
            public long AutoID
            {
                get { return autoID; }
                set { autoID = value; }
            }
            #endregion

            #region SpeakerID
            private string speakerID = "";
            /// <summary>
            /// 发言人的ID。
            /// </summary>
            public string SpeakerID
            {
                get { return speakerID; }
                set { speakerID = value; }
            }
            #endregion

            #region AudienceID
            private string audienceID = "";
            /// <summary>
            /// 听众ID，可以为GroupID。
            /// </summary>
            public string AudienceID
            {
                get { return audienceID; }
                set { audienceID = value; }
            }
            #endregion

            #region OccureTime
            private DateTime occureTime = DateTime.Now;
            /// <summary>
            /// 聊天记录发生的时间。
            /// </summary>
            public DateTime OccureTime
            {
                get { return occureTime; }
                set { occureTime = value; }
            }
            #endregion

            #region Content
            private byte[] content;
            /// <summary>
            /// 聊天的内容。
            /// </summary>
            public byte[] Content
            {
                get { return content; }
                set { content = value; }
            }
            #endregion

            #region IsGroupChat
            private bool isGroupChat = false;
            /// <summary>
            /// 是否为群聊记录。
            /// </summary>
            public bool IsGroupChat
            {
                get { return isGroupChat; }
                set { isGroupChat = value; }
            }
            #endregion


        }




        [Serializable]
        public class UserState                             //用户状态类
        {
            public string UserID { get; set; }
            public _UserState user_state { get; set; }
        }

        [Serializable]
        public class Authority                             //用户权限列表
        {
            public string UserID { get; set; }
            public bool Login { get; set; }
            public bool Fridctl { get; set; }
            public bool Talk { get; set; }
        }

        [Serializable]
        public class Admin
        {
            public string ID { get; set; }
            public string Password { get; set; }
            public string Password2 { get; set; }
        }

        [Serializable]
        public class LastWordsRecord
        {
            public LastWordsRecord() { }
            public LastWordsRecord(string _speakerID, string _audienceID, bool group, byte[] content)
            {
                this.speakerID = _speakerID;
                this.audienceID = _audienceID;
                this.isGroup = group;
                this.chatContent = content;
            }

            #region SpeakerID
            private string speakerID;
            public string SpeakerID
            {
                get { return speakerID; }
                set { speakerID = value; }
            }
            #endregion

            #region AudienceID
            private string audienceID;
            public string AudienceID
            {
                get { return audienceID; }
                set { audienceID = value; }
            }
            #endregion

            #region IsGroup
            private bool isGroup = false;
            public bool IsGroup
            {
                get { return isGroup; }
                set { isGroup = value; }
            }
            #endregion

            #region ChatContent
            private byte[] chatContent;
            public byte[] ChatContent
            {
                get { return chatContent; }
                set { chatContent = value; }
            }
            #endregion

            #region SpeakTime
            private DateTime speakTime = DateTime.Now;
            public DateTime SpeakTime
            {
                get { return speakTime; }
                set { speakTime = value; }
            }
            #endregion

        }


    }

}
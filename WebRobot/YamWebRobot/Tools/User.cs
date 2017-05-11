using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YamWebRobot
{
    /// <summary>
    /// 存储当前登陆会员信息
    /// </summary>
    public class User
    {
        public static User currentUser = null;  //全局单列

        static User()
        {
            if (currentUser == null)
            {
                currentUser = new User();
            }
        }
        public string orgName { get; set; }

        private string _token;
        public string token
        {
            get
            {
                if (_token == null)
                {
                    _token = "";
                }
                return _token;
            }
            set
            {
                _token = value;
            }
        }
    }
}

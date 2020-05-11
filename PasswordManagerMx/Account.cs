using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManagerMx
{
    public class Account
    {
        /*
         * This class contains all of login information for a given online account
         */

        private string name, email, username, pw;
        private List<string> etcInfo; //Anything else an account might have but not necessarily
                                      //such as security questions, account numbers...etc

        public Account(string name_, string email_, string username_, string pw_) {
            name = name_;
            email = email_;
            username = username_;
            pw = pw_;
        }

        public Account(string name_, string email_, string username_, string pw_, List<string> etcInfo_)
        {
            name = name_;
            email = email_;
            username = username_;
            pw = pw_;
            etcInfo = etcInfo_;
        }

        public string Name{
            get => name;
            set => name = value;
        }

        public string Email {
            get => email;
            set => email = value;
        }

        public string Username
        {
            get => username;
            set => username = value;
        }

        public string Password
        {
            get => pw;
            set => pw = value;
        }

        public List<string> EtcInfo
        {
            get => etcInfo;
            set => etcInfo = value;
        }

        //The plain text version of the line in the encrypted pw file
        public string ToString() {
            string retval = name + "," + email + "," + username + "," +
                pw;
            if (etcInfo != null)
            {
                List<string> etcInfoTemp = etcInfo;
                for (int n = 0; n < etcInfoTemp.Count; n++)
                {
                    retval = retval + "," + etcInfoTemp[n];
                }
            }
            return retval;
        }
    }
}

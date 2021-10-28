using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NCRQuality_Inspection
{
    class LoginNT
    {
        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern int LogonUser(string lpszUserName, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;
        IntPtr token = IntPtr.Zero;

        private string sUser;
        private string sPassword;


        public LoginNT()
        {
            sUser = "";
            sPassword = "";
        }

        public LoginNT(string sUserName, string sPassword)
        {
            this.sUser = sUserName;
            this.sPassword = sPassword;
        }

        public bool Logon()
        {
            if (LogonUser(this.sUser, "corp.JABIL.ORG", this.sPassword, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Logon(string sUserName, string sPassword)
        {
            if (LogonUser(sUserName, "corp.JABIL.ORG", sPassword, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

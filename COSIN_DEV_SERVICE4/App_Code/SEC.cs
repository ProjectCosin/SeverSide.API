using System;
using System.Collections.Generic;
using System.Web;
using System.Text;

namespace com.cooshare.api
{

    public class SEC
    {

        public SEC()
        {
        }


        public static string SECURITY_ContentEncrypt(string Str)
        {

            string[] p = new string[1];
            p[0] = Str;

            if (ParaHelper.IsParameterUnSafe(p)) return "-2";


            byte[] bytes = Encoding.Default.GetBytes(Str);
            string ret = Convert.ToBase64String(bytes);

            ret = GenerateRandom(3) + ret + GenerateRandom(4);

            return ret;

        }


        public static bool SECURITY_RequestDecrypt(string[,] Para, string SK, string DevId)
        {


            string query = "SELECT PRIVATEKEY FROM DC_DEVELOPERINFO WHERE DEVELOPERID='" + DevId + "'";
            string PrivateKey = DataHelperForDevService.Query_ExecuteScalar(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);


            string links = "";
            for (int s = 0; s < Para.Length / 2; s++)
            {
                links += Para[0, s] + "=" + Para[1, s] + "&";
            }
            links = links.Substring(0, links.Length - 1);
            links += PrivateKey;

            string sks = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(links, "MD5").Substring(8, 16).ToLower();

            if (sks == SK.ToLower())
            {

                return true;
            }
            else
            {
                return false;
            }

        }

        public static string SECURITY_ContentDecrypt(string Str)
        {

            string[] p = new string[1];
            p[0] = Str;

            if (ParaHelper.IsParameterUnSafe(p)) return "-2";


            string ret = Str.Substring(3, Str.Length - 7);
            byte[] outputb = Convert.FromBase64String(ret);
            string orgStr = Encoding.Default.GetString(outputb);
            return orgStr;
        }

        private static char[] constant =
        {
           '0','1','2','3','4','5','6','7','8','9',
           'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
           'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
        };
        private static string GenerateRandom(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }
            return newRandom.ToString();
        }
    }
}
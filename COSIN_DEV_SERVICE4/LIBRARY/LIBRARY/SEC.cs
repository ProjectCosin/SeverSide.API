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

           
            return "";

        }


        public static bool SECURITY_RequestDecrypt(string[,] Para, string SK, string DevId)
        {


            return true;

        }

        public static string SECURITY_ContentDecrypt(string Str)
        {

          
            return "";
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
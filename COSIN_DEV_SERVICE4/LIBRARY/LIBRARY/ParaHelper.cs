using System;
using System.Collections.Generic;
using System.Text;

namespace com.cooshare.api
{
    class ParaHelper
    {

        public static bool IsParameterUnSafe(string[] para)
        {

            bool error = false;

            for (int m = 0; m < para.Length; m++)
            {

                if (para[m].ToString().Equals(""))
                {

                    error = true;
                    break;
                }


            }

            return error;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

/// <summary>
///DataHelper 的摘要说明
/// </summary>
/// 

namespace com.cooshare.api
{

    public class DataHelperForDevService
    {
        public DataHelperForDevService()
        {

        }

        public enum DataBaseFact
        {
            CENTRAL,
            PROPERTY
        };

        public static string OUTPUT_PARA_INT16 = "0";
        public static string OUTPUT_PARA_INT32 = "1";
        public static string OUTPUT_PRAR_STRING = "2";


        public static DataTable Query_SqlDataAdapter(DataBaseFact connectionstring, string query, string[] args)
        {



            return new DataTable();


        }


        public static int Query_ExecuteNonQuery(DataBaseFact connectionstring, string query, string[] args)
        {

            return 0;


        }


        public static string Query_ExecuteScalar(DataBaseFact connectionstring, string query, string[] args)
        {
            
        
                    return "";
             
        }



        public static List<SqlParameter> QueryWithSP_OUTPUT(DataBaseFact connectionstring, string SPName, string[,] ParaCollection, string[,] OutPutCollection)
        {

          return new List<SqlParameter>();

               

        }



        public static DataTable QueryWithSP_SqlDataAdapter(DataBaseFact connectionstring, string SPName, string[,] ParaCollection, string[,] OutPutCollection)
        {

            return new DataTable();

        }

    }
}
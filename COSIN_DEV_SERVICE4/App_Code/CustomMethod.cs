using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using com.cooshare.api;
using System.Data;

/// <summary>
///CustomMethod 的摘要说明
/// </summary>
[WebService(Namespace = "http://com.cooshare.api/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class CustomMethod : System.Web.Services.WebService
{
    /// <summary>
    /// EP模型方法调用
    /// </summary>
    public CustomMethod()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }


    /// <summary>
    /// EP自定义方法调用
    /// </summary>
    /// <param name="epid">EP设备ID</param>
    /// <param name="method">方法名</param>
    /// <param name="para">方法入口参数</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// {返回值},{执行结果} 注：对于void类型方法，返回值默认为0；执行结果为1则说明方法执行成功
    /// 0: 没有找到相关的方法
    /// -1：输入参数与方法定义参数不一致
    /// -3: 方法执行错误
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string invoke(string epid, string method, string para, string devid, string sk)
    {


        string[,] p = new string[2, 4];
        p[0, 0] = "epid";
        p[1, 0] = epid;
        p[0, 1] = "method";
        p[1, 1] = method;
        p[0, 2] = "para";
        p[1, 2] = para;
        p[0, 3] = "devid";
        p[1, 3] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        epid = SEC.SECURITY_ContentDecrypt(epid);
        method = SEC.SECURITY_ContentDecrypt(method);
        para = SEC.SECURITY_ContentDecrypt(para);


        string query = "SELECT * FROM EP_PRODUCT_METHODS_FACT"
                        + " WHERE EP_PRODUCT_ID"
                        + " IN"
                        + " (SELECT EP_PRODUCTID FROM ENDPOINT_FACT WHERE EP_ID = '" + epid + "') AND METHODNAME='" + method + "'";

        DataTable dt = DataHelperForDevService.Query_SqlDataAdapter(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);

        if (dt.Rows.Count == 0) return SEC.SECURITY_ContentEncrypt("0");

        string methodname = dt.Rows[0]["MethodName"].ToString();
        string returnType = dt.Rows[0]["returntype"].ToString();
        string paracollection = dt.Rows[0]["paracollection"].ToString();
        string id = dt.Rows[0]["id"].ToString();

        string sp_para = "";

        string procedurename = "invokMethod_" + id;

        #region 确定双方参数数量

        int paracollection_number = 0;
        int para_number = 0;

        try
        {

            if (paracollection.Trim() == "")
            {
                paracollection_number = 0;
            }

            if (paracollection.IndexOf(',') == -1)
            {
                paracollection_number = 1;
            }
            else
            {
                string[] s = paracollection.Split(',');
                paracollection_number = s.Length;
            }

            if (para.Trim() == "")
            {
                para_number = 0;
            }
            if (para.IndexOf(',') == -1)
            {
                para_number = 1;
            }
            else
            {
                string[] s = para.Split(',');
                para_number = s.Length;
            }

        }
        catch { }


        #endregion

        if (paracollection_number + 1 != para_number) return SEC.SECURITY_ContentEncrypt("-1");

        if (paracollection_number == 0)
        {
            sp_para += " @SESSIONINEPID = N'" + para + "' ";
        }
        else if (paracollection_number == 1)
        {
            sp_para += paracollection + "=N'" + para.Split(',')[0].ToString().Trim() + "', @SESSIONINEPID = N'" + para.Split(',')[1].ToString().Trim() + "' ";
        }
        else
        {

            string[] x = paracollection.Split(',');
            string[] y = para.Split(',');

            for (int j = 0; j < x.Length; j++) sp_para += x[j].ToString().Trim() + "=" + y[j].ToString().Trim() + ", ";

            sp_para += " @SESSIONINEPID = N'" + y[x.Length].ToString().Trim() + "' ";
        }


        if (returnType == "1")
        {

            sp_para += " ,@INVOKERETURN = @INVOKERETURN OUTPUT;  SELECT @INVOKERETURN AS 'RET','1' AS 'STA'";

        }
        else
        {

            sp_para += "; SELECT '0' AS 'RET', '1' AS 'STA'";
        }

        string execquery = "DECLARE @INVOKERETURN NVARCHAR(500); EXEC " + procedurename + " " + sp_para;

        try
        {
            DataTable dts = DataHelperForDevService.Query_SqlDataAdapter(DataHelperForDevService.DataBaseFact.PROPERTY, execquery, null);
            if (dts == null) return SEC.SECURITY_ContentEncrypt("-3");

            if (dts.Rows.Count == 0)
            {

                return SEC.SECURITY_ContentEncrypt("-3");
            }
            else
            {

                return SEC.SECURITY_ContentEncrypt(dts.Rows[0]["RET"].ToString() + "," + dts.Rows[0]["STA"].ToString());
            }
        }
        catch
        {
            return SEC.SECURITY_ContentEncrypt("-3");
        }
    }

}

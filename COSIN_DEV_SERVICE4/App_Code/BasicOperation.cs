using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using com.cooshare.api;
using System.Data;
using System.Data.SqlClient;
using com.cooshare.os.dev;
using com.cooshare.os;
using System.Collections;
using System.Text;
using COSWEBIDE;

/// <summary>
///BasicOperation 的摘要说明
/// </summary>
[WebService(Namespace = "http://com.cooshare.api/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]

public class BasicOperation : System.Web.Services.WebService
{
    /// <summary>
    /// EP设备基本操作
    /// </summary>
    public BasicOperation()
    {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    private enum getendpointlistmodeenum
    {
        epid,
        epname,
        devid,
        epmodename,
        epmodeid
    };


    /// <summary>
    /// 用户登录接口
    /// </summary>
    /// <param name="login_mail">登录电邮</param>
    /// <param name="login_password">登录密码</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// 0, -2 in nondecryptstatus：说明用户名或密码错误
    /// 正整数：指返回对应的COSID
    /// -4: 安全码验证失败
    /// </returns>
       [WebMethod]
    public string UserAuthentication(string login_mail,string login_password,string devid,string sk) {

        string[,] p = new string[2, 3];
        p[0, 0] = "login_mail";
        p[1, 0] = login_mail;
        p[0, 1] = "login_password";
        p[1, 1] = login_password;
        p[0, 2] = "devid";
        p[1, 2] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        
	if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");




        login_mail = SEC.SECURITY_ContentDecrypt(login_mail);

        
        string query = "select HCCU_ID from HCCU_FACT WHERE HCCU_UID='" + login_mail + "' AND SUBSTRING(HCCU_PSW,4,LEN(HCCU_PSW)-7)=SUBSTRING('" + login_password + "',4,LEN('" + login_password + "')-7)";
        

        try
        {
            string ret0 = DataHelperForDevService.Query_ExecuteScalar(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);

            return SEC.SECURITY_ContentEncrypt(ret0); // return -2 if the input para equals null.
        }
        catch {

            return SEC.SECURITY_ContentEncrypt("0");
        }

    }


  /// <summary>
    /// 用户登录接口
    /// </summary>
    /// <param name="login_id">登录账号</param>
    /// <param name="login_password">登录密码</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// 0：说明用户名或密码错误
    /// 正整数|字符串：指返回对应的COSID与LOCAL IP
    /// 
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string UserAuthenticationWithLocalIPVarify(string login_id, string login_password, string devid, string sk)
    {

        string[,] p = new string[2, 3];
        p[0, 0] = "login_id";
        p[1, 0] = login_id;
        p[0, 1] = "login_password";
        p[1, 1] = login_password;
        p[0, 2] = "devid";
        p[1, 2] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        login_id = SEC.SECURITY_ContentDecrypt(login_id);

        string query = "select HCCU_ID from HCCU_FACT WHERE HCCU_UID='" + login_id + "' AND HCCU_PSW = '" + login_password + "'";

        try
        {
            string ret0 = DataHelperForDevService.Query_ExecuteScalar(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);
            string query2 = "select ip from v_hccu where hccu_id='" + ret0 + "'";
            string ret1 = DataHelperForDevService.Query_ExecuteScalar(DataHelperForDevService.DataBaseFact.CENTRAL, query2, null);

            return SEC.SECURITY_ContentEncrypt(ret0 + "|" + ret1);
        }
        catch
        {

            return SEC.SECURITY_ContentEncrypt("0");
        }

    }


    /// <summary>
    /// 同步事件方法
    /// </summary>
    /// <param name="codecontent">事件代码内容</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// 1: 事件更新成功
    /// 0: 事件移除成功
    /// -1: 代码语法错误
    /// -2: 操作失败
    /// </returns>
    [WebMethod]
    public string SyncEventMethods(string codecontent, string epid, string propertyname, string devid, string sk)
    {
        string[,] p = new string[2, 4];
        p[0, 0] = "codecontent";
        p[1, 0] = codecontent;
        p[0, 1] = "epid";
        p[1, 1] = epid;
        p[0, 2] = "propertyname";
        p[1, 2] = propertyname;
        p[0, 3] = "devid";
        p[1, 3] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        codecontent = SEC.SECURITY_ContentDecrypt(codecontent);
        propertyname = SEC.SECURITY_ContentDecrypt(propertyname);
        epid = SEC.SECURITY_ContentDecrypt(epid);

        ArrayList x = new ArrayList();
        EventCompiler c = null;

        if (codecontent != "")
        {
            try
            {
                c = new EventCompiler(devid, epid, propertyname);
                x = c.GetProcess(codecontent);
            }
            catch
            {
                x = null;
            }

        }
        else
        {
            string query2 = "DELETE FROM EP_EVENTS_CODE WHERE EVENT_CODE_ID IN (SELECT CODEID FROM  EP_EVENTS_FACT WHERE EP_ID = '" + epid + "' AND PROPERTYNAME = '" + propertyname + "');"
                          + "DELETE FROM EP_EVENTS_FACT WHERE EP_ID = '" + epid + "' AND PROPERTYNAME = '" + propertyname + "'";
            int ret2 = DataHelper_COSWEBIDE.Query_ExecuteNonQuery(DataHelper_COSWEBIDE.DataBaseFact.PROPERTY, query2, null);

            if (ret2 == 1)
            {
                return "0";
            }
            else
            {
                return "-2";
            }

        }

        try
        {
            if (x == null)
            {
                return "-1";
            }
        }
        catch { }

        string query = "DELETE FROM EP_EVENTS_CODE WHERE EVENT_CODE_ID IN (SELECT CODEID FROM  EP_EVENTS_FACT WHERE EP_ID = '" + epid + "' AND PROPERTYNAME = '" + propertyname + "');"
                     + "DELETE FROM EP_EVENTS_FACT WHERE EP_ID = '" + epid + "' AND PROPERTYNAME = '" + propertyname + "'";
        int ret = DataHelper_COSWEBIDE.Query_ExecuteNonQuery(DataHelper_COSWEBIDE.DataBaseFact.PROPERTY, query, null);



        if (ret == 1)
        {
            string ret2 = "0";
            string q2 = "INSERT INTO EP_EVENTS_CODE (RAW_CODE) VALUES (N'" + codecontent.Replace("'", "''") + "'); SELECT SCOPE_IDENTITY();";
            ret2 = DataHelper_COSWEBIDE.Query_ExecuteScalar(DataHelper_COSWEBIDE.DataBaseFact.PROPERTY, q2, null);

            for (int s = 0; s < x.Count; s++)
            {
                string q3 = "INSERT INTO EP_EVENTS_FACT (EP_ID,PROPERTYNAME,TRIGGERCONTENT,STATUS,CODEID) VALUES ('" + epid + "','" + propertyname + "',N'" + x[s].ToString().Replace("'", "''") + "','1','" + ret2 + "')";
                DataHelper_COSWEBIDE.Query_ExecuteNonQuery(DataHelper_COSWEBIDE.DataBaseFact.PROPERTY, q3, null);
            }

            return "1";
        }
        else
        {

            return "-2";

        }


    }


    /// <summary>
    /// 获取EP设备信息
    /// </summary>
    /// <param name="para">查找依据参数</param>
    /// <param name="getendpointlistmode">依据参数类型</param>
    /// <param name="cosid">COSID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// {EPID},{EP分类ID},{EP名称},{EP所属模型ID},{EP所属COSID},{EP的MAC地址}|Repeat(n)|
    /// 0: 无相关记录
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string GetEndPoint(string para, string getendpointlistmode, string cosid, string devid, string sk)
    {

        string[,] p = new string[2, 4];
        p[0, 0] = "para";
        p[1, 0] = para;
        p[0, 1] = "getendpointlistmode";
        p[1, 1] = getendpointlistmode;
        p[0, 2] = "cosid";
        p[1, 2] = cosid;
        p[0, 3] = "devid";
        p[1, 3] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        para = SEC.SECURITY_ContentDecrypt(para);
        getendpointlistmode = SEC.SECURITY_ContentDecrypt(getendpointlistmode);
        cosid = SEC.SECURITY_ContentDecrypt(cosid);

        if (para.IndexOf('^') != -1 && para.IndexOf('#') != -1) para = Decode_hc(para);

        string query = "";
        string ret = "";

        if (getendpointlistmode == getendpointlistmodeenum.devid.ToString())
        {

            query = "select * from endpoint_Fact where hccu_id='" + para + "'";
        }
        else if (getendpointlistmode == getendpointlistmodeenum.epid.ToString())
        {

            query = "select * from endpoint_Fact where ep_id='" + para + "'";
        }
        else if (getendpointlistmode == getendpointlistmodeenum.epmodeid.ToString())
        {
            query = "select * from endpoint_Fact where ep_productid='" + para + "'";
        }
        else if (getendpointlistmode == getendpointlistmodeenum.epmodename.ToString())
        {
            query = "select * from endpoint_Fact where ep_productid"
                    + " in"
                    + " (select ep_product_id from endpoint_product_fact where ep_product_name='" + para + "' and dev_id='" + devid + "')";
        }
        else if (getendpointlistmode == getendpointlistmodeenum.epname.ToString())
        {
            query = "select * from endpoint_Fact where ep_userdefined_alias='" + para + "' and hccu_id='" + cosid + "'";
        }

        DataTable dt = DataHelperForDevService.Query_SqlDataAdapter(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);

        if (dt.Rows.Count == 0) return SEC.SECURITY_ContentEncrypt("0");

        for (int s = 0; s < dt.Rows.Count; s++)
        {

            byte[] alias = Encoding.UTF8.GetBytes(dt.Rows[0]["EP_USERDEFINED_ALIAS"].ToString().Trim());


            ret += dt.Rows[0]["EP_ID"].ToString().Trim() + "," + dt.Rows[0]["EP_TYPEID"].ToString().Trim() + ","
                + Convert.ToBase64String(alias) + ","
                + dt.Rows[0]["EP_PRODUCTID"].ToString().Trim() + "," + dt.Rows[0]["HCCU_ID"].ToString().Trim() + "," + dt.Rows[0]["EP_MAC_ID"].ToString().Trim() + "|";
        }

        return SEC.SECURITY_ContentEncrypt(ret);
    }

    /// <summary>
    /// 删除EP设备
    /// </summary>
    /// <param name="para">操作依据参数</param>
    /// <param name="cosid">COSID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// 1: 操作成功
    /// -2: 该EP不属于当前用户
    /// -3: 系统执行异常
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string DeleteEndPoint(string para, string cosid,string devid, string sk)
    {

        string[,] p = new string[2, 3];
        p[0, 0] = "para";
        p[1, 0] = para;
        p[0, 1] = "cosid";
        p[1, 1] = cosid;
        p[0, 2] = "devid";
        p[1, 2] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        para = SEC.SECURITY_ContentDecrypt(para);
        cosid = SEC.SECURITY_ContentDecrypt(cosid);


        string query = "select count(*) from ENDPOINT_FACT WHERE EP_ID='" + para + "' AND HCCU_ID = '" + cosid + "'";
        string ret0 = DataHelperForDevService.Query_ExecuteScalar(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);

        if (ret0 == "0") return SEC.SECURITY_ContentEncrypt("-2");

        string[,] pa = new string[2, 1];
        pa[0, 0] = "EP_ID";
        pa[1, 0] = para;

        string[,] op = new string[2, 1];
        op[0, 0] = "RET";
        op[1, 0] = DataHelperForDevService.OUTPUT_PARA_INT16;

        List<SqlParameter> ret = DataHelperForDevService.QueryWithSP_OUTPUT(DataHelperForDevService.DataBaseFact.CENTRAL, "EP_DELETE_SP", pa, op);

        if (((SqlParameter)ret[0]).Value.ToString().Trim() != "1")
        {

            return SEC.SECURITY_ContentEncrypt("-3");
        }

        return SEC.SECURITY_ContentEncrypt("1");

    }

    /// <summary>
    /// 添加EP分类
    /// </summary>
    /// <param name="categoryname">分类名称</param>
    /// <param name="categorydesp">分类描述</param>
    /// <param name="cosid">COSID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// {新添加的分类ID}
    /// 0： 所提交的分类名称在该COSID下添加过
    /// -1：系统执行异常
    /// -2：参数不符合标准
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string AddEPCategory(string categoryname, string categorydesp, string cosid,string devid, string sk)
    {

        string[,] p = new string[2, 4];
        p[0, 0] = "categoryname";
        p[1, 0] = categoryname;
        p[0, 1] = "categorydesp";
        p[1, 1] = categorydesp;
        p[0, 2] = "cosid";
        p[1, 2] = cosid;
        p[0, 3] = "devid";
        p[1, 3] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        categoryname = SEC.SECURITY_ContentDecrypt(categoryname);
        categorydesp = SEC.SECURITY_ContentDecrypt(categorydesp);
        cosid = SEC.SECURITY_ContentDecrypt(cosid);

        COS_WEBSERVICE_EPTYPE c = new COS_WEBSERVICE_EPTYPE();
        return SEC.SECURITY_ContentEncrypt(c.EPTYPE_Add(categoryname, categorydesp, cosid).ToString());

    }


    /// <summary>
    /// 删除EP分类
    /// </summary>
    /// <param name="para">分类ID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// 1：操作成功
    /// 0: 该分类下还有EP设备,无法删除
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string DeleteEPCategory(string para, string devid, string sk)
    {

        string[,] p = new string[2, 2];
        p[0, 0] = "para";
        p[1, 0] = para;
        p[0, 1] = "devid";
        p[1, 1] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        para = SEC.SECURITY_ContentDecrypt(para);

        string[,] pa = new string[2, 1];
        pa[0, 0] = "EP_TYPEID";
        pa[1, 0] = para;

        string[,] op = new string[2, 1];
        op[0, 0] = "RET";
        op[1, 0] = DataHelperForDevService.OUTPUT_PARA_INT16;

        List<SqlParameter> ret = DataHelperForDevService.QueryWithSP_OUTPUT(DataHelperForDevService.DataBaseFact.CENTRAL, "EPTYPE_DELETE_SP", pa, op);

        string retvalue = ((SqlParameter)ret[0]).Value.ToString().Trim();

        if (retvalue == "1")
        {

            return SEC.SECURITY_ContentEncrypt("1");
        }
        else
        {

            return SEC.SECURITY_ContentEncrypt("0");
        }

    }


    /// <summary>
    /// 获取EP分类信息
    /// </summary>
    /// <param name="para">EP分类ID（如需列出本COSID下的全部分类，则赋值0）</param>
    /// <param name="cosid">COSID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// {EP_TYPE_ID},{EP_TYPE_NAME},{EP_TYPE_DESCRIPTION}|Repeat(n)|
    /// 0： 无相关信息
    /// -3: 系统执行异常
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string GetEPCategory(string para, string cosid,string devid, string sk)
    {


        string[,] p = new string[2, 3];
        p[0, 0] = "para";
        p[1, 0] = para;
        p[0, 1] = "cosid";
        p[1, 1] = cosid;
        p[0, 2] = "devid";
        p[1, 2] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        para = SEC.SECURITY_ContentDecrypt(para);
        cosid = SEC.SECURITY_ContentDecrypt(cosid);
            

        if (para != "0")
        {
            string query = "select * from endpoint_type_fact where HCCU_ID='" + cosid + "' and EP_TYPE_ID='" + para + "'";
            DataTable dt = DataHelperForDevService.Query_SqlDataAdapter(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);

            if (dt.Rows.Count == 0) return SEC.SECURITY_ContentEncrypt("0");

            string ret = "";
            for (int x = 0; x < dt.Rows.Count; x++)
            {

                ret += dt.Rows[x]["EP_TYPE_ID"].ToString() + "," + dt.Rows[x]["EP_TYPE_NAME"].ToString() + "," + dt.Rows[x]["EP_TYPE_DESCRIPTION"].ToString() + "|";
            }

            return SEC.SECURITY_ContentEncrypt(ret);
        }
        else
        {

            COS_WEBSERVICE_EPTYPE c = new COS_WEBSERVICE_EPTYPE();
            ArrayList al = c.EPTYPE_GetList(cosid);
            string ret = "";

            if (al.Count == 0) return SEC.SECURITY_ContentEncrypt("0");

            for (int x = 0; x < al.Count; x++)
            {

                if (al[x].ToString().Trim() == "-1" || al[x].ToString().Trim() == "-2") return SEC.SECURITY_ContentEncrypt("-3");
                ret += al[x].ToString() + "|";
            }

            return SEC.SECURITY_ContentEncrypt(ret);
        }

    }

    /// <summary>
    /// 获取指令执行队列信息
    /// </summary>
    /// <param name="para">EPID</param>
    /// <param name="cosid">COSID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// {待控制的属性名},{待控制的属性值},{指令建立时间},{是否下发(1:已下发|0:未下发)},{是否过期(1:已过期|0:未过期)}|Repeat(n)|
    /// 0: 无相关信息
    /// -3: 无权限查看该EP信息
    /// -4: 安全码验证失败
    /// </returns>
    [WebMethod]
    public string GetCurrentExecOrder(string para, string cosid,string devid, string sk)
    {

        string[,] p = new string[2, 3];
        p[0, 0] = "para";
        p[1, 0] = para;
        p[0, 1] = "cosid";
        p[1, 1] = cosid;
        p[0, 2] = "devid";
        p[1, 2] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        para = SEC.SECURITY_ContentDecrypt(para);
        cosid = SEC.SECURITY_ContentDecrypt(cosid);

        string query0 = "select count(*) from endpoint_fact where hccu_id='" + cosid + "'";
        string ret0 = DataHelperForDevService.Query_ExecuteScalar(DataHelperForDevService.DataBaseFact.CENTRAL, query0, null);

        if (ret0 == "0") return SEC.SECURITY_ContentEncrypt("-3");

        string query = "select * from exec_order where ep_id = '" + para + "'";

        DataTable dt = DataHelperForDevService.Query_SqlDataAdapter(DataHelperForDevService.DataBaseFact.PROPERTY, query, null);

        if (dt.Rows.Count == 0)
        {
            return SEC.SECURITY_ContentEncrypt("0");
        }
        else
        {

            string ret = "";
            for (int x = 0; x < dt.Rows.Count; x++)
            {
                ret += dt.Rows[x]["PROP"].ToString().Trim() + "," + dt.Rows[x]["VALUE"].ToString().Trim() + "," + dt.Rows[x]["ORDER_DATE"].ToString().Trim() + "," + dt.Rows[x]["IFSENT"].ToString().Trim()
                    + "," + dt.Rows[x]["EXPIRE"].ToString().Trim() + "|";
            }

            return SEC.SECURITY_ContentEncrypt(ret);
        }


    }



    /// <summary>
    /// 更新HCCU设备的网络信息
    /// </summary>
    /// <param name="cosid">COSID</param>
    /// <param name="devid">开发者ID</param>
    /// <param name="sk">安全码</param>
    /// <returns>
    /// IP,PORT 前者是代表该HCCU的本地内网IP，PORT说明SOCKET端口
    /// 0：说明没有找到相关的HCCU信息
    /// </returns>
    [WebMethod]
    public string GetHCCUNetworkInfo(string cosid,string devid, string sk)
    {

        string[,] p = new string[2, 2];
        p[0, 0] = "cosid";
        p[1, 0] = cosid;
        p[0, 1] = "devid";
        p[1, 1] = devid;

        devid = SEC.SECURITY_ContentDecrypt(devid);
        if (!SEC.SECURITY_RequestDecrypt(p, sk, devid)) return SEC.SECURITY_ContentEncrypt("-4");

        cosid = SEC.SECURITY_ContentDecrypt(cosid);


        string query = "select * from hccu_mac_fact where hccu_id='" + cosid + "'";
        DataTable dt = DataHelperForDevService.Query_SqlDataAdapter(DataHelperForDevService.DataBaseFact.CENTRAL, query, null);

        if (dt.Rows.Count == 0)
        {
            return SEC.SECURITY_ContentEncrypt("0");
        }
        else
        {

            string returns = "";
            for (int x = 0; x < dt.Rows.Count; x++)
            {

                returns += dt.Rows[0]["IP"].ToString() + "," + dt.Rows[0]["PORT"].ToString() + "|";
            }
            return SEC.SECURITY_ContentEncrypt(returns);
        }

    }

    public static string Decode_hc(string inputString)
    {

        return decode_hc(inputString);
    }

    private static string decode_hc(string inputString)
    {
        string decodeString = "";
        string tempString = "";
        string finalreturn = "";

        string[] Anay_str = inputString.ToString().Split(';');
        for (int z = 0; z < Anay_str.Length; z++)
        {

            int pivo = Anay_str[z].ToString().IndexOf('^');

            if (pivo == 0)
            {
                decodeString = "";
                tempString = Anay_str[z].ToString().Replace("^#x", "");
                int iASCII = int.Parse(tempString, System.Globalization.NumberStyles.HexNumber);
                decodeString = decodeString + (char)iASCII;
            }
            else if (pivo > 0)
            {
                decodeString = "";
                string ordinaryString = Anay_str[z].ToString().Substring(0, pivo);
                tempString = Anay_str[z].ToString().Substring(pivo, Anay_str[z].ToString().Length - pivo);
                tempString = tempString.Replace("^#x", "");
                int iASCII = int.Parse(tempString, System.Globalization.NumberStyles.HexNumber);
                decodeString = decodeString + (char)iASCII;
                decodeString = ordinaryString + decodeString;

            }
            else if (pivo == -1)
            {
                decodeString = "";
                decodeString = Anay_str[z].ToString();

            }

            finalreturn += decodeString;

        }

        return finalreturn;
    }


    public static string Encode(string inputString)
    {
        return encode(inputString);
    }

    private static string encode(string inputString)
    {
        inputString = inputString.Replace("&", "&amp;").Replace("#", "＃");
        string unicodeString = "";
        UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
        byte[] bOut = unicodeEncoding.GetBytes(inputString);
        for (int i = 0; i < bOut.Length; i++)
        {
            string lowChar = bOut[i].ToString("X");
            i++;
            string highChar = bOut[i].ToString("X");

            if (lowChar.Length == 1)
            {
                lowChar = "0" + lowChar;
            }

            if (highChar.Length == 1)
            {
                highChar = "0" + highChar;
            }

            //如果不是中文字符（高位为0）则解码，否则加入Unicode头"&#x"和";"

            if (bOut[i] == 0)
            {
                unicodeString += Decode(highChar + lowChar);
            }
            else
            {
                unicodeString += UnicodeString(highChar + lowChar);
            }
        }

        return unicodeString.Trim();
    }

    public static string Decode(string inputString)
    {
        return decode(inputString);
    }

    private static string decode(string inputString)
    {
        string decodeString = "";
        string tempString = "";
        string finalreturn = "";

        string[] Anay_str = inputString.ToString().Split(';');
        for (int z = 0; z < Anay_str.Length; z++)
        {

            int pivo = Anay_str[z].ToString().IndexOf('&');

            if (pivo == 0)
            {
                decodeString = "";
                tempString = Anay_str[z].ToString().Replace("&#x", "");
                int iASCII = int.Parse(tempString, System.Globalization.NumberStyles.HexNumber);
                decodeString = decodeString + (char)iASCII;
            }
            else if (pivo > 0)
            {
                decodeString = "";
                string ordinaryString = Anay_str[z].ToString().Substring(0, pivo);
                tempString = Anay_str[z].ToString().Substring(pivo, Anay_str[z].ToString().Length - pivo);
                tempString = tempString.Replace("&#x", "");
                int iASCII = int.Parse(tempString, System.Globalization.NumberStyles.HexNumber);
                decodeString = decodeString + (char)iASCII;
                decodeString = ordinaryString + decodeString;

            }
            else if (pivo == -1)
            {
                decodeString = "";
                decodeString = Anay_str[z].ToString();

            }

            finalreturn += decodeString;

        }

        return finalreturn;
    }

    private static string UnicodeString(string inputString)
    {
        return "&#x" + inputString + ";";
    }
}

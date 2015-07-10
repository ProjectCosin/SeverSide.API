SERVERSIDE.API 这个REPO包含了所有与客户端交互API的源码，此系统基于Microsoft .Net Framework平台，以C#为开发语言，以WebService的方式对外公开接口。开发者可以基于此源码，直接使用或进行二次开发，架设在自己的服务器端，供任意平台的操控端进行调用。（本REPO中不含数据库）


下载源码后，请先将项目工程目录中 Web.Config文件里的数据库连接字符串，更换成您自己的数据库服务器连接字符串。详见：

<connectionStrings>
		<clear/>
		<add name="CENTRAL" connectionString="Data Source=.;Initial Catalog=DB_COS_CENTRAL;uid=;password=" providerName="System.Data.SqlClient"/>
		<add name="PROPERTY" connectionString="Data Source=.;Initial Catalog=DB_COS_PROPERTY;uid=;password=" providerName="System.Data.SqlClient"/>
</connectionStrings>

<appSettings>
        <clear/>
        <add key="DB_COS_CENTRAL_connstr" value="Data Source=.;Initial Catalog=DB_COS_CENTRAL;uid=;password=" />
        <add key="DB_COS_PROPERTY_connstr" value="Data Source=.;Initial Catalog=DB_COS_PROPERTY;uid=;password=" />
        <add key="CENTRAL" value="Data Source=.;Initial Catalog=DB_COS_CENTRAL;uid=;password="/>
        <add key="PROPERTY" value="Data Source=.;Initial Catalog=DB_COS_PROPERTY;uid=;password="/>
</appSettings>
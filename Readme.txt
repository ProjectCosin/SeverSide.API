SERVERSIDE.API ���REPO������������ͻ��˽���API��Դ�룬��ϵͳ����Microsoft .Net Frameworkƽ̨����C#Ϊ�������ԣ���WebService�ķ�ʽ���⹫���ӿڡ������߿��Ի��ڴ�Դ�룬ֱ��ʹ�û���ж��ο������������Լ��ķ������ˣ�������ƽ̨�Ĳٿض˽��е��á�����REPO�в������ݿ⣩


����Դ������Ƚ���Ŀ����Ŀ¼�� Web.Config�ļ�������ݿ������ַ��������������Լ������ݿ�����������ַ����������

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
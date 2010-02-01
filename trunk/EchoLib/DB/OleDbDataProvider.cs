using System;    
using System.Data;    
using System.Data.OleDb;

namespace com.echo.DB
{
    /// <summary></summary>    
    /// OleDbDataProvider 的摘要说明。    
    ///     
    internal class OleDbDataProvider : IDataProvider
    {

        private System.Data.OleDb.OleDbConnection oleDbConnection;
        private System.Data.OleDb.OleDbCommand oleDbCommand;
        private string connectionString;
        public OleDbDataProvider()
            : this(null)
        {
            //    
            // TODO: 在此处添加构造函数逻辑    
            //    
        }

        public OleDbDataProvider(string connectionString)
        {
            if (connectionString == null || connectionString.Trim() == string.Empty)
            {
                System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
                this.connectionString = (string)(configurationAppSettings.GetValue("ConnectionString", typeof(string)));
            }
            else
            {
                this.connectionString = connectionString;
            }
        }

        /// <summary></summary>    
        /// OleDb 连接字符串    
        ///     
        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }

        /// <summary></summary>    
        /// 返回一个带有连接字符串的OleDb Connection.    
        ///     
        /// <returns></returns>OracleConnection    
        private OleDbConnection GetOleDbConnection()
        {
            try
            {
                return new OleDbConnection(this.connectionString);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                return null;
            }
        }

        /// <summary></summary>    
        /// 对于 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数。对于其他所有类型的语句，返回值为 -1    
        ///     
        /// UPDATE、INSERT 和 DELETE 语句    
        public int ExecuteNonQuery(string sql)
        {
            using (oleDbConnection = this.GetOleDbConnection())
            {
                if (oleDbConnection == null)
                    return -1;
                try
                {
                    if (oleDbConnection.State == System.Data.ConnectionState.Closed)
                        oleDbConnection.Open();
                    oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    return oleDbCommand.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                    return -1;
                }
            }
        }

        /// <summary></summary>    
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为 .NET Framework 数据类型返回。忽略额外的列或行。    
        ///     
        /// SELECT 语句    
        /// <returns></returns>.NET Framework 数据类型形式的结果集第一行的第一列；如果结果集为空或结果为 REF CURSOR，则为空引用    
        public object ExecuteScalar(string sql)
        {
            using (oleDbConnection = this.GetOleDbConnection())
            {
                if (oleDbConnection == null)
                    return null;
                try
                {
                    if (oleDbConnection.State == System.Data.ConnectionState.Closed)
                        oleDbConnection.Open();
                    oleDbCommand = new OleDbCommand(sql, oleDbConnection);
                    return oleDbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                    return null;
                }
            }
        }

        /// <summary></summary>    
        ///  执行单Sql语句查询，并将查询返回的结果作为一个数据集返回    
        ///     
        /// SELECT 语句    
        /// <returns></returns>数据集 DataSet    
        public DataSet RetriveDataSet(string sql)
        {
            if (sql == null || sql == string.Empty)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("sql 为空");
#endif
                return null;
            }
            using (oleDbConnection = this.GetOleDbConnection())
            {
                if (oleDbConnection == null)
                    return null;
                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, oleDbConnection))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                    }
                    return ds;
                }
            }
        }

        public DataSet UpdateDataSet(string sql, DataSet hasChangesDataSet)
        {
            return null;
        }

        public void Dispose()
        {
            this.connectionString = null;
            this.oleDbCommand.Dispose();
            this.oleDbConnection.Dispose();
        }
    }
}
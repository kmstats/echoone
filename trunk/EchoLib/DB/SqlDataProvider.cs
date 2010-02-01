using System;   
using System.Data;   
using System.Data.SqlClient;

namespace com.echo.DB
{
    /// <summary>   
    /// SqlDataProvider 的摘要说明。   
    /// </summary>   
    internal class SqlDataProvider : IDataProvider
    {
        private System.Data.SqlClient.SqlConnection sqlConnection;
        private System.Data.SqlClient.SqlCommand sqlCommand;
        private string connectionString;
        public SqlDataProvider()
            : this(null)
        {
            //   
            // TODO: 在此处添加构造函数逻辑   
            //   
        }

        public SqlDataProvider(string connectionString)
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

        /// <summary>   
        /// SQL 连接字符串   
        /// </summary>   
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

        /// <summary>   
        /// 返回一个带有连接字符串的SQL Connection.   
        /// </summary>   
        /// <returns>OracleConnection</returns>   
        private SqlConnection GetSqlConnection()
        {
            try
            {
                return new SqlConnection(this.connectionString);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                return null;
            }
        }

        /// <summary>   
        /// 对于 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数。对于其他所有类型的语句，返回值为 -1   
        /// </summary>   
        /// <param name="Sql">UPDATE、INSERT 和 DELETE 语句</param>   
        public int ExecuteNonQuery(string sql)
        {
            using (sqlConnection = this.GetSqlConnection())
            {
                if (sqlConnection == null)
                    return -1;
                try
                {
                    if (sqlConnection.State == System.Data.ConnectionState.Closed)
                        sqlConnection.Open();
                    sqlCommand = new SqlCommand(sql, sqlConnection);
                    return sqlCommand.ExecuteNonQuery();
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

        /// <summary>   
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为 .NET Framework 数据类型返回。忽略额外的列或行。   
        /// </summary>   
        /// <param name="sql">SELECT 语句</param>   
        /// <returns>.NET Framework 数据类型形式的结果集第一行的第一列；如果结果集为空或结果为 REF CURSOR，则为空引用</returns>   
        public object ExecuteScalar(string sql)
        {
            using (sqlConnection = this.GetSqlConnection())
            {
                if (sqlConnection == null)
                    return null;
                try
                {
                    if (sqlConnection.State == System.Data.ConnectionState.Closed)
                        sqlConnection.Open();
                    sqlCommand = new SqlCommand(sql, sqlConnection);
                    return sqlCommand.ExecuteScalar();
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

        /// <summary>   
        ///  执行单Sql语句查询，并将查询返回的结果作为一个数据集返回   
        /// </summary>   
        /// <param name="sql">SELECT 语句</param>   
        /// <returns>数据集 DataSet</returns>    
        public DataSet RetriveDataSet(string sql)
        {
            if (sql == null || sql == string.Empty)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("sql 为空");
#endif
                return null;
            }
            using (sqlConnection = this.GetSqlConnection())
            {
                if (sqlConnection == null)
                    return null;
                using (SqlDataAdapter da = new SqlDataAdapter(sql, sqlConnection))
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
            this.sqlCommand.Dispose();
            this.sqlConnection.Dispose();
        }
    }
}
  

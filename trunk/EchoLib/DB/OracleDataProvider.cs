using System;
using System.Data;
using System.Data.OracleClient;

namespace com.echo.DB
{
    /// <summary></summary>    
    /// OracleDataProvider 的摘要说明。    
    ///     
    internal class OracleDataProvider : IDataProvider
    {
        private System.Data.OracleClient.OracleConnection oracleConnection;
        private System.Data.OracleClient.OracleCommand oracleCommand;
        private string connectionString;
        public OracleDataProvider() : this(null)
        {
            //    
            // TODO: 在此处添加构造函数逻辑    
            //    
        }

        public OracleDataProvider(string connectionString)
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
        /// Oracle 连接字符串 "User Id=southfence;Data Source=FENCEORA;Password=southfence;Persist Security Info=true;"        
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
        /// 返回一个带有连接字符串的Oracle Connection.    
        ///     
        /// <returns></returns>OracleConnection    
        private OracleConnection GetOracleConnection()
        {
            try
            {
                return new OracleConnection(this.connectionString);
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
            using (oracleConnection = this.GetOracleConnection())
            {
                if (oracleConnection == null)
                    return -1;
                int rv = -1;
                OracleTransaction oracleTransaction = null;
                try
                {
                    if (oracleConnection.State == System.Data.ConnectionState.Closed)
                        oracleConnection.Open();
                    oracleCommand = new OracleCommand(sql, oracleConnection);
                    oracleTransaction = oracleConnection.BeginTransaction();
                    oracleCommand.Transaction = oracleTransaction;
                    rv = oracleCommand.ExecuteNonQuery();
                    oracleTransaction.Commit();
                }
                catch (Exception ex)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
#endif
                    oracleTransaction.Rollback();
                    rv = -1;
                }

                return rv;
            }
        }

        /// <summary></summary>    
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为 .NET Framework 数据类型返回。忽略额外的列或行。    
        ///     
        /// SELECT 语句    
        /// <returns></returns>.NET Framework 数据类型形式的结果集第一行的第一列；如果结果集为空或结果为 REF CURSOR，则为空引用    
        public object ExecuteScalar(string sql)
        {
            using (oracleConnection = this.GetOracleConnection())
            {
                if (oracleConnection == null)
                    return null;
                try
                {
                    if (oracleConnection.State == System.Data.ConnectionState.Closed)
                        oracleConnection.Open();
                    oracleCommand = new OracleCommand(sql, oracleConnection);
                    return oracleCommand.ExecuteScalar();
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
            using (oracleConnection = this.GetOracleConnection())
            {
                if (oracleConnection == null)
                    return null;
                using (OracleDataAdapter da = new OracleDataAdapter(sql, oracleConnection))
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

        /// <summary></summary>    
        /// 更新数据集.     
        /// 过程:客户层(dataSet.GetChanges()) -- 修改 --> 数据服务层(hasChangesDataSet.update()) -- 更新--> 数据层(hasChangesDataSet) ...    
        ///  数据层(hasChangesDataSet) -- 新数据 --> 数据服务层 (hasChangesDataSet) -- 合并 -- > 客户层(dataSet.Merge(hasChangesDataSet))    
        ///     
        ///     
        /// <returns></returns>    
        public DataSet UpdateDataSet(string sql, DataSet hasChangesDataSet)
        {
            return null;
        }

        /// <summary></summary>    
        ///  执行Sql数组语句查询，并将查询返回的结果作为一个数据读取器返回    
        ///     
        ///     
        /// <returns></returns>OracleDataReader    
        public OracleDataReader RetriveDataReader(string sql)
        {
            if (sql == null || sql == string.Empty)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("sql 为空");
#endif
                return null;
            }
            using (oracleConnection = this.GetOracleConnection())
            {
                if (oracleConnection == null)
                    return null;
                using (oracleCommand = new OracleCommand(sql, oracleConnection))
                {
                    try
                    {
                        OracleDataReader oracleDataReader = oracleCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        return oracleDataReader;
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
        }

        public void Dispose()
        {
            this.connectionString = null;
            this.oracleCommand.Dispose();
            this.oracleConnection.Dispose();
        }
    }
}
using System;

namespace com.echo.DB
{
    public interface IDataProvider
    {
        /// <summary></summary>    
        /// 执行 UPDATE、INSERT 和 DELETE 语句，返回值为该命令所影响的行数    
        ///       
        int ExecuteNonQuery(string sql);

        /// <summary></summary>    
        /// 执行查询，并将查询返回的结果集中第一行的第一列作为 .NET Framework 数据类型返回。忽略额外的列或行    
        ///       
        object ExecuteScalar(string sql);

        
        /// <summary></summary>    
        ///  执行单Sql语句查询，并将查询返回的结果作为一个数据集返回    
        ///       
        System.Data.DataSet RetriveDataSet(string sql);

        /// <summary></summary>    
        /// 更新库    
        ///     
        ///     
        ///     
        /// <returns></returns>    
        System.Data.DataSet UpdateDataSet(string sql, System.Data.DataSet hasChangesDataSet);

        /// <summary></summary>    
        /// 执行Dispose    
        ///     
        void Dispose();
    }   
}

using System;

namespace com.echo.DB
{
    /// <summary></summary>    
    /// 提供对数据库访问的通用类。    
    ///     
    public class DataProvider 
    {
        /// <summary></summary>    
        /// 数据库枚举类型    
        ///     
        public enum DataProviderType
        {
            OdbcDataProvider = 0,
            OleDbDataProvider = 1,
            OracleDataProvider = 2,
            SqlDataProvider = 3,
            KingbaseProvider = 4
        }

        /// <summary></summary>    
        /// 建立访问数据库的实例    
        ///     
        /// 数据库枚举类型    
        /// <returns></returns>    
        public static IDataProvider CreateDataProvider(DataProviderType dataProviderType)
        {
            switch (dataProviderType)
            {
                case DataProviderType.OdbcDataProvider:
                    return new OdbcDataProvider();
                case DataProviderType.OleDbDataProvider:
                    return new OleDbDataProvider();
                case DataProviderType.OracleDataProvider:
                    return new OracleDataProvider();
                case DataProviderType.SqlDataProvider:
                    return new SqlDataProvider();
                case DataProviderType.KingbaseProvider:
                    return new OdbcDataProvider();
                default:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("dataProviderType 类型不存在!");
#endif
                    return null;
            }
        }
    }    
}

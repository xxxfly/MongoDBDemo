using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDBDAL
{
    public class MongoDBConnect
    {
        public MongoDBConnect(string host, string dbName)
        {
            this.MONGO_CONN_HOST = host;
            this.DB_NAME = dbName;
        }

        /// <summary>  
        /// 数据库所在主机  
        /// </summary>  
        private readonly string MONGO_CONN_HOST;

        /// <summary>  
        /// 数据库所在主机的端口  
        /// </summary>  
        private readonly int MONGO_CONN_PORT = 27019;

        /// <summary>  
        /// 连接超时设置 秒  
        /// </summary>  
        private readonly string CONNECT_TIME_OUT;

        /// <summary>  
        /// 数据库的名称  
        /// </summary>  
        private readonly string DB_NAME;

        /// <summary>  
        /// 数据库用户名
        /// </summary>  
        private readonly string MONGO_CONN_UserName = "chfl";
        /// <summary>  
        /// 数据库密码
        /// </summary>  
        private readonly string MONGO_CONN_PWD = "chfl";

        public IMongoDatabase GetDataBase()
        {
            var settings = new MongoClientSettings
            {
                Credentials = new[] { MongoCredential.CreateMongoCRCredential(DB_NAME, MONGO_CONN_UserName, MONGO_CONN_PWD) },
                Server = new MongoServerAddress(CONNECT_TIME_OUT, MONGO_CONN_PORT),
                ConnectTimeout = TimeSpan.FromSeconds(1800),
                MaxConnectionIdleTime = TimeSpan.FromSeconds(1800)
            };

            MongoClient mongoClient = new MongoClient(settings);
            //var mongoClient = new MongoClient("mongodb://chfl:chfl@192.168.1.70:27019/db1"); 

            IMongoDatabase database = mongoClient.GetDatabase(DB_NAME);

            return database;
        }
    }
}

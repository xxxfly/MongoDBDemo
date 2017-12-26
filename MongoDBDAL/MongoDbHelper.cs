using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Reflection;

namespace MongoDBDAL
{
    /// <summary>  
    /// Mongo db的数据库帮助类  
    /// </summary>  
    public class MongoDbHelper
    {

        /// <summary>  
        /// 数据库的实例  
        /// </summary>  
        private IMongoDatabase _db;

        /// <summary>  
        /// ObjectId的键  
        /// </summary>  
        private readonly string OBJECTID_KEY = "_id";

        public MongoDbHelper(string host, string dbName)
        {
            this._db = new MongoDBConnect(host, dbName).GetDataBase();
        }

        public MongoDbHelper(IMongoDatabase db)
        {
            this._db = db;
        }



        #region 插入数据
        /// <summary>  
        /// 将数据插入进数据库  
        /// </summary>  
        /// <typeparam name="T">需要插入数据的类型</typeparam>  
        /// <param name="t">需要插入的具体实体</param>  
        public bool Insert<T>(T t)
        {
            //集合名称  
            string collectionName = typeof(T).Name;
            return Insert<T>(t, collectionName);
        }
        /// <summary>  
        /// 将数据插入进数据库  
        /// </summary>  
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>  
        /// <param name="t">需要插入数据库的具体实体</param>  
        /// <param name="collectionName">指定插入的集合</param>  
        public bool Insert<T>(T t, string collectionName)
        {
            if (this._db == null)
            {
                return false;
            }
            try
            {
                IMongoCollection<BsonDocument> mc = this._db.GetCollection<BsonDocument>(collectionName);
                //将实体转换为bson文档  
                BsonDocument bd = t.ToBsonDocument();
                //进行插入操作  
                mc.InsertOne(bd);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>  
        /// 批量插入数据  
        /// </summary>  
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>  
        /// <param name="list">需要插入数据的列表</param>  
        public bool Insert<T>(List<T> list)
        {
            //集合名称  
            string collectionName = typeof(T).Name;
            return this.Insert<T>(list, collectionName);
        }
        /// <summary>  
        /// 批量插入数据  
        /// </summary>  
        /// <typeparam name="T">需要插入数据库的实体类型</typeparam>  
        /// <param name="list">需要插入数据的列表</param>  
        /// <param name="collectionName">指定要插入的集合</param>  
        public bool Insert<T>(List<T> list, string collectionName)
        {
            if (this._db == null)
            {
                return false;
            }
            try
            {
                IMongoCollection<BsonDocument> mc = this._db.GetCollection<BsonDocument>(collectionName);
                //创建一个空间bson集合  
                List<BsonDocument> bsonList = new List<BsonDocument>();
                //批量将数据转为bson格式 并且放进bson文档  
                list.ForEach(t => bsonList.Add(t.ToBsonDocument()));
                //批量插入数据  
                mc.InsertMany(bsonList);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion


        #region 查询数据

        public void FindOne(string id, string collectionName)
        {
            if (this._db == null)
            {
                return;
            }
            IMongoCollection<BsonDocument> mc = this._db.GetCollection<BsonDocument>(collectionName);


        }

        #endregion

    }
}

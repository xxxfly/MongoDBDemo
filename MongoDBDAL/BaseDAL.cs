using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;
using System.Collections;

namespace MongoDBDAL
{
    public partial class BaseDAL<T> where T : BaseEntity, new()
    {
        #region 属性字段


        #endregion

        #region 构造函数及基础方法
        /// <summary>
        /// 数据库对象
        /// </summary>
        /// <returns></returns>
        protected virtual IMongoDatabase CreateDatabase()
        {
            string MongoDB_Host = "192.168.1.70";
            string MongoDB_DbName = "db1";
            string MongoDB_UserName = "chfl";
            string MongoDB_Password = "chfl";

            var settings = new MongoClientSettings
            {
                Credentials = new[] { MongoCredential.CreateMongoCRCredential(MongoDB_DbName, MongoDB_UserName, MongoDB_Password) },
                Server = new MongoServerAddress(MongoDB_Host, 27019),
                ConnectTimeout = TimeSpan.FromSeconds(1800),
                MaxConnectionIdleTime = TimeSpan.FromSeconds(1800)
            };

            MongoClient mongoClient = new MongoClient(settings);
            //var mongoClient = new MongoClient("mongodb://chfl:chfl@192.168.1.70:27019/db1"); 

            IMongoDatabase database = mongoClient.GetDatabase(MongoDB_DbName);

            return database;
        }
        /// <summary>
        /// 获取操作对象的IMongoCollection集合，强类型对象集合
        /// </summary>
        /// <returns></returns>
        protected virtual IMongoCollection<T> GetCollection()
        {

            IMongoDatabase database = CreateDatabase();
            return database.GetCollection<T>("colTest1");
        }

        #endregion

        #region 查询单个对象
        /// <summary>
        /// 查询数据库，检查是否存在指定ID对象
        /// </summary>
        /// <param name="id">对象ID值</param>
        /// <returns>存在则返回指定对象，否则返回NuLL</returns>
        public virtual T FindByID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            IMongoCollection<T> collection = GetCollection();
            return collection.Find(m => m.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// 根据条件查询数据库，如果存在则返回第一个对象
        /// </summary>
        /// <param name="filter">条件表达式</param>
        /// <returns>存在则返回第一个对象，否则返回默认值</returns>
        public virtual T FindSingle(FilterDefinition<T> filter)
        {
            IMongoCollection<T> collection = GetCollection();
            return collection.Find(filter).FirstOrDefault();
        }
        /// <summary>
        /// 查询数据库，检查是否存在指定ID的对象（异步）
        /// </summary>
        /// <param name="id">对象的ID值</param>
        /// <returns>存在则返回指定对象，否则返回Null</returns>
        public virtual async Task<T> FindByIDAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            IMongoCollection<T> collection = GetCollection();
            return await collection.FindAsync(m => m.Id == id).Result.FirstOrDefaultAsync();
        }

        #endregion

        #region GetQueryable几种方式
        /// <summary>
        /// 返回可查询的记录源
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        public virtual IFindFluent<T, T> GetQueryable(FilterDefinition<T> query)
        {
            return GetQueryable(query, "", true);
        }

        /// <summary>
        /// 根据条件表达式返回可查询的记录源
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sortPropertyName"></param>
        /// <param name="isDescending"></param>
        /// <returns></returns>
        public virtual IFindFluent<T, T> GetQueryable(FilterDefinition<T> query, string sortPropertyName, bool isDescending = true)
        {
            IMongoCollection<T> collection = GetCollection();
            IFindFluent<T, T> queryable = collection.Find(query);

            var sort = isDescending ? Builders<T>.Sort.Descending(sortPropertyName) : Builders<T>.Sort.Ascending(sortPropertyName);
            return queryable.Sort(sort);
        }

        /// <summary>
        /// 返回可查询的记录值
        /// </summary>
        /// <returns></returns>
        //public virtual IQueryable<T> GetQueryable()
        //{
        //    IMongoCollection<T> collection = GetCollection();
        //    IQueryable<T> query = collection.AsQueryable();
        //    return query.OrderBy("", true);
        //}

        /// <summary>
        /// 根据条件表达式返回可查询的记录值
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="match">查询条件</param>
        /// <param name="orderByProperty">排序表达式</param>
        /// <param name="isDescending">如果为true则为降序，否则为升序</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetQueryable<TKey>(Expression<Func<T, bool>> match, Expression<Func<T, TKey>> orderByProperty, bool isDescending = true)
        {
            IMongoCollection<T> collection = GetCollection();
            IQueryable<T> query = collection.AsQueryable();

            if (match != null)
            {
                query = query.Where(match);
            }
            if (orderByProperty != null)
            {
                query = isDescending ? query.OrderByDescending(orderByProperty) : query.OrderBy(orderByProperty);
            }
            else
            {
                //query = query.OrderBy(orderByProperty, isDescending);
            }

            return query;
        }

        #endregion

        #region 查询集合

        #endregion

        #region 对象添加、修改、删除
        /// <summary>
        /// 插入指定对象到数据库中
        /// </summary>
        /// <param name="t">指定的对象</param>
        public virtual void Insert(T t)
        {
            if (t == null)
            {
                return;
            }

            IMongoCollection<T> collection = GetCollection();
            collection.InsertOne(t);
        }
        /// <summary>
        /// 插入指定对象到数据库中（异步）
        /// </summary>
        /// <param name="t">指定对象</param>
        /// <returns></returns>
        public virtual async Task InsertAsync(T t)
        {
            if (t == null)
            {
                return;
            }

            IMongoCollection<T> collection = GetCollection();
            await collection.InsertOneAsync(t);

        }
        /// <summary>
        /// 插入指定对象集合到数据库中
        /// </summary>
        /// <param name="list">指定的对象集合</param>
        public virtual void InsertBatch(IEnumerable<T> list)
        {
            if (list == null)
            {
                return;
            }

            IMongoCollection<T> collection = GetCollection();
            collection.InsertMany(list);

        }
        /// <summary>
        /// 插入指定对象集合到数据库中
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public virtual async Task InsertBatchAsync(IEnumerable<T> list)
        {
            if (list == null)
            {
                return;
            }

            IMongoCollection<T> collection = GetCollection();
            await collection.InsertManyAsync(list);
        }
        /// <summary>
        /// 跟新对象属性到数据库中
        /// </summary>
        /// <param name="t"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual bool Update(T t, string id)
        {
            if (t == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            bool result = false;

            IMongoCollection<T> collection = GetCollection();
            //使用IsUpdate=true，如果没有记录则写入
            var update = collection.ReplaceOne(m => m.Id == id, t, new UpdateOptions() { IsUpsert = true });
            result = update != null && update.MatchedCount > 0;

            return result;
        }
        /// <summary>
        /// 封装处理更新的操作（部分字段更新）
        /// </summary>
        /// <param name="id">主键的值</param>
        /// <param name="update">更新对象</param>
        /// <returns>执行成功返回<c>true</c>,否则为<c>false</c></returns>
        public virtual bool Update(string id, UpdateDefinition<T> update)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }
            if (update == null)
            {
                return false;
            }
            IMongoCollection<T> collection = GetCollection();
            var result = collection.UpdateOne(m => m.Id == id, update, new UpdateOptions() { IsUpsert = true });
            return result != null && result.ModifiedCount > 0;
        }
        /// <summary>
        /// 封装处理更新的操作（部分字段更新）（异步）
        /// </summary>
        /// <param name="id">主键的值</param>
        /// <param name="update">更新对象</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateAsync(string id, UpdateDefinition<T> update)
        {
            if (update == null)
            {
                return false;
            }
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            IMongoCollection<T> collection = GetCollection();
            var result = await collection.UpdateOneAsync(m => m.Id == id, update, new UpdateOptions() { IsUpsert = true });

            var success = result != null && result.ModifiedCount > 0;
            return await Task.FromResult(success);
        }

        /// <summary>
        /// 根据制定对象的ID,从数据库中删除指定对象
        /// </summary>
        /// <param name="id">对象的ID</param>
        /// <returns></returns>
        public virtual bool Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            IMongoCollection<T> collection = GetCollection();
            var result = collection.DeleteOne(m => m.Id == id);
            return result != null && result.DeletedCount > 0;
        }
        /// <summary>
        /// 根据指导对象的ID,从数据库中删除指定的对象
        /// </summary>
        /// <param name="idList">对象的ID集合</param>
        /// <returns></returns>
        public virtual bool DeleteBetch(List<string> idList)
        {
            if (idList.Count == 0 || idList == null)
            {
                return false;
            }

            IMongoCollection<T> collection = GetCollection();
            var result = collection.DeleteMany(m => idList.Contains(m.Id));
            return result != null && result.DeletedCount > 0;
        }
        /// <summary>
        /// 根据指定条件，从数据库中删除指定对象
        /// </summary>
        /// <param name="match">条件表达式</param>
        /// <returns></returns>
        public virtual bool DeleteByExpression(Expression<Func<T, bool>> match)
        {
            IMongoCollection<T> collection = GetCollection();
            collection.AsQueryable().Where(match).ToList().ForEach(m => collection.DeleteOne(t => t.Id == m.Id));
            return true;
        }
        /// <summary>
        /// 根据指定条件，从数据库中删除指定对象
        /// </summary>
        /// <param name="query">条件表达式</param>
        /// <returns></returns>
        public virtual bool DeleteByQuery(FilterDefinition<T> query)
        {
            IMongoCollection<T> collection = GetCollection();
            var result = collection.DeleteMany(query);
            return result != null && result.DeletedCount > 0;
        }
        #endregion

        #region 其他接口

        #endregion
    }
}

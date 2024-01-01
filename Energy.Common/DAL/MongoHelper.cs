using Energy.Common.Utils;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.DAL
{
    public class MongoHelper
    {
        private static MongoHelper uniqueInstance;

        private static readonly object locker = new object();

        public static MongoClient m_MongoClient = null;

        public IMongoDatabase m_MongoDatabase = null;


        private MongoHelper()
        {
            string conn = SettingsHelper.GetSettingValue("MongoString");
            if (string.IsNullOrEmpty(conn) || conn == "")
            {
                conn = "mongodb://127.0.0.1:27017";
            }
            m_MongoClient = new MongoClient(conn);

            m_MongoDatabase = m_MongoClient.GetDatabase("History");
        }

        public static MongoHelper GetInstance()
        {
            if (uniqueInstance == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new MongoHelper();
                    }
                }
            }
            return uniqueInstance;
        }

      

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
           
            return m_MongoDatabase.GetCollection<BsonDocument>(collectionName);
            
        }


        /// <summary>
        /// 数据集插入一条数据
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newDocument"></param>
        public void InsertOne(IMongoCollection<BsonDocument> collection, BsonDocument newDocument)
        {
            
            collection.InsertOne(newDocument);
        }

        public void CreateIndexes(IMongoCollection<BsonDocument> collection,string name)
        {
            var indexes =collection.Indexes.List().ToList();

            bool flag = false;
            
            foreach (var item in indexes)
            {
                var tempList = item.ToList();
                foreach (var element in tempList)
                {
                    if (element.Name == name)
                        flag = true;
                }
            }

            if (!flag)
            {
                var indexOptions = new CreateIndexOptions();
                //var indexModel = new CreateIndexModel(,);
            }
          
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="newDocuments"></param>
        public void InsertMany(IMongoCollection<BsonDocument> collection, List<BsonDocument> newDocuments)
        {
            collection.InsertManyAsync(newDocuments);
        }

        /// <summary>
        /// 找到所有
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public MongoDB.Driver.Linq.IMongoQueryable FindAll(IMongoCollection<BsonDocument> collection)
        {
            return collection.AsQueryable<BsonDocument>();
        }

        /// <summary>
        /// 找到一条
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public IFindFluent<BsonDocument, BsonDocument> Find(IMongoCollection<BsonDocument> collection, FilterDefinition<BsonDocument> filter)
        {
            return collection.Find(filter);
        }

        /// <summary>
        /// 找到并更新
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public BsonDocument FindOneAndUpdate(IMongoCollection<BsonDocument> collection, FilterDefinition<BsonDocument> filter, UpdateDefinition<BsonDocument> update)
        {
            return collection.FindOneAndUpdate(filter, update);
        }

        /// <summary>
        /// 找到并删除
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public BsonDocument FindOneAndDelete(IMongoCollection<BsonDocument> collection, FilterDefinition<BsonDocument> filter)
        {
            return collection.FindOneAndDelete(filter);
        }
    }
}

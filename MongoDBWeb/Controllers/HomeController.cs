using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;
using System.Text;

namespace MongoDBWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void MongoDBTest()
        {
            string MongoDB_Host = ConfigurationManager.AppSettings["MongoDB_Host"];
            string MongoDB_DbName = ConfigurationManager.AppSettings["MongoDB_DbName"];
            string MongoDB_UserName = ConfigurationManager.AppSettings["MongoDB_UserName"];
            string MongoDB_Password = ConfigurationManager.AppSettings["MongoDB_Password"];

            var settings = new MongoClientSettings
            {
                Credentials = new[] { MongoCredential.CreateMongoCRCredential("db1", "chfl", "chfl") },
                Server = new MongoServerAddress("192.168.1.70", 27019),
                ConnectTimeout = TimeSpan.FromSeconds(1800),
                MaxConnectionIdleTime = TimeSpan.FromSeconds(1800)
            };

            var mongoClient = new MongoClient(settings);
            //var mongoClient = new MongoClient("mongodb://chfl:chfl@192.168.1.70:27019/db1"); 
            var mongoServer = mongoClient.GetServer();
            var database = mongoServer.GetDatabase("db1");

            MongoCollection<BsonDocument> testCol = database.GetCollection<BsonDocument>("colTest1");
            MongoCursor<BsonDocument> Cursor = testCol.FindAll();

            foreach (BsonDocument item in Cursor)
            {
                var ssssss = item.ToJson();
            }
        }

        public string TestUncodingString(string str)
        {
            str = "&#x6D4B;&#x8BD5;";
            string s = get_utf8(str);
            return s;

        }

        /// <summary>
        /// utf-8编码转字符串
        /// </summary>
        /// <param name="unicodeStr"></param>
        /// <returns></returns>
        public static string get_utf8(string unicodeStr)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeStr);
            string decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }


        static string AddJson()
        {
            var id = Guid.NewGuid().ToString();
            JObject obj = new JObject();
            obj["name"] = "test2";
            obj["age"] = 23;
            obj["sex"] = "0";
            obj["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            BsonDocument document = BsonDocument.Parse(JsonConvert.SerializeObject(obj));
            new MongoDbService().Add("db1", "colTest2", document);
            return "";
        }

        static string AddJsonList()
        {
            List<BsonDocument> list = new List<BsonDocument>();
            for (int i = 0; i < 100000; i++)
            {
                JObject obj = new JObject();
                obj["name"] = "test" + (i + 10);
                obj["age"] = 20 + i;
                obj["sex"] = "0";
                obj["CreateTime"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                BsonDocument document = BsonDocument.Parse(JsonConvert.SerializeObject(obj));
                list.Add(document);
            }

            new MongoDbService().BatchAdd("db1", "colTest2", list);

            return "";
        }

        static void GetOne()
        {

            var s1 = new MongoDbService().List<BsonDocument>("db1", "colTest2", m => true, m => m, 100000);

            foreach (BsonDocument item in s1)
            {

            }
        }
    }
}
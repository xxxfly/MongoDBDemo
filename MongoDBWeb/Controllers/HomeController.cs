using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Configuration;

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
    }
}
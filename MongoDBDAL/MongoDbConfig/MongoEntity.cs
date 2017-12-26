using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBDAL.MongoDbConfig
{
    public class MongoEntity
    {
        public MongoEntity()
        {
            _id = Guid.NewGuid().ToString("N");
        }

        public string _id { get; set; }
    }
}

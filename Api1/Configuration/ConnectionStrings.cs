using System.Data.SqlClient;

namespace SBPWebApi.Configuration
{
    public class ConnectionStrings
    {
        public string ApiDb { get; set; }

        public string Aws { get; set; }

        public string Awsmdp { get; set; }

        public string Api2Db { get; set; }
    }
}
using System.Data.SqlClient;

namespace SBPWebApi.Configuration
{
    public class ConnectionStrings
    {
        public ConnectionStrings()
        {
            SqlCon = new SqlConnectionStringBuilder()
                {
                    DataSource = "localhost",
                    InitialCatalog = "goodtime",
                    UserID = "root",
                    Password = "root"
                }.ConnectionString;
        }

        public string SqlCon;

        public string ApiDb { get; set; }

        //public string DBLocal = "Data Source=MSI\\MSIDAMIAN;Initial Catalog=PAESGI5;Integrated Security = True; Connect Timeout = 30; Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public string DBLocal = "Server=tcp:sbp-esgi.database.windows.net,1433;Initial Catalog=SBPDB;Persist Security Info=False;User ID=yanderu;Password=damianXXL78;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public string Api2Db { get; set; }
    }
}
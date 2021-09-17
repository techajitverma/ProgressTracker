using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace ProgressTrackerDataAccess
{
    public class ConnectionManager
    {
        private static readonly ConnectionManager instance = new ConnectionManager();
        private readonly SqlConnection con = new SqlConnection("Data Source=(Local);initial catalog=ProgressTracker;Integrated Security=True");
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ConnectionManager()
        {
        }

        private ConnectionManager()
        {
        }

        public static ConnectionManager Instance
        {
            get
            {
                return instance;
            }
        }

        public SqlConnection GetDBConnection()
        {
            return con;
        }

        public void OpenConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        public void CloseConnection(SqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
    }
}

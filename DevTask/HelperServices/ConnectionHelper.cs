namespace DevTask.HelperServices
{
    public static class ConnectionHelper
    {
        public static string getConnectionString()
        {
            string MYCONNECTIONSTRING = "";
            if (Environment.GetEnvironmentVariable("PGHOST") != null)
            {
                MYCONNECTIONSTRING =
                    $"Server={Environment.GetEnvironmentVariable("PGHOST")};" +
                    $"Database={Environment.GetEnvironmentVariable("DATABASE_URL")};" +
                    $"Port={Environment.GetEnvironmentVariable("PGPORT")};" +
                    $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};" +
                    $"Password={Environment.GetEnvironmentVariable("PGPASSWORD")}";
            }
            else
            {
                MYCONNECTIONSTRING = "";
            }
            return MYCONNECTIONSTRING;
        }
    }
}

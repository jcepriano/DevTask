namespace DevTask.HostingHelpers
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
                MYCONNECTIONSTRING = "Server=localhost;Database=MYDATABASE;Port=5432;Username=postgres;Password=password123";
            }
            return MYCONNECTIONSTRING;
        }
    }
}

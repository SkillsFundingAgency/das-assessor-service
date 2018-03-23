using System;
using System.Data.SqlClient;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers
{
    public class SqlDatabaseConncetionHelper
    {
        public static int ExecuteSqlCommand(String queryToExecute, String connectionString)
        {
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            databaseConnection.Open();
            SqlCommand command = new SqlCommand(queryToExecute, databaseConnection);
            int result = command.ExecuteNonQuery();
            databaseConnection.Close();
            return result;
        }

        public static SqlDataReader ReadDataFromDataBase(String queryToExecute, String connectionString)
        {
            SqlDataReader dataReader = null;
            SqlConnection databaseConnection = new SqlConnection(connectionString);
            databaseConnection.Open();
            SqlCommand command = new SqlCommand(queryToExecute, databaseConnection);
            dataReader = command.ExecuteReader();
            databaseConnection.Close();
            return dataReader;
        }
    }
}
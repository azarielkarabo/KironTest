using Microsoft.Data.SqlClient;
using System.Data;

namespace KironTest.Services.Database
{
    public class DBConnectionManagerService : IDisposable
    {
        private readonly SqlConnection _connection;
        private SqlTransaction _transaction;

        public DBConnectionManagerService(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("Default")!;
            _connection = new SqlConnection(connectionString);
        }

        public void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void BeginTransaction()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _transaction = _connection.BeginTransaction();
            }
            else
            {
                throw new InvalidOperationException("Connection must be open to start a transaction.");
            }
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
            _transaction = null;
        }

        public void RollbackTransaction()
        {
            _transaction?.Rollback();
            _transaction = null;
        }

        public T ExecuteStoredProcedure<T>(string storedProcedureName, SqlParameter[] parameters, Func<SqlDataReader, T> deserializeFunction)
        {
            using (var command = new SqlCommand(storedProcedureName, _connection, _transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return deserializeFunction(reader); // Deserialize into the model
                    }
                    return default;
                }
            }
        }

        public void ExecuteNonQuery(string storedProcedureName, SqlParameter[] parameters)
        {
            using (var command = new SqlCommand(storedProcedureName, _connection, _transaction))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            CloseConnection();
        }
    }
}

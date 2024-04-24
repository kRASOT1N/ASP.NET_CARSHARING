using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class CarSharingDatabase
{
    private readonly string _connectionString;

    public CarSharingDatabase(string databaseName)
    {
        _connectionString = $"Data Source={databaseName};Version=3;";
    }

    public List<Dictionary<string, object>> GetCarsData()
    {
        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

        using (var connection = new SQLiteConnection(_connectionString))
        {
            connection.Open();
            string sql = "SELECT * FROM Cars";
            using (var command = new SQLiteCommand(sql, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var carData = new Dictionary<string, object>
                    {
                        { "ID", reader.GetInt32(0) },
                        { "Model", reader.GetString(1) },
                        { "Year", reader.GetInt32(2) },
                        { "Category", reader.GetString(3) },
                        { "Status", reader.GetString(4) }
                    };
                    data.Add(carData);
                }
            }
        }

        return data;
    }
}

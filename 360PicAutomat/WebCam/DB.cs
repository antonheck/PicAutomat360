using System;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace PicAutomat
{
    public class DB
    {
        private SqliteConnection _connection = new SqliteConnection();
        private string _dbFilePath;

        public DB(string IN_DbFileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(IN_DbFileName))
                {
                    var tmpEx = new Exception("Der Dateiname ist ungültig.");
                    throw tmpEx;
                }
                _Close();

                _dbFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, IN_DbFileName);

                var tmpConnectionString = @"Data Source=" + _dbFilePath;
                _connection.ConnectionString = tmpConnectionString;

                if (!File.Exists(_dbFilePath))
                {
                    _CreateSchema();
                }
                else
                {
                    _Open();
                }
            }
            catch (Exception IN_Ex)
            {
                Debug.Write(IN_Ex.Message);
            }
        }

        async public Task Write(string IN_FileName, string IN_TimeStamp)
        {
            _Open();

            SqliteCommand tmpMainTable = new SqliteCommand();
            tmpMainTable.Connection = _connection;

            var tmpFileName = new SqliteParameter("@Name", IN_FileName);
            var tmpTimeStamp = new SqliteParameter("@TimeStamp", IN_TimeStamp);

            tmpMainTable.CommandText = "INSERT INTO [MainTable] ([Name],[TimeStamp]) VALUES(@Name,@TimeStamp)";
            SqliteParameter[] tmpRateCardTableColumns = { tmpFileName, tmpTimeStamp };
            tmpMainTable.Parameters.AddRange(tmpRateCardTableColumns);

            await tmpMainTable.ExecuteNonQueryAsync();

            _Close();
        }

        async public Task<ObservableCollection<ViewItemDb>> Get(string IN_TimeStamp)
        {
            _Open();
            var tmpResult = new ObservableCollection<ViewItemDb>();
            var tmpMainTable = new SqliteCommand();
            tmpMainTable.Connection = _connection;
            var tmpTimeStamp = new SqliteParameter("@TimeStamp", IN_TimeStamp);

            tmpMainTable.CommandText = "SELECT * FROM MainTable WHERE TimeStamp LIKE '%'||@TimeStamp||'%';";
            SqliteParameter[] tmpRateCardTableColumns = { tmpTimeStamp };
            tmpMainTable.Parameters.AddRange(tmpRateCardTableColumns);
            var tmpDataReader = await tmpMainTable.ExecuteReaderAsync();

            while (tmpDataReader.Read())
            {
                var tmpCurrentViewItemDb = new ViewItemDb();
                tmpCurrentViewItemDb.Id = tmpDataReader.GetInt32(0);
                tmpCurrentViewItemDb.Name = tmpDataReader.GetString(1);
                tmpCurrentViewItemDb.TimeStamp = tmpDataReader.GetString(2);
                tmpResult.Add(tmpCurrentViewItemDb);
            }
            _Close();
            return tmpResult;
        }

        async public Task<ObservableCollection<ViewItemDb>> Get()
        {
            _Open();
            var tmpResult = new ObservableCollection<ViewItemDb>();
            var tmpMainTable = new SqliteCommand();
            tmpMainTable.Connection = _connection;
            tmpMainTable.CommandText = "SELECT * FROM MainTable WHERE";
            var tmpDataReader = await tmpMainTable.ExecuteReaderAsync();

            while (tmpDataReader.Read())
            {
                var tmpCurrentViewItemDb = new ViewItemDb();
                tmpCurrentViewItemDb.Id = tmpDataReader.GetInt32(0);
                tmpCurrentViewItemDb.Name = tmpDataReader.GetString(1);
                tmpCurrentViewItemDb.TimeStamp = tmpDataReader.GetString(2);
                tmpResult.Add(tmpCurrentViewItemDb);
            }
            _Close();
            return tmpResult;
        }

        async public Task<ObservableCollection<ViewItemDb>> GetUniqueSeries()
        {
            _Open();
            var tmpResult = new ObservableCollection<ViewItemDb>();
            var tmpMainTable = new SqliteCommand();
            tmpMainTable.Connection = _connection;
            tmpMainTable.CommandText = "SELECT *  FROM maintable GROUP BY TimeStamp;";
            var tmpDataReader = await tmpMainTable.ExecuteReaderAsync();

            while (tmpDataReader.Read())
            {
                var tmpCurrentViewItemDb = new ViewItemDb();
                tmpCurrentViewItemDb.Id = tmpDataReader.GetInt32(0);
                tmpCurrentViewItemDb.Name = tmpDataReader.GetString(1);
                tmpCurrentViewItemDb.TimeStamp = tmpDataReader.GetString(2);
                tmpResult.Add(tmpCurrentViewItemDb);
            }
            _Close();
            return tmpResult;
        }

        private void _CreateSchema()
        {
            _Open();
            var tmpSql = "CREATE TABLE IF NOT EXISTS MainTable (ID integer PRIMARY KEY AUTOINCREMENT,Name text,TimeStamp datetime)";

            var tmpCreateCommand = new SqliteCommand(tmpSql, _connection);
            tmpCreateCommand.ExecuteNonQuery();
            _Close();
        }

        private void _Open()
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        private void _Close()
        {
            if (_connection.State != ConnectionState.Closed)
            {
                _connection.Close();
            }
        }
    }


}
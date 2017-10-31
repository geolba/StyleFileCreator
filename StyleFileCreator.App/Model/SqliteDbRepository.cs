using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Threading.Tasks;

namespace StyleFileCreator.App.Model
{
    public class SqliteDbRepository : IDataService, IDisposable
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public List<string> GetTables(string connString)
        {
            List<string> tables = new List<string>();            
            //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + file;

            SQLiteConnection conn = null;
            try
            {

                DataTable dt = new DataTable();
                // Verbindung erzeugen
                conn = new SQLiteConnection(connString);
               
                StringBuilder query = new StringBuilder();
                query.Append("SELECT name FROM sqlite_master ");
                query.Append("WHERE type='table';");    
                using (SQLiteCommand cmd = new SQLiteCommand(query.ToString(), conn))
                {
                    conn.Open();
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Console.WriteLine(dr.GetValue(0) + " " + dr.GetValue(1) + " " + dr.GetValue(2));
                            tables.Add(reader["name"].ToString());
                        }
                       
                    }
                }

              
            }
            catch (Exception ex)
            {
                // Gegebenenfalls Fehlerbehandlung
                System.Windows.MessageBox.Show("Error in finding the DB tables for the defined database! " + ex.Message,
                     "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Verbindung schließen
                //conn.Dispose();  
                if (conn != null)
                    conn.Close();
            }
            tables.Sort();
            return tables;
        }

        public IEnumerable<KeyValuePair<Type, string>> GetColumnNames(string connString, string table)
        {
            //var columnNames = new List<string>();
            //SQLiteConnection connection = null;

            //try
            //{              
            //    // Verbindung erzeugen
            //    connection = new SQLiteConnection(connString);
                                
            //    //string query = "PRAGMA table_info('" + table + "')";
            //    string cmdString = "SELECT * FROM " + table;

            //    using (SQLiteCommand cmd = new SQLiteCommand(cmdString, connection))
            //    {
            //        connection.Open(); //opens connection

            //        using (SQLiteDataReader reader = cmd.ExecuteReader())
            //        {
            //            //while (reader.Read())
            //            //{
            //            //    //columnNames.Add(reader.GetString(0)); // read 'name' column
            //            //    for (int i = 0; i < reader.FieldCount; i++)
            //            //    {
            //            //        columnNames.Add(reader.GetName(i));
            //            //    }
            //            //}
            //            var dataTable = reader.GetSchemaTable();
            //            foreach (DataRow row in dataTable.Rows) columnNames.Add(row.Field<string>("ColumnName"));
            //        }
                    
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Gegebenenfalls Fehlerbehandlung
            //    System.Windows.MessageBox.Show("Error in finding the DB tables for the defined database! " + ex.Message,
            //         "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //finally
            //{
            //    if (connection != null)
            //    {
            //        connection.Close();
            //    }
            //}
            //return columnNames;
            throw new NotImplementedException();
        }

        //public async Task<List<Farbtabelle>> GetAllColortablesAsync(string connString, string table, Dictionary<string, string> dictionary)
        //{
        //    throw new NotImplementedException();
        //}

        public List<Farbtabelle> GetAllColortables(string connString, string table, System.Collections.Generic.Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            //_db.Dispose();
        }
       
    }
}

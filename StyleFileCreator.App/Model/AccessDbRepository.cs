using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using StyleFileCreator.App.Utils;

namespace StyleFileCreator.App.Model
{
    public class AccessDbRepository : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            throw new NotImplementedException();
        }

        public List<string> GetTables(string connString)
        {            
            List<string> Tables = new List<string>();
            System.Data.DataTable tables;
            //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + file;

            // Verbindung erzeugen
            OleDbConnection conn = new OleDbConnection(connString);
            try
            {
                // Verbindung öffnen
                conn.Open();
                tables = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                for (int i = 0; i < tables.Rows.Count; i++)
                {
                    Tables.Add(tables.Rows[i][2].ToString());
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
                conn.Close();
            }
            Tables.Sort();
            return Tables;
        }

        public IEnumerable<KeyValuePair<Type, string>> GetColumnNames(string connString, string table)
        {
            var dataTypes = new ListWithDuplicates<Type, string>();
            OleDbConnection connection = null;

            try
            {              
                // Verbindung erzeugen
                connection = new OleDbConnection(connString);

                string cmdString = "SELECT * FROM " + "["+table+"]";                             
                //OleDbDataAdapter adapter = new OleDbDataAdapter();

                using (OleDbCommand cmd = new OleDbCommand(cmdString, connection))
                {                    
                    connection.Open(); //opens connection

                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {                        
                        DataTable dataTable = reader.GetSchemaTable();
                        foreach (DataRow row in dataTable.Rows)
                        {   
                            string columnName = row.Field<string>("ColumnName");                         
                            OleDbType test1 = row.Field<OleDbType>("ProviderType");                          
                            System.Type dataType = (System.Type) row["DataType"];                          
                            dataTypes.Add(dataType, columnName);                            
                        }                      
                    }
                    
                }
            }
            catch (Exception ex)
            {
                // Gegebenenfalls Fehlerbehandlung
                MessageBox.Show("Error in finding the column names for the defined table! " + ex.Message,
                        "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (connection != null)
                {
                    connection.Close();
                }
            }          
            //IEnumerable<KeyValuePair<Type, string>> set = dataTypes.Where(p => p.Key == typeof(System.String));
            //extensions:
            //dataTypes.Write();
            //ListWithDuplicates<Type, string> set = dataTypes.Where(p => p.Key == typeof(System.String)).ToListWithDuplicates(t => t.Key, t => t.Value);
                    
            return dataTypes;
        }
             
        //public async Task<List<Farbtabelle>> GetAllColortablesAsync(string connString, string table, Dictionary<string, string> dictionary)
        //{
        //    using (var db = DataContext.CreateContext(connString, table, dictionary))
        //    {
        //        var list = await db.ColorTables.ToListAsync<Farbtabelle>();
        //        return list;                
        //    }
        //}

        public List<Farbtabelle> GetAllColortables(string connString, string table, Dictionary<string, string> dictionary)
        {
            using (var db = DataContext.CreateContext(connString, table, dictionary))
            {
                List<Farbtabelle> list = db.ColorTables.ToList();                
                return list;

                //list.FirstOrDefault().Name = "Alkali olivine basalt is silica-undersaturated, characterized by the absence of orthopyroxene, absence of quartz, presence of olivine, and typically contains some feldspathoid mineral, alkali feldspar or phlogopite in the groundmass. Feldspar phenocrysts typically are labradorite to andesine in composition. Augite is rich in titanium compared to augite in tholeiitic basalt. Alkali olivine basalt is relatively rich in sodium.";
                //list.FirstOrDefault().Hexwert = "allesTest";                
                //db.SaveChanges();

                //DbEntityValidationException:
                //Validation failed for one or more entities. See 'EntityValidationErrors' property for more details.
            }


        }

        public void Dispose()
        {            
        }
    }


    

}

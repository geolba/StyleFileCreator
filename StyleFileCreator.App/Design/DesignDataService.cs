using System;
using System.Collections.Generic;
using StyleFileCreator.App.Model;
using System.Threading.Tasks;
using System.Data.Entity;

namespace StyleFileCreator.App.Design
{
   
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data
            var item = new DataItem("Inspire Stylefile Creator [design]");
            callback(item, null);
        }


        public System.Collections.Generic.List<string> GetTables(string connString)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<System.Collections.Generic.KeyValuePair<Type, string>> GetColumnNames(string connString, string table)
        {
            throw new NotImplementedException();
        }

        public List<Farbtabelle> GetAllColortables(string connString, string table, System.Collections.Generic.Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {          
        }
    }
   
}

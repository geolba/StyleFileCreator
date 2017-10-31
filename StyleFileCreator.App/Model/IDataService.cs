using System;
using System.Collections.Generic;

namespace StyleFileCreator.App.Model
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);

        List<string> GetTables(string connString);
        IEnumerable<KeyValuePair<Type, string>> GetColumnNames(string connString, string table);
        //Task<List<Farbtabelle>> GetAllColortablesAsync(string connString, string table, Dictionary<string, string> dictionary);
        List<Farbtabelle> GetAllColortables(string connString, string table, Dictionary<string, string> dictionary);
        void Dispose();
    }
}

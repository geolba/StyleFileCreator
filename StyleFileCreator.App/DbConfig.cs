using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite.EF6;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Common;
using System.Data.SQLite;
using StyleFileCreator.App.Model;

namespace StyleFileCreator.App
{

    public class SQLiteProviderInvariantName : IProviderInvariantName
    {
        public static readonly SQLiteProviderInvariantName Instance = new SQLiteProviderInvariantName();
        private SQLiteProviderInvariantName() { }
        public const string ProviderName = "System.Data.SQLite.EF6";
        public string Name { get { return ProviderName; } }
    }

    class SQLiteDbDependencyResolver : IDbDependencyResolver
    {
        public object GetService(Type type, object key)
        {
            if (type == typeof(IProviderInvariantName)) return SQLiteProviderInvariantName.Instance;
            if (type == typeof(DbProviderFactory)) return SQLiteProviderFactory.Instance;
            return SQLiteProviderFactory.Instance.GetService(type);
        }

        public IEnumerable<object> GetServices(Type type, object key)
        {
            var service = GetService(type, key);
            if (service != null) yield return service;
        }
    }

    public class AccessDbConfig : DbConfiguration
    {
        public AccessDbConfig()
        {
            //this.SetDefaultConnectionFactory(new System.Data.Entity.Infrastructure.SqlConnectionFactory());
              //SetDefaultConnectionFactory(new LocalDbConnectionFactory("v.11"));
            this.SetDatabaseInitializer<DataContext>(null); 

            this.SetProviderFactory("JetEntityFrameworkProvider", JetEntityFrameworkProvider.JetProviderFactory.Instance);
            this.SetProviderServices("JetEntityFrameworkProvider", new JetEntityFrameworkProvider.JetProviderServices());

            //SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            //SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            //SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));

            //HACK
           // var EF6ProviderServicesType = typeof(System.Data.SQLite.EF6.SQLiteProviderFactory).Assembly.DefinedTypes.First(x => x.Name == "SQLiteProviderServices");
           // var EF6ProviderServices = (DbProviderServices)Activator.CreateInstance(EF6ProviderServicesType); 
           // SetProviderServices("System.Data.SQLite.EF6", EF6ProviderServices);
           //SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
           //SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            
            
          
    

            //AddDependencyResolver(new SQLiteDbDependencyResolver());
        }
    }

    public class SqliteDbConfig : DbConfiguration
    {
        public SqliteDbConfig()
        {
            this.SetDatabaseInitializer<SqliteDataContext>(null); 

            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (System.Data.Entity.Core.Common.DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(System.Data.Entity.Core.Common.DbProviderServices)));
            
            ////SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            //SetProviderServices("System.Data.SQLite", (System.Data.Common.DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(System.Data.Common.prDbProviderServices)));

            
        }
    }
}

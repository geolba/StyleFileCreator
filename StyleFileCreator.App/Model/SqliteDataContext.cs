using System.Collections.Generic;
using StyleFileCreator.App.Model.Mapping;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Infrastructure;
using System.Data.Common;

namespace StyleFileCreator.App.Model
{
    [DbConfigurationType(typeof(SqliteDbConfig))]
    public class SqliteDataContext : DbContext
    {
        public virtual DbSet<Farbtabelle> ColorTables { get; set; }

        static SqliteDataContext()
        {
            DbConfiguration.SetConfiguration(new SqliteDbConfig());
        }

        private SqliteDataContext(string _ConnectionString)
            : base(_ConnectionString)
        {
        }

        public SqliteDataContext(string connectionString, DbCompiledModel model)
            : base(new System.Data.SQLite.SQLiteConnection() { ConnectionString = connectionString },model, true)        
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public SqliteDataContext()
            : base("Name=KartierungsbuchContext")
        {
            this.Configuration.LazyLoadingEnabled = true;
        }

        public static SqliteDataContext CreateContext(string connectionString, Dictionary<string, string> dic)
        {
            //string connectionString = ConfigurationManager.ConnectionStrings["VAMPTest"].ConnectionString;
            //DataContext dummyContext = new DataContext(connectionString);

            string table = dic["table"];
            string idColumn = dic["idColumn"];
            string colorColumn = dic["colorColumn"];
            string nameColumn = dic["nameColumn"];
            string tagColumn = dic["tagColumn"];

            DbModelBuilder modelBuilder = new DbModelBuilder(DbModelBuilderVersion.Latest);
            modelBuilder.Configurations.Add(new FarbtabelleMap(table, idColumn, colorColumn, nameColumn, tagColumn));
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //builder.Entity<DataContext>().ToTable(tableName);

            System.Data.SQLite.SQLiteConnection dummyConnection = new System.Data.SQLite.SQLiteConnection(connectionString);
            //JetEntityFrameworkProvider.JetConnection dummyConnection = new JetEntityFrameworkProvider.JetConnection(connectionString);
            //DbConnection dummyConnection = dummyContext.Database.Connection;            
            System.Data.Entity.Infrastructure.DbModel model = modelBuilder.Build(dummyConnection);
            System.Data.Entity.Infrastructure.DbCompiledModel compiledModel = model.Compile();
            // Finally make our database context
            SqliteDataContext dummyContext = new SqliteDataContext(connectionString, compiledModel);
            return dummyContext;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Configurations.Add(new FarbtabelleMap(this.table, this.colorColumn, this.nameColumn, this.tagColumn));           
        }
    }
}

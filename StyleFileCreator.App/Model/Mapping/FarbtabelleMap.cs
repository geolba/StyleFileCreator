using System.Data.Entity.ModelConfiguration;
//using System.ComponentModel.DataAnnotations.Schema;

namespace StyleFileCreator.App.Model.Mapping
{
    public class FarbtabelleMap : EntityTypeConfiguration<Farbtabelle>
    {
        private string tableName = string.Empty;
        private string idColumn = string.Empty;
        private string colorColumn = string.Empty;
        private string nameColumn = string.Empty;
        private string tagColumn = string.Empty;
        private string category = string.Empty;

        #region constructor

        public FarbtabelleMap(string tableName, string idColumn, string colorColumn, string nameColumn, string tagColumn)
        {
            this.tableName = tableName;
            this.idColumn = idColumn;
            this.colorColumn = colorColumn;
            this.nameColumn = nameColumn;
            this.tagColumn = tagColumn;

            this.SetPrimaryKey();
            this.SetTableAndColumnMappings();
            //this.SetProperties();
        }

        #endregion

        #region private helper methods

        private void SetPrimaryKey()
        {
            this.HasKey(t => t.Id);
        }

        private void SetTableAndColumnMappings()
        {
            base.ToTable(this.tableName);
            //Property(t => t.Id).HasColumnName("Objectid");
            base.Property(t => t.Id).HasColumnName(this.idColumn)
            //.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            .IsRequired();

            //Property(t => t.Hexwert).HasColumnName("HEXWert");     
            base.Property(t => t.Hexwert).HasColumnName(this.colorColumn);
            base.Property(t => t.Name).HasColumnName(this.nameColumn);
            //Ignore(t => t.Tag);
            base.Property(t => t.Tag).HasColumnName(this.tagColumn);
            base.Ignore(t => t.ShortName);
        }

        private void SetProperties()
        {
            //Property(t => t.Id)
            //  .HasMaxLength(36)
            //   .IsFixedLength()
            //  .IsUnicode(false);

            base.Property(t => t.Hexwert)
                .IsRequired()               
                .HasMaxLength(255)
                .IsUnicode(true);

            base.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(true);

            base.Property(t => t.Tag)
               .IsOptional()
               .HasMaxLength(255)
               .IsUnicode(true);

        }

        #endregion
    }

    //public class EmailAttribute : System.ComponentModel.DataAnnotations.RegularExpressionAttribute
    //{
    //    public EmailAttribute()
    //        : base(GetRegex())
    //    { }

    //    private static string GetRegex()
    //    {
    //        // TODO: Go off and get your RegEx here
    //        return @"^[\w-]+(\.[\w-]+)*@([a-z0-9-]+(\.[a-z0-9-]+)*?\.[a-z]{2,6}|(\d{1,3}\.){3}\d{1,3})(:\d{4})?$";
    //    }
    //}






}

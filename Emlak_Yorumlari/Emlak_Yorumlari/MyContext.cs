using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities.Models;

namespace Emlak_Yorumlari_Entities
{
   public class MyContext : DbContext
    {
        public MyContext() : base("Name=MyContext")
        {
            // Database.SetInitializer(new MigrateDatabaseToLatestVersion<MyContext, Emlak_Yorumlari.Migrations.Configuration>());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Adress_Description> Adress_Descriptions { get; set; }
        public DbSet<Adress_Type> Adress_Types { get; set; }
        public DbSet<Question_Definition> Question_Definitions { get; set; }
        public DbSet<Question_Type> Question_Types { get; set; }
        public DbSet<Combobox_Answer> Combobox_Answers { get; set; }
        public DbSet<Place_Statistics> Place_Statistics { get; set; }
        public DbSet<Comment_Log> Comment_Logs { get; set; }
        public DbSet<Survey_Log> Survey_Logs { get; set; }
        
    }
}

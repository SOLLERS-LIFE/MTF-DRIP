using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using MySqlConnector.Authentication.Ed25519;

using MTFramework.Utilities;
using MTFDRIP.ApplicationDB.Models;

namespace MTFDRIP.ApplicationDB.Data
{
    public partial class AppDB_Context
    // the trick to provide indexes definitions
    // from another parts of partial database context
    // Will be removed by compiler if not defined.
    {
        partial void _defineEntities_ext01(ModelBuilder modelBuilder);
    }
    public partial class AppDB_Context : DbContextWithScalarReturn
    {
        public AppDB_Context(DbContextOptions<AppDB_Context> options)
            : base(options)
        {
            Database.SetCommandTimeout((int)TimeSpan.FromMinutes(GlobalParameters._appDB_ConnectionTimeout).TotalSeconds);
        }
        // This second protected constructor allows to avoid problems
        // with inherited classes constructor
        // protected constructor is not visible to model builder but
        // it can be used in derived classes 
        protected AppDB_Context(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<sseProducts_v> _sseProducts_v { get; set; }
        public DbSet<sseProducts> _sseProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // to enable new user verification
            // with MySqlConnector.Authentication.Ed25519 NuGet Package
            Ed25519AuthenticationPlugin.Install();

            modelBuilder.Entity<sseProducts_v>()
                .HasNoKey()
                ;

            if (!GlobalParameters.IsStartedWithMain)
            {
                modelBuilder.Ignore<sseProducts_v>();
            }

            // indexes definitions from first part
            // of partial context
            _defineEntities_ext01(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}

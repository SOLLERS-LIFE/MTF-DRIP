using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SSE.ApplicationDB.Data;

namespace MTFramework.Utilities
{ 
    public static class migrater
    {
        public static void Migrate(IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var ctx = scope.ServiceProvider.GetRequiredService<AppDB_Context>();

            ctx.Database.Migrate();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;

using MTFramework.Utilities;
using SSE.ApplicationDB.Models;
using SSE.Properties;

namespace SSE.ApplicationDB.Data
{
    public static class AppDBInitializer
    {
        public static async Task<int> DoIt (AppDB_Context ADB)
        {
            var rc = (await ADB._sr.FromSqlRaw(Resources.appDB_programmatic, Array.Empty<object>())
                               .AsNoTracking()
                               .ToListAsync())
                     .ElementAtOrDefault(0);

            return await Task.FromResult(rc.RetValueInt);
        }
    }
}

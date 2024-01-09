using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tenon.Repository.EfCore.Sqlite.Configurations
{
    public sealed class SqliteOptions
    {
        public string? ConnectionString { get; set; } = string.Empty;
    }
}

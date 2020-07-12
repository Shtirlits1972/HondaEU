using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HondaEU
{
    public class Ut
    {
        public static string GetMySQLConnect()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfiguration Configuration = builder.Build();
            return Configuration.GetConnectionString("MySQLConnect");
        }
    }
}

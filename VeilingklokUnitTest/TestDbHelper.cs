using Microsoft.EntityFrameworkCore;
using Veilingklok.Infrastructure.Database;   // JUISTE namespace voor MyContext
using System;

namespace VeilingklokUnitTest
{
    internal static class TestDbHelper
    {
        public static MyContext CreateInMemory()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MyContext(options);
        }
    }
}

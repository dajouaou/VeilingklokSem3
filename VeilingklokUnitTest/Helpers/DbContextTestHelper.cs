using Microsoft.EntityFrameworkCore;
using Veilingklok.Infrastructure.Database;

namespace VeilingklokUnitTest.Helpers;

public static class DbContextTestHelper
{
    public static MyContext Create(string? name = null)
    {
        var options = new DbContextOptionsBuilder<MyContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var db = new MyContext(options);
        db.Database.EnsureCreated();
        return db;
    }
}

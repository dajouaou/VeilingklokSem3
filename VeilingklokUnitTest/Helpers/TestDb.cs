// Helpers/TestDb.cs
using Microsoft.EntityFrameworkCore;
using Veilingklok.Infrastructure.Database;

namespace VeilingklokUnitTest.Helpers;

public static class TestDb
{
    public static MyContext Create(string? name = null)
    {
        var options = new DbContextOptionsBuilder<MyContext>()
            .UseInMemoryDatabase(name ?? Guid.NewGuid().ToString())
            .Options;

        return new MyContext(options);
    }
}

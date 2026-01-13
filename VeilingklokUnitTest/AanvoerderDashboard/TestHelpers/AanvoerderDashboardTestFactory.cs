using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Veilingklok.Core.Entities;
using Veilingklok.Infrastructure.Database;

namespace VeilingklokUnitTest.AanvoerderDashboard.TestHelpers
{
    // Deze helper gebruik ik om mijn unittests overzichtelijk te houden
    public static class AanvoerderDashboardTestFactory
    {
        // Maakt een nieuwe in-memory database zodat elke test schoon start
        public static MyContext CreateDb()
        {
            var options = new DbContextOptionsBuilder<MyContext>()
                // Elke test krijgt een eigen database
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new MyContext(options);
        }

        // Voegt een aanvoerder toe die nodig is om de service te laten werken
        public static async Task SeedAanvoerderAsync(
            MyContext db,
            int gebruikerId,
            int aanvoerderId,
            string naam = "Test Aanvoerder")
        {
            db.Aanvoerders.Add(new Aanvoerder
            {
                Id = aanvoerderId,
                GebruikerId = gebruikerId,
                Naam = naam
            });

            // Opslaan zodat de data echt in de database staat
            await db.SaveChangesAsync();
        }

        // Geeft de eerstvolgende opgegeven dag terug (handig voor weekendtests)
        public static DateTime NextDayOfWeek(DateTime from, DayOfWeek day)
        {
            var d = from.Date;

            // Blijft dagen optellen tot de juiste dag is bereikt
            while (d.DayOfWeek != day)
            {
                d = d.AddDays(1);
            }

            return d;
        }
    }
}

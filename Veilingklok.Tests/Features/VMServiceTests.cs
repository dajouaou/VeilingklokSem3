// Veilingklok.Tests/Features/VM/VMServiceTests.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.VM.Services;
using Veilingklok.Infrastructure.Database;
using Xunit;

namespace Veilingklok.Tests.Features.VM;

public sealed class VMServiceTests
{
    private static MyContext CreateInMemoryContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<MyContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new MyContext(options);
    }

    [Fact]
    public async Task StartVeilingAsync_StartQueuedProductAndSetsStatusRunning()
    {
        var db = CreateInMemoryContext(nameof(StartVeilingAsync_StartQueuedProductAndSetsStatusRunning));

        var vmUser = new Gebruiker
        {
            Id = 1,
            Username = "vm1",
            Email = "vm@example.com",
            Role = UserRole.VM,
            CreatedAtUtc = DateTime.UtcNow
        };

        var vm = new Core.Entities.VM
        {
            Id = 1,
            GebruikerId = vmUser.Id,
            Gebruiker = vmUser,
            Naam = "Test VM"
        };

        db.Gebruikers.Add(vmUser);
        db.VMs.Add(vm);

        var veiling = new Veiling
        {
            Id = 1,
            Naam = "Testveiling",
            VMId = vm.Id,
            VM = vm,
            Status = VeilingStatus.Scheduled,
            Locatie = "Aalsmeer",
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Veilingen.Add(veiling);

        db.VeilingProducten.AddRange(
            new VeilingProduct
            {
                Id = 1,
                VeilingId = 1,
                Volgorde = 2,
                Hoeveelheid = 10,
                StartPrijs = 10m,
                HuidigePrijs = 10m,
                Status = VeilingProductStatus.Queued
            },
            new VeilingProduct
            {
                Id = 2,
                VeilingId = 1,
                Volgorde = 1,
                Hoeveelheid = 5,
                StartPrijs = 20m,
                HuidigePrijs = 20m,
                Status = VeilingProductStatus.Queued
            }
        );

        await db.SaveChangesAsync();

        var service = new VMService(db);

        var result = await service.StartVeilingAsync(1);

        Assert.True(result.Success);

        var veilingFromDb = await db.Veilingen
            .Include(v => v.VeilingProducten)
            .FirstAsync(v => v.Id == 1);

        Assert.Equal(VeilingStatus.Running, veilingFromDb.Status);
        Assert.Equal(2, veilingFromDb.CurrentVeilingProductId);

        var current = veilingFromDb.VeilingProducten.Single(p => p.Id == 2);
        Assert.Equal(VeilingProductStatus.Active, current.Status);
        Assert.NotNull(current.ActivatedAtUtc);

        Assert.True(veilingFromDb.AuditEntries?.Any() ?? false);
        var lastAudit = veilingFromDb.AuditEntries!.Last();
        Assert.Equal("Veiling gestart", lastAudit.Action);
    }
}

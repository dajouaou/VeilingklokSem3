// Veilingklok.Tests/Features/VM/VMDtoTests.cs
using System;
using System.Collections.Generic;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Features.VM.Dtos;
using Xunit;

namespace Veilingklok.Tests.Features.VM;

public sealed class VMDtoTests
{
    [Fact]
    public void VMAuditDto_FromEntity_GeeftJuisteActorEnActorNaam()
    {
        var gebruiker = new Gebruiker
        {
            Id = 1,
            Voornaam = "Test",
            Achternaam = "Naam",
            Username = "testuser"
        };

        var entry = new AuditEntry
        {
            Id = 10,
            Action = "Veiling gestart",
            CreatedAtUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            ActorGebruiker = gebruiker
        };

        var dto = VMAuditDto.FromEntity(entry);

        Assert.Equal(10, dto.Id);
        Assert.Equal("Veiling gestart", dto.Action);
        Assert.Equal(entry.CreatedAtUtc, dto.CreatedAtUtc);
        Assert.Equal("testuser", dto.Actor);
        Assert.Equal("Test Naam", dto.ActorNaam);
    }

    [Fact]
    public void VMAuditDto_FromEntity_ValtTerugOpFullNameAlsUsernameLeegIs()
    {
        var gebruiker = new Gebruiker
        {
            Id = 1,
            Voornaam = "Alleen",
            Achternaam = "Naam",
            Username = ""
        };

        var entry = new AuditEntry
        {
            Id = 11,
            Action = "Actie",
            CreatedAtUtc = DateTime.UtcNow,
            ActorGebruiker = gebruiker
        };

        var dto = VMAuditDto.FromEntity(entry);

        Assert.Equal("Alleen Naam", dto.Actor);
        Assert.Equal("Alleen Naam", dto.ActorNaam);
    }

    [Fact]
    public void VMAuditDto_FromEntity_LegeStringsAlsGeenActor()
    {
        var entry = new AuditEntry
        {
            Id = 12,
            Action = "Actie",
            CreatedAtUtc = DateTime.UtcNow,
            ActorGebruiker = null
        };

        var dto = VMAuditDto.FromEntity(entry);

        Assert.Equal(string.Empty, dto.Actor);
        Assert.Equal(string.Empty, dto.ActorNaam);
    }

    [Fact]
    public void VMBidDto_FromEntity_GebruiktFullNameBovenUsernameEnKoperNaam()
    {
        var gebruiker = new Gebruiker
        {
            Voornaam = "Full",
            Achternaam = "Name",
            Username = "user123"
        };

        var koper = new Koper
        {
            Naam = "Fallback Naam",
            Gebruiker = gebruiker
        };

        var bid = new Bid
        {
            Id = 5,
            VeilingProductId = 99,
            Amount = 12.34m,
            PlacedAtUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            Koper = koper
        };

        var dto = VMBidDto.FromEntity(bid);

        Assert.Equal(5, dto.Id);
        Assert.Equal(99, dto.VeilingProductId);
        Assert.Equal(12.34m, dto.Amount);
        Assert.Equal(bid.PlacedAtUtc, dto.PlacedAtUtc);
        Assert.Equal("Full Name", dto.KoperNaam);
    }

    [Fact]
    public void VMBidDto_FromEntity_GebruiktUsernameAlsGeenFullName()
    {
        var gebruiker = new Gebruiker
        {
            Username = "user123"
        };

        var koper = new Koper
        {
            Naam = "Fallback Naam",
            Gebruiker = gebruiker
        };

        var bid = new Bid
        {
            Id = 6,
            VeilingProductId = 100,
            Amount = 5m,
            PlacedAtUtc = DateTime.UtcNow,
            Koper = koper
        };

        var dto = VMBidDto.FromEntity(bid);

        Assert.Equal("user123", dto.KoperNaam);
    }

    [Fact]
    public void VMBidDto_FromEntity_GebruiktKoperNaamAlsGeenUserGegevens()
    {
        var koper = new Koper
        {
            Naam = "Fallback Naam",
            Gebruiker = null
        };

        var bid = new Bid
        {
            Id = 7,
            VeilingProductId = 101,
            Amount = 5m,
            PlacedAtUtc = DateTime.UtcNow,
            Koper = koper
        };

        var dto = VMBidDto.FromEntity(bid);

        Assert.Equal("Fallback Naam", dto.KoperNaam);
    }

    [Fact]
    public void VMCurrentProductDto_FromEntity_OrdentBidsAfDalendOpPlacedAt()
    {
        var bids = new List<Bid>
        {
            new Bid
            {
                Id = 1,
                Amount = 10,
                PlacedAtUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                Koper = new Koper { Naam = "A" }
            },
            new Bid
            {
                Id = 2,
                Amount = 11,
                PlacedAtUtc = new DateTime(2025, 1, 1, 12, 5, 0, DateTimeKind.Utc),
                Koper = new Koper { Naam = "B" }
            }
        };

        var vp = new VeilingProduct
        {
            Id = 10,
            ProductId = 1,
            AanvoerderId = 2,
            Product = new Product { Naam = "Roos" },
            Aanvoerder = new Aanvoerder { Naam = "Aanvoerder X" },
            StartPrijs = 5,
            HuidigePrijs = 7,
            Hoeveelheid = 100,
            Status = VeilingProductStatus.Active,
            ActivatedAtUtc = DateTime.UtcNow,
            ClosedAtUtc = null,
            Bids = bids
        };

        var dto = VMCurrentProductDto.FromEntity(vp);

        Assert.Equal(10, dto.Id);
        Assert.Equal(1, dto.ProductId);
        Assert.Equal(2, dto.AanvoerderId);
        Assert.Equal("Roos", dto.ProductNaam);
        Assert.Equal("Aanvoerder X", dto.Aanvoerder);
        Assert.Equal(5, dto.StartPrijs);
        Assert.Equal(7, dto.HuidigePrijs);
        Assert.Equal(100, dto.Hoeveelheid);
        Assert.Equal(VeilingProductStatus.Active, dto.Status);
        Assert.Equal(2, dto.Bids.Count);
        Assert.Equal(2, dto.Bids[0].Id);
        Assert.True(dto.Bids[0].PlacedAtUtc > dto.Bids[1].PlacedAtUtc);
    }

    [Fact]
    public void VMVeilingProductDto_FromEntity_MaptVeldenJuist()
    {
        var vp = new VeilingProduct
        {
            Id = 1,
            Volgorde = 3,
            Product = new Product { Naam = "Tulp" },
            Aanvoerder = new Aanvoerder { Naam = "Aanvoerder Y" },
            Status = VeilingProductStatus.Queued,
            HuidigePrijs = 4.5m,
            Hoeveelheid = 50
        };

        var dto = VMVeilingProductDto.FromEntity(vp);

        Assert.Equal(1, dto.Id);
        Assert.Equal(3, dto.Volgorde);
        Assert.Equal("Tulp", dto.ProductNaam);
        Assert.Equal("Aanvoerder Y", dto.Aanvoerder);
        Assert.Equal(VeilingProductStatus.Queued, dto.Status);
        Assert.Equal(4.5m, dto.HuidigePrijs);
        Assert.Equal(50, dto.Hoeveelheid);
    }

    [Fact]
    public void VMVeilingDashboardDto_FromEntity_BouwtQueueAuditEnStats()
    {
        var vmGebruiker = new Gebruiker
        {
            Id = 10,
            Username = "vmuser"
        };

        var vm = new Core.Entities.VM
        {
            Id = 20,
            GebruikerId = vmGebruiker.Id,
            Gebruiker = vmGebruiker,
            Naam = "VM Naam"
        };

        var product1 = new VeilingProduct
        {
            Id = 1,
            Volgorde = 2,
            Product = new Product { Naam = "P2" },
            Aanvoerder = new Aanvoerder { Naam = "A2" },
            Status = VeilingProductStatus.Queued,
            HuidigePrijs = 10,
            Hoeveelheid = 20,
            Bids = new List<Bid>
            {
                new Bid { Id = 1, Amount = 5, PlacedAtUtc = DateTime.UtcNow, Koper = new Koper { Naam = "K1" } }
            }
        };

        var product2 = new VeilingProduct
        {
            Id = 2,
            Volgorde = 1,
            Product = new Product { Naam = "P1" },
            Aanvoerder = new Aanvoerder { Naam = "A1" },
            Status = VeilingProductStatus.Queued,
            HuidigePrijs = 8,
            Hoeveelheid = 10,
            Bids = new List<Bid>()
        };

        var soldProduct = new VeilingProduct
        {
            Id = 3,
            Volgorde = 3,
            Product = new Product { Naam = "P3" },
            Aanvoerder = new Aanvoerder { Naam = "A3" },
            Status = VeilingProductStatus.Sold,
            HuidigePrijs = 12,
            Hoeveelheid = 5,
            Bids = new List<Bid>
            {
                new Bid { Id = 2, Amount = 6, PlacedAtUtc = DateTime.UtcNow, Koper = new Koper { Naam = "K2" } }
            }
        };

        var current = new VeilingProduct
        {
            Id = 4,
            Volgorde = 0,
            Product = new Product { Naam = "Current" },
            Aanvoerder = new Aanvoerder { Naam = "AC" },
            Status = VeilingProductStatus.Active,
            HuidigePrijs = 15,
            Hoeveelheid = 30,
            Bids = new List<Bid>()
        };

        var auditEntries = new List<AuditEntry>
        {
            new AuditEntry
            {
                Id = 1,
                Action = "Laatste actie",
                CreatedAtUtc = new DateTime(2025, 1, 2, 12, 0, 0, DateTimeKind.Utc),
                ActorGebruiker = vmGebruiker
            },
            new AuditEntry
            {
                Id = 2,
                Action = "Eerdere actie",
                CreatedAtUtc = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                ActorGebruiker = vmGebruiker
            }
        };

        var veiling = new Veiling
        {
            Id = 99,
            Naam = "Testveiling",
            VMId = vm.Id,
            VM = vm,
            Locatie = "Aalsmeer",
            Status = VeilingStatus.Running,
            StartTijdUtc = new DateTime(2025, 1, 1, 8, 0, 0, DateTimeKind.Utc),
            EindTijdUtc = null,
            CurrentVeilingProduct = current,
            VeilingProducten = new List<VeilingProduct>
            {
                product1,
                product2,
                soldProduct,
                current
            },
            AuditEntries = auditEntries
        };

        var dto = VMVeilingDashboardDto.FromEntity(veiling);

        Assert.Equal(99, dto.VeilingId);
        Assert.Equal("Testveiling", dto.VeilingNaam);
        Assert.Equal("Aalsmeer", dto.Locatie);
        Assert.Equal(VeilingStatus.Running, dto.Status);
        Assert.Equal("VM Naam", dto.VMNaam);
        Assert.Equal(veiling.StartTijdUtc, dto.StartTijdUtc);
        Assert.Null(dto.EindTijdUtc);
        Assert.NotNull(dto.Current);
        Assert.Equal(current.Id, dto.Current!.Id);
        Assert.Equal(4, dto.TotaalProducten);
        Assert.Equal(2, dto.ProductenInQueue);
        Assert.Equal(1, dto.VerkochteProducten);
        Assert.Equal(2, dto.TotaalBiedingen);
        Assert.Equal(2, dto.Queue.Count);
        Assert.Equal(2, dto.Queue[0].Id);
        Assert.Equal(1, dto.Queue[1].Id);
        Assert.Equal(2, dto.Audit.Count);
        Assert.Equal("Laatste actie", dto.Audit[0].Action);
        Assert.True(dto.Audit[0].CreatedAtUtc > dto.Audit[1].CreatedAtUtc);
    }
}

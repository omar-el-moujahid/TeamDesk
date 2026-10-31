using Microsoft.EntityFrameworkCore;
using TeamDesk.Api.Data;
using TeamDesk.Api.Entities;
using TeamDesk.Api.Entities.enums;                       // ← "Enums" si tu as renommé le dossier
using TeamDesk.Api.Services;
using static TeamDesk.Api.Dtos.Requests.RequestDtos;     // ← enlève "static ...RequestDtos" si tes DTOs ne sont plus imbriqués

namespace TeamDesk.Tests;

public class RequestServiceTests
{
    // ---------- Outils communs ----------

    // Crée une base en mémoire NEUVE pour chaque test (nom unique = base isolée)
    private static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();     // applique tes HasData : catégories 1-3, utilisateurs 1-5
        return db;
    }

    // Ajoute une demande de test en base et la renvoie
    private static Request AddRequest(AppDbContext db, RequestStatus status = RequestStatus.New, int? assignedToId = null)
    {
        var request = new Request
        {
            Title = "Test",
            Description = "Description de test",
            CategoryId = 1,
            Priority = Priority.Medium,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            CreatedById = 2,                 // Sara
            AssignedToId = assignedToId
        };
        db.Requests.Add(request);
        db.SaveChanges();
        return request;
    }

    // ---------- Tests ----------

    [Fact]
    public async Task CreateAsync_UnknownCategory_ReturnsNull()
    {
        // Arrange : préparer
        using var db = CreateDb();
        var service = new RequestService(db);
        var dto = new RequestCreateDto
        {
            Title = "Écran",
            Description = "Ne s'allume plus",
            CategoryId = 99,                 // n'existe pas
            Priority = Priority.High
        };

        // Act : agir
        var result = await service.CreateAsync(dto, 2);

        // Assert : vérifier
        Assert.Null(result);
        Assert.Empty(db.Requests);           // rien n'a été enregistré
    }

    [Fact]
    public async Task UpdateStatusAsync_UserNotAssigned_ReturnsForbidden()
    {
        using var db = CreateDb();
        var request = AddRequest(db, assignedToId: 3);      // assignée à Yassine
        var service = new RequestService(db);

        // Sara (Id 2), qui n'est ni Manager ni assignée, essaie de changer le statut
        var result = await service.UpdateStatusAsync(request.Id, RequestStatus.InProgress, userId: 2, isManager: false);

        Assert.Equal(OperationResult.Forbidden, result);
        Assert.Equal(RequestStatus.New, db.Requests.Single().Status);   // le statut n'a PAS changé
    }

    [Fact]
    public async Task UpdateStatusAsync_AssignedUser_ReturnsSuccess()
    {
        using var db = CreateDb();
        var request = AddRequest(db, assignedToId: 2);      // assignée à Sara
        var service = new RequestService(db);

        var result = await service.UpdateStatusAsync(request.Id, RequestStatus.InProgress, userId: 2, isManager: false);

        Assert.Equal(OperationResult.Success, result);
        Assert.Equal(RequestStatus.InProgress, db.Requests.Single().Status);
    }

    [Fact]
    public async Task AssignAsync_UnknownUser_ReturnsInvalid()
    {
        using var db = CreateDb();
        var request = AddRequest(db);
        var service = new RequestService(db);

        var result = await service.AssignAsync(request.Id, assignedToId: 999);

        Assert.Equal(OperationResult.Invalid, result);
        Assert.Null(db.Requests.Single().AssignedToId);
    }

    [Fact]
    public async Task GetDashboardAsync_CountsRequestsByStatus()
    {
        using var db = CreateDb();
        AddRequest(db, RequestStatus.New);
        AddRequest(db, RequestStatus.New);
        AddRequest(db, RequestStatus.InProgress);
        var service = new RequestService(db);

        var dashboard = await service.GetDashboardAsync();

        Assert.Equal(3, dashboard.Total);
        Assert.Equal(2, dashboard.ByStatus.Single(s => s.Status == "New").Count);
        Assert.Equal(1, dashboard.ByStatus.Single(s => s.Status == "InProgress").Count);
        Assert.Equal(0, dashboard.ByStatus.Single(s => s.Status == "Closed").Count);  // statut sans demande = 0
    }
}
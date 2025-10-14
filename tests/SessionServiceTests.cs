using Domain.Constants;
using FluentAssertions;
using Infrastructure.Database;
using Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests;

public class SessionServiceTests : IAsyncDisposable
{
    private readonly SqliteConnection _connection;
    private readonly IDbContextFactory<AppDbContext> _factory;
    private readonly SessionService _service;

    public SessionServiceTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();
        }

        _factory = new TestDbContextFactory(options, _connection);
        _service = new SessionService(_factory);
    }

    [Fact]
    public async Task GetOrCreateAsync_СоздаетОднуЗапись()
    {
        var session1 = await _service.GetOrCreateAsync(12345, CancellationToken.None);
        var session2 = await _service.GetOrCreateAsync(12345, CancellationToken.None);

        session1.Id.Should().Be(session2.Id);
        session1.State.Should().Be(SessionStates.Idle);

        await using var context = await _factory.CreateDbContextAsync();
        var total = await context.UserSessions.CountAsync();

        total.Should().Be(1);
    }

    [Fact]
    public async Task SetStateAsync_ОбновляетСостояние()
    {
        await _service.GetOrCreateAsync(42, CancellationToken.None);

        var updated = await _service.SetStateAsync(42, SessionStates.PostText, "{\"value\":\"demo\"}", CancellationToken.None);

        updated.State.Should().Be(SessionStates.PostText);
        updated.PayloadJson.Should().Contain("demo");

        var loaded = await _service.GetAsync(42, CancellationToken.None);
        loaded.Should().NotBeNull();
        loaded!.State.Should().Be(SessionStates.PostText);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    private sealed class TestDbContextFactory : IDbContextFactory<AppDbContext>
    {
        private readonly DbContextOptions<AppDbContext> _options;
        private readonly SqliteConnection _connection;

        public TestDbContextFactory(DbContextOptions<AppDbContext> options, SqliteConnection connection)
        {
            _options = options;
            _connection = connection;
        }

        public AppDbContext CreateDbContext() => new(_options);

        public ValueTask<AppDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return new ValueTask<AppDbContext>(new AppDbContext(_options));
        }
    }
}

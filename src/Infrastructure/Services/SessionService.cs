using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class SessionService : ISessionService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SessionService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<UserSession?> GetAsync(long userId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        return await context.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<UserSession> GetOrCreateAsync(long userId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var session = await context.UserSessions
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (session is not null)
        {
            return session;
        }

        session = new UserSession
        {
            UserId = userId,
            State = SessionStates.Idle,
            UpdatedAt = DateTime.UtcNow
        };

        await context.UserSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task<UserSession> SetStateAsync(long userId, string state, string? payloadJson, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var session = await context.UserSessions
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (session is null)
        {
            session = new UserSession
            {
                UserId = userId
            };

            context.UserSessions.Add(session);
        }

        session.State = state;
        session.PayloadJson = payloadJson;
        session.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);

        return session;
    }

    public async Task ResetAsync(long userId, CancellationToken cancellationToken)
    {
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        var session = await context.UserSessions
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (session is null)
        {
            return;
        }

        session.State = SessionStates.Idle;
        session.PayloadJson = null;
        session.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync(cancellationToken);
    }
}

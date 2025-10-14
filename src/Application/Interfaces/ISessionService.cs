using Domain.Entities;

namespace Application.Interfaces;

public interface ISessionService
{
    Task<UserSession?> GetAsync(long userId, CancellationToken cancellationToken);

    Task<UserSession> GetOrCreateAsync(long userId, CancellationToken cancellationToken);

    Task<UserSession> SetStateAsync(long userId, string state, string? payloadJson, CancellationToken cancellationToken);

    Task ResetAsync(long userId, CancellationToken cancellationToken);
}

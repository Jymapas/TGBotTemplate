namespace Application.Interfaces;

public interface IRoleService
{
    bool IsAdmin(long userId);

    bool IsUser(long userId);

    IReadOnlyCollection<long> AdminIds { get; }
}

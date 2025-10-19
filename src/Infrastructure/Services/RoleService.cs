using Application.Interfaces;

namespace Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly HashSet<long> _adminIds;

    public RoleService(IEnumerable<long> adminIds)
    {
        _adminIds = new HashSet<long>(adminIds);
    }

    public bool IsAdmin(long userId) => _adminIds.Contains(userId);

    public bool IsUser(long userId) => !_adminIds.Contains(userId);

    public IReadOnlyCollection<long> AdminIds => _adminIds;
}

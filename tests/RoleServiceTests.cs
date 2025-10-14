using FluentAssertions;
using Infrastructure.Services;

namespace ApplicationTests;

public class RoleServiceTests
{
    [Fact]
    public void IsAdmin_ВозвращаетПравильныеЗначения()
    {
        var service = new RoleService(new[] { 1L, 2L });

        service.IsAdmin(1).Should().BeTrue();
        service.IsAdmin(3).Should().BeFalse();
    }

    [Fact]
    public void IsUser_ОтличаетАдминаОтПользователя()
    {
        var service = new RoleService(new[] { 7L });

        service.IsUser(7).Should().BeFalse();
        service.IsUser(8).Should().BeTrue();

        service.AdminIds.Should().ContainSingle(id => id == 7);
    }
}

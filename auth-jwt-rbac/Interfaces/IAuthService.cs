using auth_jwt_rbac.Dtos;
using auth_jwt_rbac.Entities;

namespace auth_jwt_rbac.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(UserDto req);
        Task<string?> LoginAsync(UserDto req);
    }
}

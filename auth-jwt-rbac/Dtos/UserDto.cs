namespace auth_jwt_rbac.Dtos
{
    public class UserDto
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}

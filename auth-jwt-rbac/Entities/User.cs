namespace auth_jwt_rbac.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Roles { get; set; } = "USER";
        public string RefreshToken { get; set; } = string.Empty;
        public DateTimeOffset? RefreshExpiresAt { get; set; }
        public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}

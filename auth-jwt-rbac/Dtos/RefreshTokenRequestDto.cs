namespace auth_jwt_rbac.Dtos
{
    public class RefreshTokenRequestDto
    {
        public Guid UserID { get; set; }
        public required string RefreshToken { get; set; }
    }
}

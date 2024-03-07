
using Services.Viewmodels;
using System.IdentityModel.Tokens.Jwt;


namespace DataAccess.ServiceRepository.IServiceRepository
{
    public interface IJwtRepository
    {
        string GenerateJwtToken(LoggedInPersonViewModel loggedInPerson);

        bool ValidateToken(string token , out JwtSecurityToken jwtSecurityToken);
    }
}

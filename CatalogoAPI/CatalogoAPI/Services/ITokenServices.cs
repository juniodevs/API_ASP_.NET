using CatalogoAPI.Models;

namespace CatalogoAPI.Services;
public interface ITokenServices
{
    string GerarToken(string key, string issuer, string audience, UserModel user);
}

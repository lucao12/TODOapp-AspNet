using Teste.Models;

namespace Teste.Interfaces
{
    public interface ITokenServices
    {
        string GenerateToken(User user);
    }
}

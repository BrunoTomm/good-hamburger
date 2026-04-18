using GoodHamburger.Domain.Auth;

namespace GoodHamburger.Application.Common.Interfaces;

public interface IJwtService
{
    string GerarToken(Usuario usuario);
}

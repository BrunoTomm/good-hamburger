using GoodHamburger.Domain.Common;

namespace GoodHamburger.Application.Common.Interfaces;

public interface IDbContextAccessor
{
    IEnumerable<BaseEntity> GetTrackedEntities();
}

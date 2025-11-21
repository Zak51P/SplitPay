using System.Threading;
using System.Threading.Tasks;

namespace SplitPay.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

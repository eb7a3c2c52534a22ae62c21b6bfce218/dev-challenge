using System.Threading;
using System.Threading.Tasks;

namespace Wool.DevChallenge.Api.Application.Commands.CalculateTrolleyTotalCommand
{
    public interface ITrolleyCalculationService
    {
        Task<decimal> GetTrolleyCalculations(CalculateTrolleyTotalCommand.RequestTrolley request, CancellationToken cancellationToken);
    }
}

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace ProjectArena.Infrastructure.Mongo.Operations
{
    internal interface IOperation
    {
        Task ProcessAsync(IClientSessionHandle session, CancellationToken token);
    }
}
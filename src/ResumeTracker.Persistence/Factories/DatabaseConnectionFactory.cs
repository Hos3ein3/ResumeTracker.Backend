using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ResumeTracker.Application.Abstractions.Persistence;

namespace ResumeTracker.Persistence.Factories;

public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    public Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

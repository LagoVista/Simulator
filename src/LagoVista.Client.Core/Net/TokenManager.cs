using LagoVista.Core.Networking.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.Net
{
    public class TokenManager : ITokenManager
    {
        public Task<bool> ValidateTokenAsync(IAuthManager authManager, CancellationTokenSource cancellationTokenSource)
        {
            return Task.FromResult(true);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.Net
{
    public interface IUDPClient : IDisposable
    {
        Task ConnectAsync(string host, int port);

        Task<byte[]> ReceiveAsync();

        Task<int> WriteAsync(byte[] buffer, int start, int length);

        Task<int> WriteAsync(string output);

        Task<int> WriteAsync<T>(T obj) where T : class;

        Task DisconnectAsync();
    }
}

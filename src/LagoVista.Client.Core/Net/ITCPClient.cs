using System;
using System.Threading.Tasks;

namespace LagoVista.Client.Core.Net
{
    public interface ITCPClient : IDisposable
    {
        Task Connect(string host, int port);

        Task<int> ReadAsync(byte[] buffer);

        Task WriteAsync(byte[] buffer, int start, int length);

        Task WriteAsync(string output);

        Task CloseAsync();
    }
}

using LagoVista.Client.Core.Auth;
using LagoVista.Client.Core.Models;
using LagoVista.Client.Core.Net;
using LagoVista.Client.Core.Resources;
using LagoVista.Core.Authentication.Interfaces;
using LagoVista.Core.Interfaces;
using LagoVista.Core.IOC;
using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.PlatformSupport;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LagoVista.Client.Core
{
    public static class Startup
    {
        public static void Init(ServerInfo serverInfo)
        {
            SLWIOC.RegisterSingleton<IAppStyle>(new AppStyle());
            SLWIOC.RegisterSingleton<ILogger>(new Loggers.MobileCenterLogger());


            SLWIOC.RegisterSingleton<ServerInfo>(serverInfo);
            SLWIOC.RegisterSingleton<IAuthManager, AuthManager>();
            SLWIOC.RegisterSingleton<ITokenManager, TokenManager>();
            SLWIOC.RegisterSingleton<IAuthClient>(new AuthClient());


            var client = new HttpClient();
            client.BaseAddress = serverInfo.BaseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            SLWIOC.RegisterSingleton<HttpClient>(client);

        }
    }
}

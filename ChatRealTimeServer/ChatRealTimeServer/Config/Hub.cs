using Microsoft.AspNetCore.SignalR;
using HubSignalR = Microsoft.AspNetCore.SignalR.Hub;

namespace ChatRealTimeServer.Config
{
    public class Hub : HubSignalR
    {
        public async Task NovaMensagem(string usuario, string mensagem)
        {
            await Clients.All.SendAsync("novaMensagem", usuario, mensagem);
        }
    }
}

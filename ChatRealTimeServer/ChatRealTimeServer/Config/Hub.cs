using Microsoft.AspNetCore.SignalR;
using HubSignalR = Microsoft.AspNetCore.SignalR.Hub;

namespace ChatRealTimeServer.Config
{
    public class Hub : HubSignalR
    {
        public async Task NewMessage(string user, string msg)
        {
            await Clients.All.SendAsync("ChatGeneral", user, msg);
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using HubSignalR = Microsoft.AspNetCore.SignalR.Hub;

namespace ChatRealTimeServer.Config
{
    public class Hub : HubSignalR
    {
        public async Task GeneralChat(string user, string msg)
        {
            await Clients.All.SendAsync("GeneralChat", user, msg);
        }

        public async Task PrivateChat(string s, string m, string? r)
        {
            if (m == "Disconnected" && ValidateUserExists(s))
            {
                var temp = ListUsers.Users.Where(x => x.Name.Equals(s)).FirstOrDefault();
               ListUsers.RemoveUser(temp);
            }



            if (r != null && ValidateUserExists(r))
                await Clients.User(s).SendAsync("PrivateChat", s, m);

        }
        public async Task<List<User>> OnlineUsers()
        {
            return ListUsers.Users;
        }

        public async Task AddUser(string code)
        {

            var newuser = new User(code);
            ListUsers.AddUser(newuser);

        }

        private bool ValidateUserExists(string? u)
        {
            var user = ListUsers.Users.Where(x => x.Name.Equals(u)).FirstOrDefault();
            return user != null ? true : false;
        }
    }

}


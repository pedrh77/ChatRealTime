using Microsoft.AspNetCore.SignalR;
namespace ChatRealTimeServer.Config
{
    public class Hub : Microsoft.AspNetCore.SignalR.Hub
    {
        public class Handle : Hub
        {
            private readonly IHubContext<Hub> _context;
            public Handle(IHubContext<Hub> context)
            {
                _context = context;
            }

            public async Task GeneralChat(string user, string msg)
            {
                await Clients.All.SendAsync("GeneralChat", user, msg);
            }

            public async Task PrivateChat(string s, string m, string? r)
            {
                if (r != null || ValidateUserExists(r))
                {
                    User? temp = ListUsers.Users.FirstOrDefault(x => x.Name.Equals(r));
                   
                  _context.Clients.Users(r).SendAsync("PrivateChat", s, m);
                };
            }

            public async Task AddUser(string code)
            {
                var test = _context.Clients.User(code);
               
                var newuser = new User(code, test);
                ListUsers.AddUser(newuser);
            }

            public async Task<List<User>> OnlineUsers()
            {
                return ListUsers.Users;
            }

            public async Task LogoutUser(string code)
            {
                if (ValidateUserExists(code))
                {
                    var temp = ListUsers.Users.Where(x => x.Name.Equals(code)).FirstOrDefault();
                    ListUsers.RemoveUser(temp);
                }
            }

            private bool ValidateUserExists(string? u)
            {
                var user = ListUsers.Users.Where(x => x.Name.Equals(u)).FirstOrDefault();
                return user != null ? true : false;
            }
        }
    }
}


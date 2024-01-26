using Microsoft.AspNetCore.SignalR;

namespace ChatRealTimeServer
{
    public record User(string Name, IClientProxy ConnectionId);


    public static class ListUsers
    {
        private static readonly object lockObject = new object();
        private static List<User> users = new List<User>();

        public static List<User> Users
        {
            get
            {
                foreach (var user in users.ToList())
                {
                    Console.WriteLine(user);
                }
                return users.ToList();
            }
        }

        public static void AddUser(User user)
        {
            users.Add(user);
        }

        public static void RemoveUser(User user)
        {
            users.Remove(user);
        }

        public static bool UserExists(string userName)
        {
           
            return users.Any(u => u.Name == userName);
        }
    }
}

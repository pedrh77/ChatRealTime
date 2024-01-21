using ChatRealTimeClient;
using Microsoft.AspNetCore.SignalR.Client;

await Menu();

async Task Menu()
{
    var name = CreateCodeUser();
    await AddUser(name);
    bool logout;
    do
    {
        Console.WriteLine("1- General Chat");
        Console.WriteLine("2- Privated Chat");
        Console.WriteLine("0- Log Out");
        var answer = Console.ReadLine();
        switch (answer)
        {
            case "1":
                await SendMessage(name, "ChatGeneral");
                logout = false;
                break;
            case "2":
                await SendMessage(name, "PrivateChat");
                logout = false;
                break;
            case "0":
                logout = true;
                break;
            default:
                logout = false;
                break;
        }
        Console.Clear();
    } while (logout == false);
    Logout(name);
}


async Task SendMessage(string code, string Channel)
{
    var messages = new List<Message>();
    var i = await HomeChat(code, Channel);
    int count = 0;

    while (true)
    {
        await using var connection = new HubConnectionBuilder()
                            .WithUrl("https://localhost:7036/chat").Build();
        connection.On<string, string>(Channel, async (n, m) =>
        {
            if (n != code)
            {
                Console.WriteLine("{0}: '{1}'", n, m);
                SaveMessage(messages, n, m);
            }
        });

        await connection.StartAsync();

        var msg = Console.ReadLine();

        if (msg.ToUpper().Equals("SAIR - CHAT"))
        {
            await ExitChat(code, Channel, connection, messages);
            break;
        }

        if (Channel.Equals("PrivateChat"))
        {
            await connection.InvokeAsync(Channel, code, msg, i);
            await connection.StopAsync();
        }
    }
}

List<Message> SaveMessage(List<Message> messagelist, string nome, string mensagem)
{
    messagelist.Add(new Message(nome, mensagem));
    return messagelist;
};

async Task SaveMessagesAsync(List<Message> lista, string nome)
{
    try
    {
        using (var writer = new StreamWriter(Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")), $"history-{nome}-{DateTime.UtcNow.ToString("dd.MM.yy.HH.mm.ss")}.txt"), true))
        {
            foreach (var mensagem in lista)
            {
                await writer.WriteLineAsync($"{mensagem.name}: {mensagem.message}");
            }
        }
    }
    catch (Exception ex)
    {
        throw ex;
    }
}

string CreateCodeUser()
{
    Random random = new Random();
    int randomNumber = random.Next(10, 1000);
    var randomString1 = (char)random.Next('A', 'Z' + 1);
    var randomString2 = (char)random.Next('A', 'Z' + 1);
    var randomString3 = (char)random.Next('A', 'Z' + 1);
    return $"{randomString1}{randomString2}-{randomNumber}";
}

async Task<List<User>> ListOnlineUsers(string code)
{
    await using var connection = new HubConnectionBuilder()
                             .WithUrl("https://localhost:7036/chat").Build();
    await connection.StartAsync();
    var users = await connection.InvokeAsync<List<User>>("OnlineUsers");
    var temp = users.Where(x => x.Name == code).FirstOrDefault();
    if (temp != null)
        users.Remove(temp);
    foreach (var user in users)
    {
        Console.WriteLine(user);
    }
    return users;
}


async Task AddUser(string code)
{
    await using var connection = new HubConnectionBuilder()
                            .WithUrl("https://localhost:7036/chat").Build();
    await connection.StartAsync();
    await connection.InvokeAsync("AddUser", code);
}

async Task ExitChat(string code, string channel, HubConnection connection, List<Message> messages)
{

    if (code.ToUpper() == "SAIR - CHAT")
    {
        Console.WriteLine("Save Messages:(Y|n)");
        string value = Console.ReadLine();
        if (value == null || value.ToUpper() == "Y") await SaveMessagesAsync(messages, code);
    }
}

async Task Logout(string code)
{
    await using var connection = new HubConnectionBuilder()
                           .WithUrl("https://localhost:7036/chat").Build();
    await connection.StartAsync();
    await connection.InvokeAsync("LogoutUser", code);
}


async Task<string> HomeChat(string code, string Channel)
{
    string i = null;
    User user;
    Console.Clear();
    Console.WriteLine($"Type 'Sair - Chat' to get out of the chat.\nYour UserCode is '{code}'.");
    if (Channel == "PrivateChat")
    {
        Console.WriteLine("Type a User code to type whith somone or see the list of users bellow.");
        Console.WriteLine("------------------------------------------------------");
        Console.WriteLine("Type '1' to see the List of Online Users or type the code of a Friend");

        while (true)
        {
            i = Console.ReadLine();
            if (i == "1")
            {
                var users = await ListOnlineUsers(code);
                i = Console.ReadLine();
                if (users.Any(x => i.Contains(x.Name)))
                {
                    Console.Clear();
                    Console.WriteLine($"Are you connecting whith...{i}");

                    return i;
                }
            }
        }
    }
    Console.WriteLine("------------------------------------------------------");
    return i;
}
using ChatRealTimeClient;
using Microsoft.AspNetCore.SignalR.Client;


await Menu();

async Task Menu()
{
    Console.WriteLine("Write your name:");
    var name = Console.ReadLine();
    bool logout;
    do
    {
        Console.WriteLine("1- Chat");
        Console.WriteLine("0- Log Out");

        var answer = Console.ReadLine();

        switch (answer)
        {
            case "1":
                await SendMessage($"{name}");
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

}
async Task SendMessage(string name)
{
    var messages = new List<Message>();
    Console.Clear();
    Console.WriteLine("Type 'Sair - Chat' to get out of the chat");
    Console.WriteLine("------------------------------------------------------");
    int count = 0;
    while (true)
    {
        await using var connection = new HubConnectionBuilder()
                           .WithUrl("https://localhost:7036/chat")
                           .Build();
        connection.On<string, string>("ChatGeneral", async (n, m) =>
        {
            if (n != name)
            {
                Console.WriteLine("{0}: '{1}'", n, m);

                SaveMessage(messages, n, m);

            }
        });
        await connection.StartAsync();

        string msg = Console.ReadLine();
        if (msg.ToUpper() == "SAIR - CHAT")
        {
            await connection.InvokeAsync("ChatGeneral", name, "Saiu do Chat");
            await connection.StopAsync();

            Console.WriteLine("Save Messages:(Y|n)");
            string value = Console.ReadLine();
            if (value == null || value.ToUpper() == "Y") await SaveMessagesAsync(messages, name);
            break;
        }
        while (count == 0)
        {
            await connection.InvokeAsync("ChatGeneral", name, "Entrou no Servidor");
            count = 1;
        }
        await connection.InvokeAsync("ChatGeneral", name, msg);
        SaveMessage(messages, name, msg);
        await connection.StopAsync();
    }
}

List<Message> SaveMessage(List<Message> messagelist, string nome, string mensagem)
{
    messagelist.Add(new Message(nome, mensagem));
    return messagelist;
};

async Task SaveMessagesAsync(List<Message> lista ,string nome)
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


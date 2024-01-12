using ChatRealTimeClient;
using Microsoft.AspNetCore.SignalR.Client;


await Menu();

async Task Menu()
{
    Console.WriteLine("Escreva Seu nome");
    var nome = Console.ReadLine();
    bool sair;
    do
    {
        Console.WriteLine("1- Chat");
        Console.WriteLine("0- Sair");

        var resp = Console.ReadLine();

        switch (resp)
        {
            case "1":
                await MandarMensagem($"{nome}");
                sair = false;
                break;
            case "0":
                sair = true;
                break;
            default:
                sair = false;
                break;
        }
        Console.Clear();
    } while (sair == false);

}
async Task MandarMensagem(string nome)
{
    var mensagens = new List<MensagemRequest>();
    Console.Clear();
    Console.WriteLine("Digite 'Sair - Chat' para encerrar o chat");
    Console.WriteLine("------------------------------------------------------");
    int count = 0;
    while (true)
    {
        await using var connection = new HubConnectionBuilder()
                           .WithUrl("https://localhost:7036/chat")
                           .Build();
        connection.On<string, string>("novaMensagem", async (n, m) =>
        {
            if (n != nome)
            {
                Console.WriteLine("{0}: '{1}'", n, m);

                SalvarMensagens(mensagens, n, m);

            }
        });
        await connection.StartAsync();

        string msg = Console.ReadLine();
        if (msg.ToUpper() == "SAIR - CHAT")
        {
            await connection.InvokeAsync("novaMensagem", nome, "Saiu do Chat");
            await connection.StopAsync();

            Console.WriteLine("Salvar Historico:(Y|n)");
            string value = Console.ReadLine();
            if (value == null || value.ToUpper() == "Y") await SalvarMensagensAsync(mensagens, nome);
            break;
        }
        while (count == 0)
        {
            await connection.InvokeAsync("novaMensagem", nome, "Entrou no Servidor");
            count = 1;
        }
        await connection.InvokeAsync("novaMensagem", nome, msg);
        SalvarMensagens(mensagens, nome, msg);
        await connection.StopAsync();
    }
}

List<MensagemRequest> SalvarMensagens(List<MensagemRequest> lista, string nome, string mensagem)
{
    lista.Add(new MensagemRequest(nome, mensagem));
    return lista;
};

async Task SalvarMensagensAsync(List<MensagemRequest> lista ,string nome)
{
    try
    {
        using (var writer = new StreamWriter(Path.Combine(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\")), $"history-{nome}-{DateTime.UtcNow.ToString("dd.MM.yy.HH.mm.ss")}.txt"), true))
        {

            foreach (var mensagem in lista)
            {
                await writer.WriteLineAsync($"{mensagem.Nome}: {mensagem.Mensagem}");
            }

        }
    }
    catch (Exception ex)
    {
        throw ex;
    }

}
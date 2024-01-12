namespace ChatRealTimeServer
{
    public class MensagemRequest
    {
        public string Nome { get; set; }
        public string Mensagem { get; set; }

        public MensagemRequest(string nome, string mensagem)
        {
            Nome = nome;
            Mensagem = mensagem;
        }
    }
}

namespace Chat.Domain.Interfaces
{
    public interface IChatServerService<TRequest, TResponse>
    {
        TResponse Process(TRequest message);
    }
}

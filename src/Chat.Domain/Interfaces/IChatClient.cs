namespace Chat.Domain.Interfaces
{
    public interface IChatClient<TRequest, TResponse>
    {
        TResponse Send(TRequest message);
    }
}

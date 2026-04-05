using Microsoft.EntityFrameworkCore;

public class TLogger()
{
    internal void LogError(object ex, string v, long? chatId)
    {
        throw new NotImplementedException();
    }

    internal void LogError(Exception exception, string v)
    {
        throw new NotImplementedException();
    }

    internal void LogWarning(Exception ex, string v, int v1)
    {
        throw new NotImplementedException();
    }

    internal void LogWarning(string v)
    {
        throw new NotImplementedException();
    }
}
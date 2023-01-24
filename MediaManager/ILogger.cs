namespace MediaManager;

public interface ILogger
{
    void LogError(string error);
    
    void LogInfo(string message);

    void LogFatal(string error);
}

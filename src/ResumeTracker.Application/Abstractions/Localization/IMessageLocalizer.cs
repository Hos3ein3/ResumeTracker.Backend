namespace ResumeTracker.Application.Abstractions.Localization;

public interface IMessageLocalizer
{
    string MessageResources(string key, params object[] args);
    string UserPreferencesResources(string key, params object[] args);
    string ExceptionResources(string key, params object[] args);
    //string Validation(string key, params object[] args);

}
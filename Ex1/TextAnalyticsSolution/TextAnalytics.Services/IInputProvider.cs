namespace TextAnalytics.Services;

public interface IInputProvider
{
    /// <summary>
    /// Zwraca tekst do analizy z danego źródła.
    /// </summary>
    string GetInput();
}
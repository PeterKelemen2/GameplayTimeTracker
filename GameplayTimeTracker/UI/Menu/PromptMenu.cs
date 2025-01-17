using System.Windows;

namespace GameplayTimeTracker.Menu;

public class PromptMenu : CustomMenu
{
    public enum PromptType
    {
        YesNo,
        Ok
    }

    private RoutedEventHandler _yesHandler;
    private RoutedEventHandler _noHandler;

    public PromptMenu(string[] text, double width = 300, double height = 400, PromptType type = PromptType.Ok,
        RoutedEventHandler yesHandler = null, RoutedEventHandler noHandler = null, bool performanceMode = true)
        : base(width, height, performanceMode)
    {
        
    }
}
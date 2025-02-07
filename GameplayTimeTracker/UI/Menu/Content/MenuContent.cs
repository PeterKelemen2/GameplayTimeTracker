using System.Windows.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace GameplayTimeTracker.UI.Menu.Content;

public class MenuContent : UserControl
{
    public ScrollViewer _scrollViewer { get; set; }
    public StackPanel _stackPanel { get; set; }

    public MenuContent()
    {
        _scrollViewer = new ScrollViewer { VerticalScrollBarVisibility = ScrollBarVisibility.Hidden, };

        _stackPanel = new StackPanel();
        _scrollViewer.Content = _stackPanel;
    }
}
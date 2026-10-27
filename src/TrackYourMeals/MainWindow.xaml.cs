using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace TrackYourMeals;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void RecentEntriesGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // O DataGrid consome a roda do mouse mesmo sem ter o que rolar;
        // nos limites, repassa o evento para o scroll da coluna lateral.
        var innerScroll = FindDescendant<ScrollViewer>((DependencyObject)sender);
        if (innerScroll is not null)
        {
            var atTop = e.Delta > 0 && innerScroll.VerticalOffset <= 0;
            var atBottom = e.Delta < 0 && innerScroll.VerticalOffset >= innerScroll.ScrollableHeight;
            if (!atTop && !atBottom)
            {
                return;
            }
        }

        e.Handled = true;
        SidePanelScroll.RaiseEvent(new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
        {
            RoutedEvent = MouseWheelEvent,
            Source = sender
        });
    }

    private static T? FindDescendant<T>(DependencyObject parent)
        where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T match)
            {
                return match;
            }

            var descendant = FindDescendant<T>(child);
            if (descendant is not null)
            {
                return descendant;
            }
        }

        return null;
    }
}

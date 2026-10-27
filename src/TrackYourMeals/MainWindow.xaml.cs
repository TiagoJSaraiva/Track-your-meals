using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TrackYourMeals.ViewModels;

namespace TrackYourMeals;

public partial class MainWindow : Window
{
    // Menor área (sem zoom) em que o layout cabe sem cortar elementos.
    private const double MinContentWidth = 1028;
    private const double MinContentHeight = 600;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Help_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        new HelpWindow { Owner = this }.ShowDialog();
    }

    private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control) || DataContext is not MainViewModel viewModel)
        {
            return;
        }

        var command = e.Delta > 0 ? viewModel.ZoomInCommand : viewModel.ZoomOutCommand;
        if (command.CanExecute(null))
        {
            command.Execute(null);
        }

        e.Handled = true;
    }

    private void RootScroll_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateRootContentSize();
    }

    private void RootScale_Changed(object? sender, EventArgs e)
    {
        UpdateRootContentSize();
    }

    private void UpdateRootContentSize()
    {
        // O ScrollViewer mede o conteúdo com espaço infinito, o que quebraria as
        // linhas "*" e o scroll interno das tabelas. Por isso o conteúdo recebe o
        // tamanho visível dividido pelo zoom, respeitando o tamanho mínimo; acima
        // dele, as barras de rolagem da janela aparecem.
        var zoom = RootScale.ScaleX;
        if (zoom <= 0 || RootScroll.ActualWidth <= 0 || RootScroll.ActualHeight <= 0)
        {
            return;
        }

        // A margem fica fora do LayoutTransform, então não é escalada.
        var margin = RootContent.Margin;
        var horizontalMargin = margin.Left + margin.Right;
        var verticalMargin = margin.Top + margin.Bottom;
        var minWidth = MinContentWidth * zoom + horizontalMargin;
        var minHeight = MinContentHeight * zoom + verticalMargin;

        var availableWidth = RootScroll.ActualWidth;
        var availableHeight = RootScroll.ActualHeight;

        // Uma barra de rolagem em um eixo reduz o espaço do outro.
        var overflowsX = minWidth > availableWidth;
        var overflowsY = minHeight > availableHeight;
        if (overflowsY)
        {
            availableWidth -= SystemParameters.VerticalScrollBarWidth;
            overflowsX = minWidth > availableWidth;
        }

        if (overflowsX)
        {
            availableHeight -= SystemParameters.HorizontalScrollBarHeight;
            if (!overflowsY && minHeight > availableHeight)
            {
                availableWidth -= SystemParameters.VerticalScrollBarWidth;
            }
        }

        RootContent.Width = Math.Max(MinContentWidth, Math.Floor((availableWidth - horizontalMargin) / zoom));
        RootContent.Height = Math.Max(MinContentHeight, Math.Floor((availableHeight - verticalMargin) / zoom));
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

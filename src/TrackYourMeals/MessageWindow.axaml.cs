using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TrackYourMeals;

public partial class MessageWindow : Window
{
    public MessageWindow()
    {
        InitializeComponent();
    }

    public MessageWindow(string message)
        : this()
    {
        MessageText.Text = message;
    }

    // Sem janela dona (ex.: erro antes da janela principal abrir), a mensagem abre centralizada na tela.
    public static Task ShowAsync(string message, Window? owner = null)
    {
        var window = new MessageWindow(message);
        if (owner is { IsVisible: true })
        {
            return window.ShowDialog(owner);
        }

        var closed = new TaskCompletionSource();
        window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        window.Closed += (_, _) => closed.TrySetResult();
        window.Show();
        return closed.Task;
    }

    private void Ok_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

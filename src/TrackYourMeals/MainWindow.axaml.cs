using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using CommunityToolkit.Mvvm.Input;
using TrackYourMeals.ViewModels;

namespace TrackYourMeals;

public partial class MainWindow : Window
{
    // Menor área (sem zoom) em que o layout cabe sem cortar elementos.
    private const double MinContentWidth = 1028;
    private const double MinContentHeight = 600;

    private readonly AsyncRelayCommand helpCommand;
    private MainViewModel? viewModel;

    public MainWindow()
    {
        InitializeComponent();

        helpCommand = new AsyncRelayCommand(OpenHelpAsync);

        // Tunnel: o Ctrl+roda vira zoom antes de qualquer controle rolar o conteúdo.
        AddHandler(PointerWheelChangedEvent, Window_PointerWheelChanged, RoutingStrategies.Tunnel);
        RootScroll.SizeChanged += (_, _) => UpdateRootContentSize();
    }

    // Ctrl no Windows e no Linux, Cmd no macOS.
    private KeyModifiers CommandModifier =>
        PlatformSettings?.HotkeyConfiguration.CommandModifiers ?? KeyModifiers.Control;

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (viewModel is not null)
        {
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        viewModel = DataContext as MainViewModel;
        RegisterKeyBindings();

        if (viewModel is not null)
        {
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        ApplyZoom();
    }

    private void RegisterKeyBindings()
    {
        KeyBindings.Clear();
        AddKeyBinding(Key.F1, KeyModifiers.None, helpCommand);

        if (viewModel is null)
        {
            return;
        }

        var modifier = CommandModifier;
        AddKeyBinding(Key.OemPlus, modifier, viewModel.ZoomInCommand);
        AddKeyBinding(Key.OemPlus, modifier | KeyModifiers.Shift, viewModel.ZoomInCommand);
        AddKeyBinding(Key.Add, modifier, viewModel.ZoomInCommand);
        AddKeyBinding(Key.OemMinus, modifier, viewModel.ZoomOutCommand);
        AddKeyBinding(Key.Subtract, modifier, viewModel.ZoomOutCommand);
        AddKeyBinding(Key.D0, modifier, viewModel.ResetZoomCommand);
        AddKeyBinding(Key.NumPad0, modifier, viewModel.ResetZoomCommand);
    }

    private void AddKeyBinding(Key key, KeyModifiers modifiers, ICommand command)
    {
        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(key, modifiers),
            Command = command
        });
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.ZoomLevel))
        {
            ApplyZoom();
        }
    }

    private async void Help_Click(object? sender, RoutedEventArgs e)
    {
        await OpenHelpAsync();
    }

    private Task OpenHelpAsync()
    {
        return new HelpWindow().ShowDialog(this);
    }

    private void Window_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!e.KeyModifiers.HasFlag(CommandModifier) || viewModel is null || e.Delta.Y == 0)
        {
            return;
        }

        var command = e.Delta.Y > 0 ? viewModel.ZoomInCommand : viewModel.ZoomOutCommand;
        if (command.CanExecute(null))
        {
            command.Execute(null);
        }

        e.Handled = true;
    }

    private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
    {
        // As linhas são recicladas entre itens, então as classes são recalculadas a cada carga.
        e.Row.Classes.Set("alt", e.Row.Index % 2 == 1);
        e.Row.Classes.Set("inactive", e.Row.DataContext is FoodRowViewModel { IsActive: false });
    }

    private void ApplyZoom()
    {
        var zoom = viewModel?.ZoomLevel ?? 1.0;
        RootTransform.LayoutTransform = new ScaleTransform(zoom, zoom);
        UpdateRootContentSize();
    }

    private void UpdateRootContentSize()
    {
        // O ScrollViewer mede o conteúdo com espaço infinito, o que quebraria as
        // linhas "*" e o scroll interno das tabelas. Por isso o conteúdo recebe o
        // tamanho visível dividido pelo zoom, respeitando o tamanho mínimo; acima
        // dele, as barras de rolagem da janela aparecem. No tema Fluent as barras
        // ficam sobre o conteúdo, então não descontam espaço.
        var zoom = viewModel?.ZoomLevel ?? 1.0;
        var available = RootScroll.Bounds.Size;
        if (zoom <= 0 || available.Width <= 0 || available.Height <= 0)
        {
            return;
        }

        // A margem fica fora do LayoutTransformControl, então não é escalada.
        var margin = RootTransform.Margin;
        var horizontalMargin = margin.Left + margin.Right;
        var verticalMargin = margin.Top + margin.Bottom;

        RootContent.Width = Math.Max(MinContentWidth, Math.Floor((available.Width - horizontalMargin) / zoom));
        RootContent.Height = Math.Max(MinContentHeight, Math.Floor((available.Height - verticalMargin) / zoom));
    }
}

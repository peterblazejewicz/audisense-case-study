using System.Windows;
using System.Windows.Threading;

using AudiSense.Wpf.Core.Services;

namespace AudiSense.Wpf.App;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer;
    private readonly INavigationService _navigationService;

    public MainWindow(INavigationService navigationService)
    {
        _navigationService = navigationService;
        InitializeComponent();

        // Initialize Timer
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (sender, args) =>
        {
            TimeText.Text = DateTime.Now.ToString("hh:mm:ss tt");
        };
        _timer.Start();

        // Set up navigation
        _navigationService.SetMainFrame(ContentFrame);

        // Navigate to landing page (hearing tests list) by default
        _navigationService.NavigateTo("hearingtests");
    }

    private void DashboardButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Dashboard opened.";
        // TODO: Show dashboard view
    }

    private void HearingTestButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Hearing Test module opened.";
        _navigationService.NavigateTo("hearingtests");
    }

    private void AudioAnalysisButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Audio Analysis module opened.";
        // Logic to display Audio Analysis Module
    }

    private void ReportsButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Reports section opened.";
        // Logic to display Reports Section
    }

    private void NewTestButton_Click(object sender, RoutedEventArgs e)
    {
        StatusText.Text = "Creating new hearing test...";
        _navigationService.NavigateTo("newhearingtest");
    }
}

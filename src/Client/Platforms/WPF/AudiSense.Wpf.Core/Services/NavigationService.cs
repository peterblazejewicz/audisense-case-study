using System.Windows.Controls;

using AudiSense.Wpf.Controls;
using AudiSense.Wpf.Core.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AudiSense.Wpf.Core.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NavigationService> _logger;
    private readonly Stack<string> _navigationHistory;
    private ContentControl? _mainFrame;

    public NavigationService(IServiceProvider serviceProvider, ILogger<NavigationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _navigationHistory = new Stack<string>();
    }

    public bool CanNavigateBack => _navigationHistory.Count > 1;

    public void SetMainFrame(ContentControl frame)
    {
        _mainFrame = frame;
        _logger.LogInformation("Main frame set for navigation");
    }

    public void NavigateTo(string viewName)
    {
        NavigateTo(viewName, null);
    }

    public void NavigateTo(string viewName, object? parameter)
    {
        if (_mainFrame == null)
        {
            _logger.LogError("Cannot navigate: Main frame not set");
            return;
        }

        try
        {
            var view = CreateView(viewName, parameter);
            if (view != null)
            {
                _mainFrame.Content = view;
                _navigationHistory.Push(viewName);
                _logger.LogInformation("Navigated to {ViewName}", viewName);
            }
            else
            {
                _logger.LogWarning("Failed to create view: {ViewName}", viewName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating to {ViewName}", viewName);
        }
    }

    public void NavigateBack()
    {
        if (!CanNavigateBack)
        {
            _logger.LogWarning("Cannot navigate back: No previous view");
            return;
        }

        // Remove current view
        _navigationHistory.Pop();

        // Get previous view
        var previousView = _navigationHistory.Peek();

        try
        {
            var view = CreateView(previousView, null);
            if (view != null && _mainFrame != null)
            {
                _mainFrame.Content = view;
                _logger.LogInformation("Navigated back to {ViewName}", previousView);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating back to {ViewName}", previousView);
        }
    }

    private UserControl? CreateView(string viewName, object? parameter)
    {
        return viewName.ToLower() switch
        {
            "hearingtests" or "landing" => CreateHearingTestsListView(),
            "newhearingtest" or "addhearingtest" => CreateNewHearingTestView(),
            "edithearingtest" => CreateEditHearingTestView(parameter),
            _ => null
        };
    }

    private UserControl CreateHearingTestsListView()
    {
        var viewModel = _serviceProvider.GetRequiredService<HearingTestsViewModel>();

        // Load data when navigating to the list - runs on UI thread to ensure proper binding updates
        _ = viewModel.LoadHearingTestsAsync();

        return new HearingTestsList
        {
            DataContext = viewModel
        };
    }

    private UserControl CreateNewHearingTestView()
    {
        var hearingTestsViewModel = _serviceProvider.GetRequiredService<HearingTestsViewModel>();

        // Reset the form for new entry
        hearingTestsViewModel.FormViewModel.Reset();

        // Unsubscribe any existing events first to prevent duplicates
        UnsubscribeFromFormEvents(hearingTestsViewModel.FormViewModel);

        // Subscribe to form events
        hearingTestsViewModel.FormViewModel.SaveRequested += OnNewTestSaveRequested;
        hearingTestsViewModel.FormViewModel.CancelRequested += OnFormCancelRequested;

        return new HearingTestForm
        {
            DataContext = hearingTestsViewModel.FormViewModel
        };
    }

    private UserControl CreateEditHearingTestView(object? parameter)
    {
        var hearingTestsViewModel = _serviceProvider.GetRequiredService<HearingTestsViewModel>();

        if (parameter is HearingTestItemViewModel hearingTestItem)
        {
            hearingTestsViewModel.FormViewModel.LoadFromResponse(hearingTestItem.ToHearingTestResponse());
        }

        // Unsubscribe any existing events first to prevent duplicates
        UnsubscribeFromFormEvents(hearingTestsViewModel.FormViewModel);

        // Subscribe to form events
        hearingTestsViewModel.FormViewModel.SaveRequested += OnEditTestSaveRequested;
        hearingTestsViewModel.FormViewModel.CancelRequested += OnFormCancelRequested;

        return new HearingTestForm
        {
            DataContext = hearingTestsViewModel.FormViewModel
        };
    }

    private async void OnNewTestSaveRequested(object? sender, HearingTestSaveEventArgs e)
    {
        var hearingTestsViewModel = _serviceProvider.GetRequiredService<HearingTestsViewModel>();

        // Update status to show progress
        hearingTestsViewModel.StatusMessage = "Creating hearing test...";

        try
        {
            var apiService = _serviceProvider.GetRequiredService<IHearingTestApiService>();
            var created = await apiService.CreateHearingTestAsync(e.Request);

            if (created != null)
            {
                _logger.LogInformation("New hearing test created successfully with ID: {Id}", created.Id);

                // Update status
                hearingTestsViewModel.StatusMessage = "Hearing test created successfully!";

                // Unsubscribe from events
                UnsubscribeFromFormEvents(hearingTestsViewModel.FormViewModel);

                // Navigate back to the list - this will refresh the data
                NavigateTo("hearingtests");
            }
            else
            {
                _logger.LogWarning("Failed to create hearing test - API returned null");
                hearingTestsViewModel.StatusMessage = "Failed to create hearing test. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating hearing test");
            hearingTestsViewModel.StatusMessage = $"Error creating hearing test: {ex.Message}";
        }
    }

    private async void OnEditTestSaveRequested(object? sender, HearingTestSaveEventArgs e)
    {
        var hearingTestsViewModel = _serviceProvider.GetRequiredService<HearingTestsViewModel>();

        try
        {
            if (e.HearingTestId.HasValue)
            {
                var apiService = _serviceProvider.GetRequiredService<IHearingTestApiService>();
                var updated = await apiService.UpdateHearingTestAsync(e.HearingTestId.Value, e.Request);

                if (updated != null)
                {
                    _logger.LogInformation("Hearing test updated successfully with ID: {Id}", e.HearingTestId.Value);

                    // Unsubscribe from events
                    UnsubscribeFromFormEvents(hearingTestsViewModel.FormViewModel);

                    // Navigate back to the list
                    NavigateTo("hearingtests");
                }
                else
                {
                    _logger.LogWarning("Failed to update hearing test with ID: {Id}", e.HearingTestId.Value);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating hearing test");
        }
    }

    private void OnFormCancelRequested(object? sender, EventArgs e)
    {
        var hearingTestsViewModel = _serviceProvider.GetRequiredService<HearingTestsViewModel>();

        // Unsubscribe from events
        UnsubscribeFromFormEvents(hearingTestsViewModel.FormViewModel);

        // Navigate back to the list
        NavigateTo("hearingtests");
    }

    private void UnsubscribeFromFormEvents(HearingTestFormViewModel formViewModel)
    {
        formViewModel.SaveRequested -= OnNewTestSaveRequested;
        formViewModel.SaveRequested -= OnEditTestSaveRequested;
        formViewModel.CancelRequested -= OnFormCancelRequested;
    }
}

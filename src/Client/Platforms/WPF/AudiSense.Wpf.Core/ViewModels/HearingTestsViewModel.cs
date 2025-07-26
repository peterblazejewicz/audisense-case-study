using System.Collections.ObjectModel;
using System.Windows.Input;

using AudiSense.Wpf.Core.Commands;
using AudiSense.Wpf.Core.Services;

using Microsoft.Extensions.Logging;

namespace AudiSense.Wpf.Core.ViewModels;

public class HearingTestsViewModel : BaseViewModel
{
    private readonly IHearingTestApiService _hearingTestService;
    private readonly ILogger<HearingTestsViewModel> _logger;
    private readonly Services.INavigationService? _navigationService;
    private bool _isLoading;
    private string _statusMessage = "Ready";
    private HearingTestItemViewModel? _selectedHearingTest;

    public HearingTestsViewModel(IHearingTestApiService hearingTestService, ILogger<HearingTestsViewModel> logger,
        Services.INavigationService? navigationService = null)
    {
        _hearingTestService = hearingTestService;
        _logger = logger;
        _navigationService = navigationService;

        HearingTests = new ObservableCollection<HearingTestItemViewModel>();
        FormViewModel = new HearingTestFormViewModel();

        LoadHearingTestsCommand = new RelayCommand(async () => await LoadHearingTestsAsync());
        NewHearingTestCommand = new RelayCommand(NavigateToNewHearingTest);
        EditHearingTestCommand = new RelayCommand<HearingTestItemViewModel>(NavigateToEditHearingTest, item => item != null);
        DeleteHearingTestCommand = new RelayCommand<HearingTestItemViewModel>(async item => await DeleteHearingTestAsync(item), item => item != null);
        RefreshCommand = new RelayCommand(async () => await LoadHearingTestsAsync());

        // Note: Event handlers are managed by the Navigation Service
        // FormViewModel.SaveRequested += OnFormSaveRequested;
        // FormViewModel.CancelRequested += OnFormCancelRequested;
    }

    public ObservableCollection<HearingTestItemViewModel> HearingTests { get; }
    public HearingTestFormViewModel FormViewModel { get; }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public HearingTestItemViewModel? SelectedHearingTest
    {
        get => _selectedHearingTest;
        set => SetProperty(ref _selectedHearingTest, value);
    }

    public bool IsFormVisible => FormViewModel.IsEditMode || !string.IsNullOrEmpty(FormViewModel.TesterName);

    public ICommand LoadHearingTestsCommand { get; }
    public ICommand NewHearingTestCommand { get; }
    public ICommand EditHearingTestCommand { get; }
    public ICommand DeleteHearingTestCommand { get; }
    public ICommand RefreshCommand { get; }

    public async Task LoadHearingTestsAsync()
    {
        IsLoading = true;
        StatusMessage = "Loading hearing tests...";

        try
        {
            var hearingTests = await _hearingTestService.GetAllHearingTestsAsync();

            HearingTests.Clear();
            foreach (var test in hearingTests.OrderByDescending(t => t.DateConducted))
            {
                HearingTests.Add(new HearingTestItemViewModel(test));
            }

            StatusMessage = $"Loaded {HearingTests.Count} hearing tests";
            _logger.LogInformation("Loaded {Count} hearing tests", HearingTests.Count);
        }
        catch (Exception ex)
        {
            StatusMessage = "Failed to load hearing tests";
            _logger.LogError(ex, "Error loading hearing tests");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void NavigateToNewHearingTest()
    {
        if (_navigationService != null)
        {
            _navigationService.NavigateTo("newhearingtest");
        }
        else
        {
            // Fallback for when navigation service is not available
            ShowNewHearingTestForm();
        }
    }

    private void NavigateToEditHearingTest(HearingTestItemViewModel? item)
    {
        if (item == null) return;

        if (_navigationService != null)
        {
            _navigationService.NavigateTo("edithearingtest", item);
        }
        else
        {
            // Fallback for when navigation service is not available
            ShowEditHearingTestForm(item);
        }
    }

    private void ShowNewHearingTestForm()
    {
        FormViewModel.Reset();
        OnPropertyChanged(nameof(IsFormVisible));
        StatusMessage = "Creating new hearing test";
    }

    private void ShowEditHearingTestForm(HearingTestItemViewModel? item)
    {
        if (item == null) return;

        FormViewModel.LoadFromResponse(item.ToHearingTestResponse());
        OnPropertyChanged(nameof(IsFormVisible));
        StatusMessage = $"Editing hearing test for {item.TesterName}";
    }

    private async Task DeleteHearingTestAsync(HearingTestItemViewModel? item)
    {
        if (item == null) return;

        StatusMessage = $"Deleting hearing test for {item.TesterName}...";

        try
        {
            var success = await _hearingTestService.DeleteHearingTestAsync(item.Id);
            if (success)
            {
                HearingTests.Remove(item);
                StatusMessage = "Hearing test deleted successfully";
                _logger.LogInformation("Deleted hearing test with ID: {Id}", item.Id);
            }
            else
            {
                StatusMessage = "Failed to delete hearing test";
                _logger.LogWarning("Failed to delete hearing test with ID: {Id}", item.Id);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = "Error deleting hearing test";
            _logger.LogError(ex, "Error deleting hearing test with ID: {Id}", item.Id);
        }
    }

    private async void OnFormSaveRequested(object? sender, HearingTestSaveEventArgs e)
    {
        StatusMessage = e.HearingTestId.HasValue ? "Updating hearing test..." : "Creating hearing test...";

        try
        {
            if (e.HearingTestId.HasValue)
            {
                // Update existing hearing test
                var updated = await _hearingTestService.UpdateHearingTestAsync(e.HearingTestId.Value, e.Request);
                if (updated != null)
                {
                    var existingItem = HearingTests.FirstOrDefault(ht => ht.Id == e.HearingTestId.Value);
                    if (existingItem != null)
                    {
                        existingItem.TesterName = updated.TesterName;
                        existingItem.DateConducted = updated.DateConducted;
                        existingItem.Result = updated.Result;
                    }
                    StatusMessage = "Hearing test updated successfully";
                    _logger.LogInformation("Updated hearing test with ID: {Id}", e.HearingTestId.Value);
                }
                else
                {
                    StatusMessage = "Failed to update hearing test";
                }
            }
            else
            {
                // Create new hearing test
                var created = await _hearingTestService.CreateHearingTestAsync(e.Request);
                if (created != null)
                {
                    HearingTests.Insert(0, new HearingTestItemViewModel(created));
                    StatusMessage = "Hearing test created successfully";
                    _logger.LogInformation("Created new hearing test with ID: {Id}", created.Id);
                }
                else
                {
                    StatusMessage = "Failed to create hearing test";
                }
            }

            FormViewModel.Reset();
            OnPropertyChanged(nameof(IsFormVisible));
        }
        catch (Exception ex)
        {
            StatusMessage = "Error saving hearing test";
            _logger.LogError(ex, "Error saving hearing test");
        }
    }

    private void OnFormCancelRequested(object? sender, EventArgs e)
    {
        FormViewModel.Reset();
        OnPropertyChanged(nameof(IsFormVisible));
        StatusMessage = "Ready";
    }
}

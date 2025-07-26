using System.Windows.Input;

using AudiSense.Contracts.Requests;
using AudiSense.Contracts.Responses;
using AudiSense.Wpf.Core.Commands;

namespace AudiSense.Wpf.Core.ViewModels;

public class HearingTestFormViewModel : BaseViewModel
{
    private string _testerName = string.Empty;
    private DateTime _dateConducted = DateTime.Now;
    private string _result = string.Empty;
    private bool _isEditMode;
    private int _hearingTestId;

    public HearingTestFormViewModel()
    {
        SaveCommand = new RelayCommand(Save, CanSave);
        CancelCommand = new RelayCommand(Cancel);
    }

    public int HearingTestId
    {
        get => _hearingTestId;
        set => SetProperty(ref _hearingTestId, value);
    }

    public string TesterName
    {
        get => _testerName;
        set => SetProperty(ref _testerName, value);
    }

    public DateTime DateConducted
    {
        get => _dateConducted;
        set => SetProperty(ref _dateConducted, value);
    }

    public string Result
    {
        get => _result;
        set => SetProperty(ref _result, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string Title => IsEditMode ? "Edit Hearing Test" : "New Hearing Test";

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public event EventHandler<HearingTestSaveEventArgs>? SaveRequested;
    public event EventHandler? CancelRequested;

    public void LoadFromResponse(HearingTestResponse response)
    {
        HearingTestId = response.Id;
        TesterName = response.TesterName;
        DateConducted = response.DateConducted;
        Result = response.Result;
        IsEditMode = true;
        OnPropertyChanged(nameof(Title));
    }

    public void Reset()
    {
        HearingTestId = 0;
        TesterName = string.Empty;
        DateConducted = DateTime.Now;
        Result = string.Empty;
        IsEditMode = false;
        OnPropertyChanged(nameof(Title));
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(TesterName) &&
               !string.IsNullOrWhiteSpace(Result) &&
               DateConducted <= DateTime.Now &&
               TesterName.Trim().Length >= 2 &&
               Result.Trim().Length >= 5;
    }

    private void Save()
    {
        var request = new HearingTestRequest
        {
            TesterName = TesterName.Trim(),
            DateConducted = DateConducted,
            Result = Result.Trim()
        };

        SaveRequested?.Invoke(this, new HearingTestSaveEventArgs(request, IsEditMode ? HearingTestId : null));
    }

    private void Cancel()
    {
        CancelRequested?.Invoke(this, EventArgs.Empty);
    }
}

public class HearingTestSaveEventArgs : EventArgs
{
    public HearingTestRequest Request { get; }
    public int? HearingTestId { get; }

    public HearingTestSaveEventArgs(HearingTestRequest request, int? hearingTestId = null)
    {
        Request = request;
        HearingTestId = hearingTestId;
    }
}

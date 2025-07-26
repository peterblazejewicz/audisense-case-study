using AudiSense.Contracts.Responses;

namespace AudiSense.Wpf.Core.ViewModels;

public class HearingTestItemViewModel : BaseViewModel
{
    private readonly HearingTestResponse _hearingTest;

    public HearingTestItemViewModel(HearingTestResponse hearingTest)
    {
        _hearingTest = hearingTest;
    }

    public int Id => _hearingTest.Id;

    public string TesterName
    {
        get => _hearingTest.TesterName;
        set
        {
            if (_hearingTest.TesterName != value)
            {
                _hearingTest.TesterName = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime DateConducted
    {
        get => _hearingTest.DateConducted;
        set
        {
            if (_hearingTest.DateConducted != value)
            {
                _hearingTest.DateConducted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DateConductedFormatted));
                OnPropertyChanged(nameof(DaysAgo));
            }
        }
    }

    public string Result
    {
        get => _hearingTest.Result;
        set
        {
            if (_hearingTest.Result != value)
            {
                _hearingTest.Result = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ResultPreview));
            }
        }
    }

    public string DateConductedFormatted => DateConducted.ToString("MMM dd, yyyy HH:mm");

    public string DaysAgo
    {
        get
        {
            var days = (DateTime.Now - DateConducted).Days;
            return days switch
            {
                0 => "Today",
                1 => "Yesterday",
                _ => $"{days} days ago"
            };
        }
    }

    public string ResultPreview
    {
        get
        {
            if (string.IsNullOrEmpty(Result))
                return "No result";

            return Result.Length > 100 ? $"{Result[..100]}..." : Result;
        }
    }

    public HearingTestResponse ToHearingTestResponse()
    {
        return new HearingTestResponse
        {
            Id = Id,
            TesterName = TesterName,
            DateConducted = DateConducted,
            Result = Result
        };
    }
}

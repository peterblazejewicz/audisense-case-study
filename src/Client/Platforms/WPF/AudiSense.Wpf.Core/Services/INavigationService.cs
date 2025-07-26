using System.Windows.Controls;

namespace AudiSense.Wpf.Core.Services;

/// <summary>
/// Interface for navigation service to manage WPF view transitions
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigate to a specific view
    /// </summary>
    /// <param name="viewName">Name of the view to navigate to</param>
    void NavigateTo(string viewName);

    /// <summary>
    /// Navigate to a specific view with parameters
    /// </summary>
    /// <param name="viewName">Name of the view to navigate to</param>
    /// <param name="parameter">Parameter to pass to the view</param>
    void NavigateTo(string viewName, object parameter);

    /// <summary>
    /// Navigate back to the previous view
    /// </summary>
    void NavigateBack();

    /// <summary>
    /// Check if navigation back is possible
    /// </summary>
    bool CanNavigateBack { get; }

    /// <summary>
    /// Set the main content frame for navigation
    /// </summary>
    /// <param name="frame">The main content frame</param>
    void SetMainFrame(ContentControl frame);
}

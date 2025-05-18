using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controller for the login form UI component.
/// Handles login logic and UI interactions.
/// </summary>
public partial class LoginForm : MonoBehaviour
{
    [SerializeField]
    private UIDocument Document;
    
    /// <summary>
    /// Called when the component is enabled.
    /// Initializes the document and sets up event listeners.
    /// </summary>
    private void OnEnable()
    {
        // This method will be called by the auto-generated code
        InitializeDocument();
        
        // Add event listeners
        if (LoginButton != null)
        {
            LoginButton.clicked += OnLoginButtonClicked;
        }
        
        if (CancelButton != null)
        {
            CancelButton.clicked += OnCancelButtonClicked;
        }
    }
    
    /// <summary>
    /// Called when the component is disabled.
    /// Removes event listeners to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        // Remove event listeners
        if (LoginButton != null)
        {
            LoginButton.clicked -= OnLoginButtonClicked;
        }
        
        if (CancelButton != null)
        {
            CancelButton.clicked -= OnCancelButtonClicked;
        }
    }
    
    /// <summary>
    /// Handles the login button click event.
    /// Validates input fields and performs login logic.
    /// </summary>
    private void OnLoginButtonClicked()
    {
        string username = UsernameInput.value;
        string password = PasswordInput.value;
        
        Debug.Log($"Login attempted with username: {username}");
        
        // Here you would add authentication logic
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogWarning("Username or password cannot be empty");
            return;
        }
        
        // Simulate successful login
        Debug.Log("Login successful!");
    }
    
    /// <summary>
    /// Handles the cancel button click event.
    /// Clears all input fields and logs cancellation.
    /// </summary>
    private void OnCancelButtonClicked()
    {
        // Clear input fields
        UsernameInput.value = "";
        PasswordInput.value = "";
        
        Debug.Log("Login cancelled");
    }
}
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Utility class to help work with Rosalina-generated code.
/// Provides utility methods for working with UI Toolkit and Rosalina.
/// </summary>
public static class RosalinaBridge
{
    /// <summary>
    /// Binds a UXML file to a UI Document and initializes the controller.
    /// </summary>
    /// <param name="document">The UI Document to bind to.</param>
    /// <param name="controller">The controller with Rosalina-generated code.</param>
    /// <param name="visualTreeAsset">The Visual Tree Asset (UXML) to load.</param>
    /// <param name="styleSheet">Optional StyleSheet to apply.</param>
    public static void BindUIDocument<T>(UIDocument document, T controller, VisualTreeAsset visualTreeAsset, StyleSheet styleSheet = null) 
        where T : Component
    {
        if (document == null || visualTreeAsset == null)
        {
            Debug.LogError("UIDocument or VisualTreeAsset is null!");
            return;
        }
        
        // Set the source asset
        document.visualTreeAsset = visualTreeAsset;
        
        // Apply stylesheet if provided
        if (styleSheet != null)
        {
            VisualElement root = document.rootVisualElement;
            root.styleSheets.Add(styleSheet);
        }
        
        Debug.Log($"UI Document bound to {visualTreeAsset.name}");
    }
    
    /// <summary>
    /// Creates a panel settings object for runtime UI.
    /// </summary>
    /// <returns>A new PanelSettings object.</returns>
    public static PanelSettings CreateDefaultPanelSettings()
    {
        PanelSettings settings = ScriptableObject.CreateInstance<PanelSettings>();
        settings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        settings.referenceResolution = new Vector2Int(1920, 1080);
        settings.match = 0.5f;
        return settings;
    }
} 
using UnityEngine;
using UnityEngine.UIElements;

public class MainUISetup : MonoBehaviour
{
    [SerializeField]
    private UIDocument mainDocument;
    
    [SerializeField]
    private VisualTreeAsset loginFormAsset;
    
    [SerializeField]
    private StyleSheet loginFormStyleSheet;
    
    private void OnEnable()
    {
        if (mainDocument == null)
        {
            Debug.LogError("UIDocument is not assigned!");
            return;
        }
        
        VisualElement root = mainDocument.rootVisualElement;
        
        // Clear any existing content
        root.Clear();
        
        // Apply USS to the document
        if (loginFormStyleSheet != null)
        {
            root.styleSheets.Add(loginFormStyleSheet);
        }
        
        // Instantiate login form
        if (loginFormAsset != null)
        {
            TemplateContainer loginForm = loginFormAsset.Instantiate();
            root.Add(loginForm);
        }
        
        // Center the login form
        root.style.alignItems = Align.Center;
        root.style.justifyContent = Justify.Center;
        root.style.flexGrow = 1;
    }
} 
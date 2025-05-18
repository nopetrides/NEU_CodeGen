using UnityEngine;
using UnityEngine.UIElements;

namespace Core
{
    /// <summary>
    /// Base class for all UI components in the game.
    /// </summary>
    public abstract class UIComponent
    {
        protected UIDocument Document { get; private set; }
        protected VisualElement Root { get; private set; }

        /// <summary>
        /// Initializes the UI component.
        /// </summary>
        public virtual void Initialize()
        {
            // Create UI Document GameObject
            var go = new GameObject($"{GetType().Name}");
            Document = go.AddComponent<UIDocument>();

            // Add a UIComponentUpdater to call Update
            var updater = go.AddComponent<UIComponentUpdater>();
            updater.SetUIComponent(this);

            // Set panel settings
            Document.panelSettings = Game.UI.PanelSettings;

            // Load and set UXML
            var uxmlPath = $"UI/Components/{GetType().Name}";
            var uxml = Resources.Load<VisualTreeAsset>(uxmlPath);
            if (uxml == null)
            {
                Debug.LogError($"Failed to load UXML at path: {uxmlPath}");
                return;
            }
            Document.visualTreeAsset = uxml;

            // Load and set USS
            var ussPath = $"UI/Components/{GetType().Name}";
            var uss = Resources.Load<StyleSheet>(ussPath);
            if (uss != null)
            {
                Document.rootVisualElement.styleSheets.Add(uss);
            }

            // Get root element
            Root = Document.rootVisualElement;

            // Hide by default
            Root.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Shows the UI component.
        /// </summary>
        public virtual void Show()
        {
            if (Root != null)
            {
                Root.style.display = DisplayStyle.Flex;
            }
        }

        /// <summary>
        /// Hides the UI component.
        /// </summary>
        public virtual void Hide()
        {
            if (Root != null)
            {
                Root.style.display = DisplayStyle.None;
            }
        }

        /// <summary>
        /// Sets the UIDocument for this UI component.
        /// </summary>
        /// <param name="document">The UIDocument to set</param>
        protected void SetDocument(UIDocument document)
        {
            Document = document;
            Root = document.rootVisualElement;
        }

        /// <summary>
        /// Called every frame to update the UI component.
        /// </summary>
        public virtual void Update()
        {
            // Override in derived classes to implement update logic
        }
    }
} 

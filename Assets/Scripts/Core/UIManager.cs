using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// Manages UI components and their resources throughout the game lifecycle.
    /// </summary>
    public class UIManager
    {
        private class UIResourceState
        {
            public UIDocument Document { get; set; }
            public Dictionary<string, Texture2D> Textures { get; set; } = new();
            public bool IsLoaded { get; set; }
            public VisualTreeAsset Uxml { get; set; }
            public StyleSheet Uss { get; set; }
        }

        private PanelSettings _panelSettings;
        private Dictionary<Type, UIResourceState> _uiStates = new();
        private Dictionary<Type, UIComponent> _activeUIs = new();
        private Stack<UIComponent> _navigationStack = new();

        public PanelSettings PanelSettings => _panelSettings;

        public UIManager()
        {
            _panelSettings = Resources.Load<PanelSettings>("UI/Settings/DefaultPanelSettings");
            if (_panelSettings == null)
            {
                Debug.LogError("Failed to load default panel settings!");
            }
        }

        /// <summary>
        /// Shows a UI component, replacing the current content.
        /// </summary>
        /// <typeparam name="T">Type of UI component to show</typeparam>
        public void Show<T>() where T : UIComponent, new()
        {
            var type = typeof(T);

            // Load resources if needed
            LoadUIResources<T>();

            // Create or get UI component
            if (!_activeUIs.TryGetValue(type, out var ui))
            {
                ui = new T();
                ui.Initialize();
                _activeUIs[type] = ui;
            }

            // Clear navigation stack and show new UI
            while (_navigationStack.Count > 0)
            {
                var current = _navigationStack.Pop();
                current.Hide();
            }

            _navigationStack.Push(ui);
            ui.Show();
        }

        /// <summary>
        /// Pushes a new UI component onto the navigation stack.
        /// </summary>
        /// <typeparam name="T">Type of UI component to push</typeparam>
        public void Push<T>() where T : UIComponent, new()
        {
            var type = typeof(T);

            // Load resources if needed
            LoadUIResources<T>();

            // Create or get UI component
            if (!_activeUIs.TryGetValue(type, out var ui))
            {
                ui = new T();
                ui.Initialize();
                _activeUIs[type] = ui;
            }

            // Hide current UI if any
            if (_navigationStack.Count > 0)
            {
                _navigationStack.Peek().Hide();
            }

            _navigationStack.Push(ui);
            ui.Show();
        }

        /// <summary>
        /// Pops the current UI component from the navigation stack.
        /// </summary>
        /// <returns>True if a component was popped, false otherwise</returns>
        public bool Pop()
        {
            if (_navigationStack.Count <= 1)
            {
                Debug.LogWarning("Can't pop the last or only component!");
                return false;
            }

            var current = _navigationStack.Pop();
            current.Hide();

            var previous = _navigationStack.Peek();
            previous.Show();

            return true;
        }

        /// <summary>
        /// Hides a UI component and optionally unloads its resources.
        /// </summary>
        /// <typeparam name="T">Type of UI component to hide</typeparam>
        /// <param name="unloadResources">Whether to unload the UI's resources</param>
        public void Hide<T>(bool unloadResources = true) where T : UIComponent
        {
            var type = typeof(T);

            if (_activeUIs.TryGetValue(type, out var ui))
            {
                ui.Hide();

                if (unloadResources)
                {
                    UnloadUIResources<T>();
                }
            }
        }

        /// <summary>
        /// Gets an active UI component by type.
        /// </summary>
        /// <typeparam name="T">Type of UI component to get</typeparam>
        /// <returns>The UI component, or null if not active</returns>
        public T GetActiveUI<T>() where T : UIComponent
        {
            var type = typeof(T);

            if (_activeUIs.TryGetValue(type, out var ui))
            {
                return (T)ui;
            }

            return null;
        }

        private void LoadUIResources<T>() where T : UIComponent
        {
            var type = typeof(T);
            if (!_uiStates.TryGetValue(type, out var state))
            {
                state = new UIResourceState();
                _uiStates[type] = state;
            }

            if (!state.IsLoaded)
            {
                // Load UXML and USS resources on the main thread
                var uxmlPath = $"UI/Components/{type.Name}";
                state.Uxml = Resources.Load<VisualTreeAsset>(uxmlPath);
                if (state.Uxml == null)
                {
                    Debug.LogError($"Failed to load UXML at path: {uxmlPath}");
                    return;
                }

                var ussPath = $"UI/Components/{type.Name}";
                state.Uss = Resources.Load<StyleSheet>(ussPath);

                state.IsLoaded = true;
            }
        }

        private void UnloadUIResources<T>() where T : UIComponent
        {
            var type = typeof(T);
            if (_uiStates.TryGetValue(type, out var state))
            {
                // Unload textures
                foreach (var texture in state.Textures.Values)
                {
                    if (texture != null)
                    {
                        Resources.UnloadAsset(texture);
                    }
                }
                state.Textures.Clear();

                // Clear document reference
                if (state.Document != null)
                {
                    UnityEngine.Object.Destroy(state.Document.gameObject);
                    state.Document = null;
                }

                state.IsLoaded = false;
            }
        }
    }
} 

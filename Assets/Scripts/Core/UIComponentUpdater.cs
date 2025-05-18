using UnityEngine;

namespace Core
{
    /// <summary>
    /// MonoBehaviour that calls Update on a UIComponent.
    /// </summary>
    public class UIComponentUpdater : MonoBehaviour
    {
        private UIComponent _uiComponent;

        /// <summary>
        /// Sets the UIComponent to update.
        /// </summary>
        /// <param name="uiComponent">The UIComponent to update</param>
        public void SetUIComponent(UIComponent uiComponent)
        {
            _uiComponent = uiComponent;
        }

        private void Update()
        {
            if (_uiComponent != null)
            {
                _uiComponent.Update();
            }
        }
    }
}
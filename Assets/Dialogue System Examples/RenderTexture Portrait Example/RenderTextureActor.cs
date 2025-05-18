using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// When this actor speaks, points a portrait camera at its portrait model.
/// The portrait model should be located outside of the main gameplay area.
/// The portrait camera should render to a RenderTexture that can be shown
/// in the dialogue UI.
/// </summary>
public class RenderTextureActor : MonoBehaviour
{
    public Camera portraitCamera;
    public Transform portraitCameraPosition;

    void OnConversationLine(Subtitle subtitle)
    {
        if (subtitle.speakerInfo.transform == this.transform)
        {
            portraitCamera.transform.position = portraitCameraPosition.position;
            portraitCamera.transform.rotation = portraitCameraPosition.rotation;
        }
    }
}


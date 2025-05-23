//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the Rosalina Code Generator tool.
//     Version: 4.0.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UIElements;

public partial class LoginForm
{
    [SerializeField]
    private UIDocument _document;
    public Label UsernameLabel { get; private set; }

    public TextField UsernameInput { get; private set; }

    public Label PasswordLabel { get; private set; }

    public TextField PasswordInput { get; private set; }

    public Button LoginButton { get; private set; }

    public Button CancelButton { get; private set; }

    public Label TitleLabel { get; private set; }

    public VisualElement UsernameContainer { get; private set; }

    public VisualElement PasswordContainer { get; private set; }

    public VisualElement ButtonsContainer { get; private set; }

    public VisualElement LoginContainer { get; private set; }

    public VisualElement Root => _document?.rootVisualElement;
    public void InitializeDocument()
    {
        UsernameLabel = Root?.Q<Label>("username-label");
        UsernameInput = Root?.Q<TextField>("username-input");
        PasswordLabel = Root?.Q<Label>("password-label");
        PasswordInput = Root?.Q<TextField>("password-input");
        LoginButton = Root?.Q<Button>("login-button");
        CancelButton = Root?.Q<Button>("cancel-button");
        TitleLabel = Root?.Q<Label>("title-label");
        UsernameContainer = Root?.Q<VisualElement>("username-container");
        PasswordContainer = Root?.Q<VisualElement>("password-container");
        ButtonsContainer = Root?.Q<VisualElement>("buttons-container");
        LoginContainer = Root?.Q<VisualElement>("login-container");
    }
}
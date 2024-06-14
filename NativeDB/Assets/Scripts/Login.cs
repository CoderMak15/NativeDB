using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : Window
{
    [SerializeField] private TMP_InputField _username;
    [SerializeField] private TMP_InputField _password;

    [SerializeField] private Window _menu;

    public void TryConfirm()
    {
        Client c =  Socket._instance.TryLogin(_username.text, _password.text);
        if (c != null)
        {
            UI._instance.Open(_menu);
        }
    }
}
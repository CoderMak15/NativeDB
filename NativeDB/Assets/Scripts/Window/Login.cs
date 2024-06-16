using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : Window
{
    [SerializeField] private TMP_InputField _username;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] private TextMeshProUGUI _error;

    [SerializeField] private Window _menu;

    public void TryConfirm()
    {
        Client c = Socket._instance.TryLoginSignin<Client>("Login", _username.text, _password.text);
        if (c != null)
        {
            UI._instance.Open(_menu);
        }
        else
        {
            _error.text = "Username doesn't exist or invalid credentials";
        }
    }

    public void TrySignin()
    {
        if (_password.text.Length < 8)
        {
            _error.text = "Password must be at least 8 characters";
            return;
        }

        bool result = Socket._instance.TryLoginSignin<bool>("Create", _username.text, _password.text);
        if (result)
        {
            UI._instance.Open(_menu);
        }
        else
        {
            _error.text = "Username already exists";
        }
    }
}
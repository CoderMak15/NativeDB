using TMPro;
using UnityEngine;

public class Login : Window
{
    public static Client _currentClient;

    [SerializeField] private TMP_InputField _username;
    [SerializeField] private TMP_InputField _password;
    [SerializeField] private TextMeshProUGUI _error;

    [SerializeField] private Window _menu;

    public void TryConfirm()
    {
        Client[] rows = Socket._instance.TryLoginSignin<Client[]>("Login", _username.text, _password.text);
        if (rows == null || rows.Length == 0)
        {
            _error.color = Color.red;
            _error.text = "Username doesn't exist or invalid credentials";
        }
        else if (rows[0] != null)
        {
            _currentClient = rows[0];
            UI._instance.Open(_menu);
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

        if (!result)
        {
            _error.color = Color.red;
            _error.text = "Username already exists";
        }
        else
        {
            _error.color = Color.green;
            _error.text = "User successfully create";
        }
    }
}
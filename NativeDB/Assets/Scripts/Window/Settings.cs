using com.rac.network;
using com.rac.ui;
using TMPro;
using UnityEngine;

namespace com.rac.core
{
    public class Settings : Window
    {
        [SerializeField] private Window _catalog;

        [Header("User Info")]
        [SerializeField] private TMP_InputField _name;
        [SerializeField] private TMP_InputField _lastName;
        [SerializeField] private TMP_InputField _email;
        [SerializeField] private TMP_InputField _username;
        [SerializeField] private TMP_InputField _password;

        [SerializeField] private TextMeshProUGUI _error;

        public override void Init(object parameter)
        {
            base.Init(parameter);
            _name.text = Login._currentClient.name;
            _lastName.text = Login._currentClient.surname;
            _email.text = Login._currentClient.email;
            _username.text = Login._currentClient.username;
            _password.text = "";
        }

        public void Save()
        {
            Client temp = new()
            {
                id = Login._currentClient.id,
                name = _name.text,
                surname = _lastName.text,
                email = _email.text,
                username = _username.text == "" ? Login._currentClient.username : _username.text,
                password = _password.text == "" ? Login._currentClient.password : _password.text,
            };

            if (temp.username != Login._currentClient.username)
            {
                string password = temp.password == "" ? Login._currentClient.password : temp.password;
                if (!Socket._instance.TryLoginSignin<bool>("Create", _username.text, password))
                {
                    _error.color = Color.red;
                    _error.text = "Username already in use";
                    return;
                }
            }

            if (Socket._instance.TrySaveChanges(temp))
            {
                _error.color = Color.green;
                _error.text = "User successfully updated";
                Login._currentClient = temp;
            }
            else
            {
                _error.color = Color.red;
                _error.text = "Something went wrong";
            }
        }

        public void Quit()
        {
            UI._instance.Open(_catalog);
        }
    }
}
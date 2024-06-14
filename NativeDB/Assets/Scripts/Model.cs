[System.Serializable]
public class LoginResponse
{
    public int status;
    public Client payload;
}

[System.Serializable]
public class Client
{
    public int id;
    public string username;
    public string password;
    public bool access;
    public string name;
    public string surname;
    public string email;
    public System.DateTime date;
}

[System.Serializable]
public class LoginInfo
{
    public string username;
    public string password;
}

[System.Serializable]
public class LoginPayload
{
    public string message;
    public LoginInfo login;
}
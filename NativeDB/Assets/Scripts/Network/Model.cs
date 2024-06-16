[System.Serializable]
public class Response<T>
{
    public int status;
    public T payload;
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
public class Movie
{
    public string name;
    public string description;
    public string url;
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

[System.Serializable]
public class Request
{
    public string message;
}
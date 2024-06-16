using System.Net.Sockets;
using UnityEngine;

public class Socket : MonoBehaviour
{
    public static Socket _instance { get; private set; }

    private TcpClient _client = null;
    private const string URI = "10.0.0.121";
    private const int PORT = 3000;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Connect();
    }

    private void OnDestroy()
    {
        Disconnect();
        _instance = null;
    }

    public void Connect()
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(URI, PORT);
        }
        catch (System.Exception e)
        {
            Debug.Log($"Exception: {e.Message}");
        }
    }

    public void Disconnect()
    {
        _client?.Close();
        _client?.Dispose();
        _client = null;
    }

    public T TryLoginSignin<T>(string loginType, string username, string password)
    {
        LoginPayload payload = new()
        {
            message = loginType,
            login = new()
            {
                username = username,
                password = password
            }
        };

        Response<T> logResult = SendMessage<Response<T>>(payload);

        Debug.Log("Status : " + logResult.status);

        if (logResult.status == 200)
        {
            return logResult.payload;
        }

        return default(T);
    }

    public Movie[] GetMovies()
    {
        Request payload = new()
        {
            message = "Movies"
        };

        Response<Movie[]> logResult = SendMessage<Response<Movie[]>>(payload);
        if (logResult.status == 200)
        {
            return logResult.payload;
        }

        return null;
    }

    private T SendMessage<T>(object payload)
    {
        string JSON_Body = JsonUtility.ToJson(payload);
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(JSON_Body);
        NetworkStream stream = _client.GetStream();
        stream.Write(bodyRaw, 0, bodyRaw.Length);
        byte[] responseData = new byte[2048];
        int bytesRead = stream.Read(responseData, 0, responseData.Length);
        string response = System.Text.Encoding.UTF8.GetString(responseData, 0, bytesRead);
        return JsonUtility.FromJson<T>(response);
    }
}

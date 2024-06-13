using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _text;

    public string url = "http://10.0.0.121:3000";
    private TcpClient _client = null;
    private string _uri = "10.0.0.121";
    private int _port = 3000;

    void Start()
    {
        ConnectSocket();
    }


    private void ConnectSocket()
    {
        try
        {
            _client = new TcpClient();
            _client.Connect(_uri, _port);

            LoginPayload payload = new LoginPayload()
            {
                Username = "maksens",
                Password = "0192023a7bbd73250516f069df18b500"
            };

            string JSON_Body = JsonUtility.ToJson(payload);
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(JSON_Body);
            NetworkStream stream = _client.GetStream();
            stream.Write(bodyRaw, 0, bodyRaw.Length);
            byte[] responseData = new byte[1024];
            int bytesRead = stream.Read(responseData, 0, responseData.Length);
            string response = System.Text.Encoding.UTF8.GetString(responseData, 0, bytesRead);
            Message Message = JsonUtility.FromJson<Message>(response);
            _text.text = Message.user.name;

            _client.Close();
        }
        catch (System.Exception e)
        {
            Debug.Log($"Exception: {e.Message}");
        }
        finally
        {
            // Ensure the client is properly disposed
            _client?.Dispose();
        }
    }

    [System.Serializable]
    public class LoginPayload
    {
        public string Username;
        public string Password;
    }

    [System.Serializable]
    public class Message
    {
        public string message;
        public Client user;
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
}

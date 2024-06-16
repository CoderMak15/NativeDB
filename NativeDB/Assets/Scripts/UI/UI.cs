using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI _instance { get; private set; }

    private readonly float[] _ref = new float[2] { 375f, 812f };

    private CanvasScaler _scaler;
    private Window _currentWindow;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _scaler = GetComponent<CanvasScaler>();
        if(Screen.width > Screen.height)
        {
            _scaler.scaleFactor = (Screen.height / _ref[1]);
        }
        else
        {
            _scaler.scaleFactor = (Screen.width / _ref[0]);
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    public void Open(Window window, object param = null)
    {
        _currentWindow?.Close();
        _currentWindow = Instantiate(window, transform);
        _currentWindow.Init(param);
    }

    public void Close()
    {
        if(_currentWindow != null)
        {
            _currentWindow.Close();
            _currentWindow = null;
        }
    }
}

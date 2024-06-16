using UnityEngine;

public class Window : MonoBehaviour
{
    public virtual void Init(object parameter) { }   
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void Close()
    {
        Destroy(gameObject);
    }
}
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Content : MonoBehaviour
{
    [SerializeField] private Image _img;

    private Movie _movie;

    public void Init(Movie m)
    {
        _movie = m;
        _img.sprite = Resources.Load<Sprite>(_movie.url);
    }
}

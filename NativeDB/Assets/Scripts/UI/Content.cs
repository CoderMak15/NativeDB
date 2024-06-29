using com.rac.network;
using UnityEngine;
using UnityEngine.UI;

namespace com.rac.core
{
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
}
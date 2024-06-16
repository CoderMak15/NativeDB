using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Movies : Window
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _description;

    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private Content _contentPrefab;

    [SerializeField] private UIContentCarousel _carousel;

    private Movie[] _movies;

    private void Start()
    {
        Movie[] movies = Socket._instance.GetMovies();
        if(movies != null && movies.Length > 0)
        {
            _movies = new Movie[movies.Length];
            for (int i = 0; i < movies.Length; ++i)
            {
                Movie movie = movies[i];
                _movies[i] = movie;
                Instantiate(_contentPrefab, _contentParent).Init(movie);
            }
        }

        _carousel._onMovieSnap += ChangeMovieInfo;
        _carousel.Init();
    }

    private void OnDestroy()
    {
        _carousel._onMovieSnap -= ChangeMovieInfo;
    }

    private void ChangeMovieInfo(int index)
    {
        Movie current = _movies[index];
        _title.text = current.name;
        _description.text = current.description;
    }

}
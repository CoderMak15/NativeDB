using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private Window _login;

    void Start()
    {
        UI._instance.Open(_login);
    }
}
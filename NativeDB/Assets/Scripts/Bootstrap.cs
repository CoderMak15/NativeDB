using com.rac.ui;
using UnityEngine;

namespace com.rac
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private Window _login;

        void Start()
        {
            UI._instance.Open(_login);
        }
    }
}
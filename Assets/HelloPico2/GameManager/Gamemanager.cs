using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Singleton
{
    public class Gamemanager : MonoBehaviour
    {
        public GameObject _Player;
        public Camera _MainCamera;
        public static Gamemanager _Instance;
        public static Gamemanager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = GameObject.FindObjectOfType<Gamemanager>();

                return _Instance;
            }
        }
        private void Start()
        {
            _MainCamera = Camera.main;
            _Player = GameObject.FindGameObjectWithTag("Player");
        }
    }
}

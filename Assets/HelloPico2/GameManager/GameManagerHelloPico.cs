using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Singleton
{
    public class GameManagerHelloPico : MonoBehaviour
    {
        public GameObject _Player;
        public HelloPico2.PlayerController.SpiritBehavior _Spirit;
        public Camera _MainCamera;
        public static GameManagerHelloPico _Instance;
        public static GameManagerHelloPico Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = GameObject.FindObjectOfType<GameManagerHelloPico>();

                return _Instance;
            }
        }
        private void Start()
        {
            _MainCamera = Camera.main;
            _Player = GameObject.FindGameObjectWithTag("Player");
            _Spirit = GameObject.FindObjectOfType<HelloPico2.PlayerController.SpiritBehavior>();
        }
    }
}

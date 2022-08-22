using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Singleton
{
    public class GameManager : MonoBehaviour
    {
        public GameObject _Player;
        public HelloPico2.PlayerController.SpiritBehavior _Spirit;
        public Camera _MainCamera;
        public static GameManager _Instance;
        public static GameManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = GameObject.FindObjectOfType<GameManager>();

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

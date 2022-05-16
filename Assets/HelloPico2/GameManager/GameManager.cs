using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.GameManager
{
    public class GameManager : MonoBehaviour
    {
        public HelloPico2.Audio.SFXLibrary _SFXLibrary;

        private static GameManager _Instance;
        public static GameManager Instance { 
            get {
                if (_Instance == null)
                    _Instance = GameObject.FindObjectOfType<GameManager>();

                return _Instance;
            }
        }

    }
}

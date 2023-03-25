using System;
using UnityEngine;


namespace HelloPico2.LevelTool
{
    public class URLLink : MonoBehaviour
    {
        public string Url;
        private void OnEnable()
        {
            Application.OpenURL(Url);
        }
    }

}
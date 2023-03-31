using System;
using UnityEngine;


namespace HelloPico2.LevelTool
{
    public class URLLink : MonoBehaviour
    {
        public string CNUrl;
        public string OtherUrl;
        
        private void OnEnable()
        {
            if(Application.systemLanguage== SystemLanguage.ChineseSimplified)
            {
                Application.OpenURL(CNUrl);
            }
            else
            {
                Debug.Log("EN");
                Application.OpenURL(OtherUrl);
            }
            //Application.OpenURL(Url);
        }
    }

}
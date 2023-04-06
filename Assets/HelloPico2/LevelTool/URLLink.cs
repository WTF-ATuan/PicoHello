using System;
using UnityEngine;


namespace HelloPico2.LevelTool
{
    public class URLLink : MonoBehaviour
    {
        //CN URL
        public string CNUrl;
        //Other URL
        public string OtherUrl;
        
        private void OnEnable()
        {
            //Check area
            if(Application.systemLanguage== SystemLanguage.ChineseSimplified)
            {
                Application.OpenURL(CNUrl);
            }
            else
            {
                Debug.Log("EN");
                Application.OpenURL(OtherUrl);
            }
        }
    }

}
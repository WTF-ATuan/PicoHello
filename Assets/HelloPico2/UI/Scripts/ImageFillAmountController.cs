using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace HelloPico2.UI
{
    public class ImageFillAmountController : MonoBehaviour, IImageController
    {
        public Image _Image;
        public Color _DefaultColor = Color.cyan;
        public Color _MaxColor = Color.red;

        public void UpdateAmount(float amount)
        {
            amount = Mathf.Clamp01(amount);
            _Image.fillAmount = amount;
            
            if (amount < 1) 
                _Image.color = _DefaultColor;
            else
                _Image.color = _MaxColor;
            
        }
    }
    public interface IImageController {
        public void UpdateAmount(float amount);        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HelloPico2.UI;
using Sirenix.OdinInspector;

namespace HelloPico2.PlayerController.Arm
{
    public class ArmDataVisualizer : SerializedMonoBehaviour
    {
        public ArmData _ArmData;
        public IImageController m_Energy;

        private void Update()
        {
            if (m_Energy != null)
            {
                var amount = _ArmData.Energy / _ArmData.MaxEnergy;
                m_Energy.UpdateAmount(amount);
            }
        }
    }
}

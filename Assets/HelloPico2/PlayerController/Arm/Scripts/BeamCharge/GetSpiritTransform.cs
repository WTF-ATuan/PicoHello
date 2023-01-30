using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public class GetSpiritTransform : MonoBehaviour
    {
        [SerializeField] private LineRendererDrawer _LineRendererDrawer;

        private void Awake()
        {
            _LineRendererDrawer._From = HelloPico2.Singleton.GameManagerHelloPico.Instance.Spirit.GetComponentInChildren<TrailFader>().transform;
        }
    }
}

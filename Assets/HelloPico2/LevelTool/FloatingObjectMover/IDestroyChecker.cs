using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.LevelTool
{
    public interface IDestroyChecker
    {
        public Action<GameObject> OnDestroy { get; set; }        
    }
}

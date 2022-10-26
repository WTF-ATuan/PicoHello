using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.PlayerController
{
    [CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/ArmControllerInputMeshNameData")]
    public class ArmControllerInputMeshNameData : ScriptableObject
    {
        public string SeperateArmRigName;
        public string CombinedArmRigName;
        public enum Controller { Seperate, Combined}
        public enum InputMesh { Grip, Trigger, Primary, Secondary, Axis}
        [System.Serializable]
        public struct MeshName
        {
            public Controller Controller;
            public InputMesh InputMesh;
            public string SeperateName;
            public string CombinedName;
        }
        public List<MeshName> _MeshNames = new List<MeshName>();

        public string GetMeshName(Controller controller, InputMesh inputMesh) {
            for (int i = 0; i < _MeshNames.Count; i++)
            {
                if (_MeshNames[i].InputMesh == inputMesh)
                { 
                    if(controller == Controller.Seperate)
                        return SeperateArmRigName + "/" + _MeshNames[i].SeperateName; 
                    if(controller == Controller.Combined)
                        return CombinedArmRigName + "/" + _MeshNames[i].CombinedName; 
                }
            }

            return null;
        }
    }
}
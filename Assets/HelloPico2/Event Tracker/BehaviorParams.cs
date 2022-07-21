using Sirenix.OdinInspector;

namespace HelloPico2.Event_Tracker{
	[System.Serializable]
	public class BehaviorParams{
		public string paramsName;
		[HorizontalGroup] public float startPoint;

		[ReadOnly] [LabelText(" / ")] [HorizontalGroup]
		public float currentPoint;

		public BehaviorParams(){
			currentPoint = startPoint;
		}

		public void SetPoint(float settingValue){
			currentPoint = settingValue;
		}

		public void ModifyPoint(float amount){
			currentPoint += amount;
		}
	}
}
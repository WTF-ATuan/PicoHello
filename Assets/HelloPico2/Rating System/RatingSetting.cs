using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Rating_System{
	[CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/ RatingSetting", fileName = "RatingSetting", order = 0)]
	public class RatingSetting : ScriptableObject{
		[SerializeField] private List<Vector2> angleList;
		[SerializeField] private List<Vector2> offsetList;

		public List<Vector2> GetAngleRange(){
			return angleList;
		}

		public List<Vector2> GetOffsetRange(){
			return offsetList;
		}
	}
}
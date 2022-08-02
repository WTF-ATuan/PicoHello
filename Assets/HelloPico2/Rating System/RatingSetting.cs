using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.Rating_System{
	[CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/ RatingSetting", fileName = "RatingSetting", order = 0)]
	public class RatingSetting : ScriptableObject{
		[SerializeField] private List<RatingData> offsetList;

		public List<RatingData> GetOffsetRange(){
			return offsetList;
		}
	}

	[Serializable]
	public class RatingData{
		[HorizontalGroup] [HideLabel] public float minValue, maxValue;
		[HorizontalGroup] [HideLabel] public float ratingPoint;
	}
}
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace HelloPico2.Rating_System{
	[CreateAssetMenu(menuName = "HelloPico2/ScriptableObject/ RatingSetting", fileName = "RatingSetting", order = 0)]
	public class RatingSetting : ScriptableObject{
		[SerializeField] [LabelText("Offset Rating")]
		private List<RatingData> offsetList;

		[LabelText("Min Ignore")] public int minimumIgnoreCount = 10;

		[BoxGroup("Hit Rate")] [Range(0, 1)] public float weekLevel;

		[FormerlySerializedAs("midLevel")] [BoxGroup("Hit Rate")] [Range(0, 1)] public float middenLevel;

		[FormerlySerializedAs("greatLevel")] [BoxGroup("Hit Rate")] [Range(0, 1)] public float masterLevel;


		public List<RatingData> GetOffsetRange(){
			return offsetList;
		}
	}

	[Serializable]
	public class RatingData{
		[HorizontalGroup] [HideLabel] public float minValue, maxValue;

		[HorizontalGroup] [HideLabel] [Range(0, 1)] [MaxValue(1)]
		public float ratingPoint;
	}
}
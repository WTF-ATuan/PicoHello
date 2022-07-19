using System.Collections.Generic;
using UnityEngine;

namespace HelloPico2.Event_Tracker{
	[CreateAssetMenu(fileName = "TrackedEvent-Overview", menuName = "HelloPico2/ScriptableObject/ TrackedEvent Overview", order = 0)]
	public class TrackedEventOverview : ScriptableObject{
		[SerializeField] private List<TrackedEvent> trackedEventList;
	}
}
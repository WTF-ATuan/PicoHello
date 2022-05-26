using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2{
	[Serializable]
	public class ViewEventData{
		[Required] public string identity;
		[TextArea] public string description;
	}
}
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HelloPico2.Event_Tracker{
	public class BehaviorDataWriter : MonoBehaviour{
		[Required] public PlayerBehaviorOverview dataRepository;

		//接收所有需要記錄的資廖，在依照Repository 需要的資料形式進行整理。
	}
}
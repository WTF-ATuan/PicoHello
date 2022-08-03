using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using JetBrains.Annotations;
using Sirenix.Utilities.Editor;

#endif

namespace HelloPico2{
	[CreateAssetMenu(fileName = "ViewEventDataOverview",
		menuName = "HelloPico2/ScriptableObject/ ViewEventData Overview",
		order = 0)]
	public class ViewEventDataOverview : ScriptableObject{
		[TypeFilter("GetEventType")] [HideReferenceObjectPicker] [InlineButton("CreateEvent")]
		public ViewEventData eventData;

		private IEnumerable GetEventType(){
			var eventType = typeof(ViewEventData).Assembly
					.GetTypes()
					.Where(x => !x.IsAbstract)
					.Where(x => !x.IsGenericTypeDefinition)
					.Where(x => typeof(ViewEventData).IsAssignableFrom(x));
			return eventType;
		}

		private void CreateEvent(){
			var type = eventData.GetType();
			if(type == typeof(ViewEventData)) Debug.Log($"Choose else Event");
			var instance = Activator.CreateInstance(type);
			var data = (ViewEventData)instance;
			viewEventDataList.Add(data);
		}

		[SerializeReference]
		[HideReferenceObjectPicker]
		[PropertyOrder(100)]
		[Searchable]
		[LabelText("Data")]
		[ListDrawerSettings(OnBeginListElementGUI = "BeginListElementGUI", OnEndListElementGUI = "EndListElementGUI",
			Expanded = true)]
		private List<ViewEventData> viewEventDataList = new List<ViewEventData>();

		public T FindEventData<T>(string id) where T : ViewEventData{
			var viewEventData = viewEventDataList.Find(x => x.Equals(id));
			if(viewEventData == null){
				throw new NullReferenceException($"Can,t Not Find {id}");
			}

			return viewEventData as T;
		}

		public List<T> FindAllEvent<T>() where T : ViewEventData{
			var type = typeof(T);
			var foundDataList = viewEventDataList.FindAll(x => x.GetType() == type);
			return foundDataList.Cast<T>().ToList();
		}

		#if UNITY_EDITOR
		[UsedImplicitly]
		private void BeginListElementGUI(int index){
			GUILayout.BeginHorizontal();
			var elementBoxText = GetElementBoxText(index);
			var guiContent = new GUIContent(elementBoxText);
			guiContent.tooltip = "AAA";
			var contentColor = Color.white;
			GUI.backgroundColor = GetGUIColor(index);
			GUI.contentColor = contentColor;
			SirenixEditorGUI.BeginBox(guiContent);
			GUI.contentColor = Color.white;
		}

		private Color GetGUIColor(int index){
			return index % 2 == 0 ? Color.green : Color.red;
		}


		[UsedImplicitly]
		private void EndListElementGUI(int index){
			SirenixEditorGUI.EndBox();
			GUILayout.EndHorizontal();
		}

		private string GetElementBoxText(int index){
			var data = viewEventDataList[index];
			var identity = data.identity;
			var type = data.GetType().Name;
			var text = $"( {index} -> {type}) : {identity}";
			return text;
		}
		#endif
	}
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
		[TypeFilter("GetEventType")] [InlineButton("CreateEvent")] [HideLabel]
		public ViewEventData eventData;

		public bool useColorWithType = true;

		[Title("Select Tool")] [AssetSelector(Paths = "Assets/HelloPico2/Data/Audio")] [InlineButton("AddRangeAudio")]
		public List<AudioClip> audioSelector;

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

		private void AddRangeAudio(){
			foreach(var audioData in audioSelector.Select(
						audioClip => new AudioData{
							identity = audioClip.name,
							clip = audioClip
						})){
				viewEventDataList.Add(audioData);
			}

			audioSelector.Clear();
		}

		[ButtonGroup("Sort")]
		[Button("With Type")]
		[PropertyOrder(200)]
		private void SortWithType(){
			var copiedData = viewEventDataList.Select(x => x).ToList();
			var sortedEvent = new List<ViewEventData>();
			while(copiedData.Count > 0){
				var type = copiedData.First().GetType();
				var typeData = copiedData.FindAll(x => x.GetType() == type);
				copiedData.RemoveAll(x => x.GetType() == type);
				sortedEvent.AddRange(typeData);
			}

			viewEventDataList = sortedEvent;
		}

		[ButtonGroup("Sort")]
		[Button("With Name")]
		[PropertyOrder(200)]
		private void SortWithName(){ }

		[Title("Data List")]
		[SerializeReference]
		[HideReferenceObjectPicker]
		[PropertyOrder(100)]
		[Searchable]
		[LabelText("Data")]
		[ListDrawerSettings(OnBeginListElementGUI = "BeginListElementGUI"
			, OnEndListElementGUI = "EndListElementGUI"
			, ElementColor = "GetGUIColor"
			, DraggableItems = false
			, NumberOfItemsPerPage = 5)]
		private List<ViewEventData> viewEventDataList = new List<ViewEventData>();

		public T FindEventData<T>(string id) where T : ViewEventData{
			var viewEventData = viewEventDataList.Find(x => x.Equals(id));
			if(viewEventData == null){
				throw new NullReferenceException($"[{name}] couldn't find [{id}] asset");
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
			var contentColor = Color.white;
			GUI.contentColor = contentColor;
			SirenixEditorGUI.BeginBox(guiContent);
			GUI.contentColor = Color.white;
		}

		private Color GetGUIColor(int index){
			if(!useColorWithType){
				return index % 2 == 0
						? new Color(0, 0.8f, 0, 0.1f)
						: new Color(0.8f, 0, 0, 0.1f);
			}

			var viewEventData = viewEventDataList[index];
			var color = viewEventData.GetEditorColor();
			return color;
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
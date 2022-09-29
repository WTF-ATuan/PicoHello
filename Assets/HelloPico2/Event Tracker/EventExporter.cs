using System;
using System.IO;
using UnityEngine;

namespace Actor.Scripts.EventMessage{
	public class EventExporter{
		public string FileName{ get; }
		public string FileFullPath{ get; }
		private readonly FileStream _file;
		private readonly StreamWriter _streamWriter;

		public EventExporter(string fileName){
			FileFullPath = Application.persistentDataPath + "/" + $"{fileName}" + ".json";
			if(File.Exists(FileFullPath)) //TODO : handle this situation
				return;

			_file = new FileStream(FileFullPath, FileMode.CreateNew);
			_streamWriter = new StreamWriter(_file);
			FileName = fileName;
		}


		public void WriteEvent(object eventData){
			var jsonString = GetJsonString(eventData);
			_streamWriter.Write(jsonString + Environment.NewLine);
			_streamWriter.Flush();
		}

		public string GetJsonString(object eventData){
			var type = eventData.GetType();
			if(!type.IsSerializable){
				throw new Exception($"{type} is not Serializable Type");
			}
			var jsonString = JsonUtility.ToJson(eventData);
			return jsonString;
		}

		public void Timeout(){
			_streamWriter.Flush();
			_streamWriter.Close();
			_streamWriter.Dispose();
		}

		public static void PostTrackerEvent(string eventName, string jsonString){
			var cls = new AndroidJavaClass("os.teatracker.TeaAgent");
			var jsonParam = new AndroidJavaObject("org.json.JSONObject", jsonString);
			cls.CallStatic("onEvent", eventName, jsonParam);
		}

		public static void InitTracker(){
			var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			var cls = new AndroidJavaClass("os.teatracker.TeaAgent");
			cls.CallStatic("init", currentActivity, null);
			cls.CallStatic("addTeaListener", currentActivity);
		}
	}
}
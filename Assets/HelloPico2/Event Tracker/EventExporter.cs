using System.IO;
using UnityEngine;

namespace Actor.Scripts.EventMessage{
	public class EventExporter{
		public string FileName{ get; }

		private readonly FileStream _file;

		private string _fileFullName;

		private StreamWriter _streamWriter;

		public EventExporter(string fileName){
			_fileFullName = Application.persistentDataPath + "/" + $"{fileName}" + ".json";
			_file = new FileStream(_fileFullName, FileMode.CreateNew);
			_streamWriter = new StreamWriter(_file);
			FileName = fileName;
		}


		public void WriteEvent(){
			// var jsonString = JsonUtility.ToJson();
			// _streamWriter.Write(jsonString + Environment.NewLine);
			_streamWriter.Flush();
		}

		public void Timeout(){
			_streamWriter.Flush();
			_streamWriter.Close();
			_streamWriter.Dispose();
		}
		public static void PostTrackerEvent(string eventName,string jsonString) 
		{
			var cls = new AndroidJavaClass("os.teatracker.TeaAgent");
			var jsonParam = new AndroidJavaObject("org.json.JSONObject",jsonString);
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
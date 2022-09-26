using System.IO;

namespace Actor.Scripts.EventMessage{
	public class EventExporter{
		public string FilePath{ get; }
		public string FileName{ get; }

		private readonly FileStream _file;

		private string _fileFullName;

		private StreamWriter _streamWriter;

		public EventExporter(string filePath, string fileName){
			_fileFullName = filePath + "/" + $"{fileName}" + ".json";
			_file = new FileStream(_fileFullName, FileMode.CreateNew);
			_streamWriter = new StreamWriter(_file);
			FilePath = filePath;
			FileName = fileName;
		}


		public void WriteMessage(){
			// var jsonString = JsonUtility.ToJson();
			// _streamWriter.Write(jsonString + Environment.NewLine);
			_streamWriter.Flush();
		}

		public void Timeout(){
			_streamWriter.Flush();
			_streamWriter.Close();
			_streamWriter.Dispose();
		}
	}
}
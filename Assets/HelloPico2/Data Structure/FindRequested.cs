using System;

namespace HelloPico2.Data_Structure{
	public class FindRequested{
		public Type RequestDataType{ get; }

		public FindRequested(Type requestDataType){
			RequestDataType = requestDataType;
		}
	}
}
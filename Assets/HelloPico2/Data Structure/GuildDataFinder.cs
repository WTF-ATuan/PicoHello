using UnityEngine;

namespace HelloPico2.Data_Structure{
	public class GuildDataFinder{
		
		public static GuildDataFinder NewInstance(){
			return new GuildDataFinder();
		}

		public Animation Find(){
			// 發出搜索事件 
			// 誰要接收搜索事件 (接收者必須擁有找尋資料的權限) 擁有權限的角色需要是Mono? 
			// 依循相應Input 找到對應資料並 通過事件回傳
			return null;
		}
	}
}
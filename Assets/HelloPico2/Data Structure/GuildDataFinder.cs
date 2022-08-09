using System.Collections.Generic;
using System.Linq;
using Project;
using UnityEngine;

namespace HelloPico2.Data_Structure{
	public class GuildDataFinder{
		private string _identity;

		public static GuildDataFinder NewInstance(){
			return new GuildDataFinder();
		}

		public GuildDataFinder SetIdentity(string id){
			_identity = id;
			return this;
		}
		// 發出搜索事件 => 拿到所有相對應的Animation 資料 (可能不會是List) => 依照自身的Condition 去搜索資料
		// 誰要接收搜索事件 (接收者必須擁有找尋資料的權限) 擁有權限的角色需要是Mono? 
		// 依循相應Input 找到對應資料並 通過事件回傳

		public ModuleModel Find(){
			var findRequested = new FindRequested(typeof(ModuleModel));
			var objects = EventBus.Post<FindRequested, List<object>>(findRequested);
			var models = objects.Cast<ModuleModel>().ToList();
			var foundModel = models.Find(x => x.identity == _identity);
			return foundModel;
		}
	}
}
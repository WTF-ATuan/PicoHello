using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Sirenix.Utilities;
using UnityEngine;

namespace Project{
	public class EventBus : MonoBehaviour{
		private static readonly Dictionary<Type, List<Action<object>>> NonCallbackActions =
				new Dictionary<Type, List<Action<object>>>();

		private static readonly Dictionary<Type, List<Func<object, object>>> CallbackActions =
				new Dictionary<Type, List<Func<object, object>>>();

		public static void Subscribe<T>(Action<T> callback){
			var type = typeof(T);
			var containsKey = NonCallbackActions.ContainsKey(type);
			if(containsKey){
				var actions = NonCallbackActions[type];
				actions.Add(o => callback((T)o));
			}
			else{
				var actions = new List<Action<object>>{ o => callback((T)o) };
				NonCallbackActions.Add(type, actions);
			}
		}

		public static void Subscribe<T, TResult>(Func<T, TResult> callback){
			var type = typeof(T);
			var containsKey = CallbackActions.ContainsKey(type);
			if(containsKey){
				var callbackAction = CallbackActions[type];
				callbackAction.Add(o => callback((T)o));
			}
			else{
				var func = new List<Func<object, object>>{ o => callback((T)o) };
				CallbackActions.Add(type, func);
			}
		}

		public static void Post<T>(T obj){
			var type = typeof(T);
			var containsKey = NonCallbackActions.ContainsKey(type);
			if(containsKey){
				var actions = NonCallbackActions[type];
				foreach(var o in actions){
					try{
						o.Invoke(obj);
					}
					catch(Exception exception){
						actions.Remove(o);
						NonCallbackActions[type] = actions;
						Debug.Log(
							$"Removed Event from {type.Name} : {exception}");
						return;
					}
				}
			}
			else{
				var fullName = type.Name;
				Debug.Log($" Event {fullName}  is no subscriber");
			}
		}

		public static TResult Post<T, TResult>(T obj){
			var type = typeof(T);
			var containsKey = CallbackActions.ContainsKey(type);
			if(containsKey){
				var callbackAction = CallbackActions[type];
				foreach(var returnValue in callbackAction.Select(func => func.Invoke(obj))){
					return (TResult)returnValue;
				}
			}
			else{
				var fullName = type.Name;
				Debug.Log($" Type {fullName}  is not Contain in EventBus");
			}

			return default;
		}

		public static void DebugList(){
			Debug.Log(NonCallbackActions.Count + "Non CallbackActions");
			NonCallbackActions.ForEach(x => Debug.Log($"Event Name = {x.Key.Name}"));
			CallbackActions.ForEach(x => Debug.Log(x.Key.Name));
		}

		public static void Clear(){
			NonCallbackActions.Clear();
			CallbackActions.Clear();
		}


		private void OnDisable(){
			NonCallbackActions.Clear();
			CallbackActions.Clear();
		}
	}
}
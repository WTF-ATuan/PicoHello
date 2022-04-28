using UnityEngine;

namespace HelloPico2.InputDevice.Scripts{
	public class Sword : MonoBehaviour{

		private void OnSelect(){
			//生成一把劍
		}

		private void OnXAButtonDown(){
			
		}

		private void OnTouchPad(Vector2 axis){
			var axisX = axis.x;
			var axisY = axis.y;
			if(axisX > 0.1f){
				//變長
			}
			else{
				//變短
			}

			if(axisY > 0.1f){
				//變硬
			}
			else{
				//變軟
			}
		}
		}
}
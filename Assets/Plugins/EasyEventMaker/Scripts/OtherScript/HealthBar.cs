using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {
	public Image hpBar;
	public GameObject player;
	
	void Start(){
		if(!player){
			player = GameObject.FindWithTag("Player");
		}
	}
	
	void Update(){
		if(!player){
			Destroy(gameObject);
			return;
		}
		Status stat = player.GetComponent<Status>();
		int maxHp = stat.maxHealth;
		float hp = stat.health;

		float curHp = hp/maxHp;

		//HP Gauge
		if(curHp > hpBar.fillAmount){
			hpBar.fillAmount += 1 / 1 * Time.unscaledDeltaTime;
			if(hpBar.fillAmount > curHp){
				hpBar.fillAmount = curHp;
			}
		}	
		if(curHp < hpBar.fillAmount){
			hpBar.fillAmount -= 1 / 1 * Time.unscaledDeltaTime;
			if(hpBar.fillAmount < curHp){
				hpBar.fillAmount = curHp;
			}
		}
	}
}

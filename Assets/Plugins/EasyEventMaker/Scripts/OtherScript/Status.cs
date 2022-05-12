using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour {
	public Animator animator;
	//public GameObject mainModel;

	public int attack = 5;
	public int defense = 3;
	public int maxHealth = 100;
	public int health = 100;
	public int freezeGlobalBoolId = 1;
	public GameObject deadPrefab;

	[HideInInspector]
	public bool dead = false;
	[HideInInspector]
	public bool freeze = false;
	[HideInInspector]
	public bool flinch = false;

	// Use this for initialization
	void Start(){
		if(!animator && GetComponent<Animator>()){
			animator = GetComponent<Animator>();
		}
		/*if(!mainModel && animator){
			mainModel = animator.gameObject;
		}*/
	}

	public void Healing(int amount){
		health += amount;
		if(health > maxHealth){
			health = maxHealth;
		}
	}

	public void OnDamage(int dmg){
		if(dmg < 1){
			dmg = 1;
		}
		health -= dmg;
		if(health <= 0){
			health = 0;
			Death();
		}
	}

	public void Flinch(){
		if(flinch){
			return;
		}
		if(animator){
			animator.SetTrigger("Hurt");
		}
		StartCoroutine(KnockBack());
	}

	IEnumerator KnockBack(){
		flinch = true;
		yield return new WaitForSeconds(0.2f);
		flinch = false;
	}

	public void Death(){
		if(dead){
			return;
		}
		dead = true;
		if(!deadPrefab){
			print("This Character didn't have dead prefab");
		}else{
			Instantiate(deadPrefab , transform.position , transform.rotation);
		}
		Destroy(gameObject);
	}

	public void DamagePercent(int amount){
		int dmg = maxHealth * amount;
		dmg /= 100;

		maxHealth -= amount;
		if(maxHealth < 0){
			maxHealth = 0;
			Death();
		}
	}
}

[System.Serializable]
public class AttackSetting{
	public GameObject attackPrefab;
	public Transform attackPoint;
	public float preAttackDelay = 0.3f;
	public float attackDelay = 0.5f;
	public float lastComboDelay = 0.3f;
	public int maxCombo = 5;
}

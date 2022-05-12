using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Status))]
[RequireComponent(typeof(CharMotor))]

public class AiEnemy : MonoBehaviour {
	public enum AIState{
		Moving = 0,
		Pausing = 1,
		Idle = 2,
	}
	public AttackSetting attack;

	public float approachDistance = 2.0f;
	public float detectRange = 15.0f;
	public float lostSight = 40.0f;
	public float speed = 4.0f;
	public Transform followTarget;

	[HideInInspector]
	public bool cancelAttack = false;
	private bool attacking = false;

	//[HideInInspector]
	public AIState followState;
	private float distance = 0.0f;

	// Use this for initialization
	void Start(){
		gameObject.tag = "Enemy";
		if(!attack.attackPoint){
			attack.attackPoint = new GameObject().transform;
			attack.attackPoint.parent = this.transform;
		}
	}

	Vector3 GetDestination(){
		Vector3 destination = followTarget.position;
		destination.y = transform.position.y;
		return destination;
	}

	// Update is called once per frame
	void Update(){
		Status stat = GetComponent<Status>();
		CharacterController controller = GetComponent<CharacterController>();
		if(stat.flinch){
			cancelAttack = true;
			controller.Move(transform.TransformDirection(Vector3.back) * 4.3f * Time.deltaTime);
			followState = AIState.Moving;
			return;
		}
		if(stat.dead || stat.freeze || Time.timeScale == 0 || EventActivator.globalFreeze || attacking){
			if(stat.animator){
				stat.animator.SetBool("Moving" , false);
			}
			return;
		}

		FindClosestEnemy();
		if(!followTarget){
			return;
		}

		distance = (transform.position - GetDestination()).magnitude;
		
		if(followState == AIState.Moving){
			if(stat.animator){
				stat.animator.SetBool("Moving" , true);
			}
			
			if(distance <= approachDistance) {
				followState = AIState.Pausing;
				//----Attack----
				StartCoroutine(Attack());
			}else if(distance >= lostSight){
				//Lost Sight
				followState = AIState.Idle;
				if(stat.animator){
					stat.animator.SetBool("Moving" , false);
				}
			}else{
				Vector3 forward = transform.TransformDirection(Vector3.forward);
				controller.Move(forward * speed * Time.deltaTime);
				
				Vector3 look = followTarget.position;
				look.y = transform.position.y;
				transform.LookAt(look);
			}
		}else if(followState == AIState.Pausing){
			if(stat.animator){
				stat.animator.SetBool("Moving" , false);
			}
			
			Vector3 look = followTarget.position;
			look.y = transform.position.y;
			transform.LookAt(look);
			
			if(distance > approachDistance){
				followState = AIState.Moving;
			}
		}else if(followState == AIState.Idle){
			//----------------Idle Mode--------------
			Vector3 look = followTarget.position;
			look.y = transform.position.y - look.y;
			int getHealth = stat.maxHealth - stat.health;
			
			if(distance < detectRange && Mathf.Abs(look.y) <= 4 || getHealth > 0){
				followState = AIState.Moving;
			}
		}
	}
	private int c = 0;
	IEnumerator Attack(){
		cancelAttack = false;
		Status stat = GetComponent<Status>();
		if(!stat.flinch && !stat.freeze && !attacking){
			attacking = true;
			if(stat.animator){
				stat.animator.SetBool("Moving" , false);
				stat.animator.SetInteger("Combo" , c);
				stat.animator.SetTrigger("Attack");
			}
			yield return new WaitForSeconds(attack.preAttackDelay);
			
			if(!cancelAttack){
				GameObject bl = Instantiate(attack.attackPrefab , attack.attackPoint.position , attack.attackPoint.rotation) as GameObject;
				bl.GetComponent<BulletStatus>().Setting(stat.attack , "Enemy");
				yield return new WaitForSeconds(attack.attackDelay);
				attacking = false;
				CheckDistance();
			}else{
				attacking = false;
				followState = AIState.Moving;
			}
		}
	}

	void CheckDistance(){
		Status stat = GetComponent<Status>();
		if(!followTarget || EventActivator.globalFreeze){
			followState = AIState.Idle;
			if(stat.animator){
				stat.animator.SetBool("Moving" , false);
			}
			return;
		}
		float dist = (followTarget.position - transform.position).magnitude;
		if(dist <= approachDistance){
			Vector3 look = followTarget.position;
			look.y = transform.position.y;
			transform.LookAt(look);
			StartCoroutine(Attack());
		}else{
			followState = AIState.Moving;
		}
	}

	void FindClosestEnemy(){ 
		// Find all game objects with tag Enemy
		float dist = Mathf.Infinity;
		float findingradius = detectRange;
		
		if(GetComponent<Status>().health < GetComponent<Status>().maxHealth){
			findingradius += lostSight + 5.0f;
		}
		
		Collider[] objectsAroundMe = Physics.OverlapSphere(transform.position , findingradius);
		foreach(Collider obj in objectsAroundMe){
			if(obj.CompareTag("Player")){
				Vector3 diff = (obj.transform.position - transform.position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < dist) { 
					//------------
					followTarget = obj.transform;
					dist = curDistance;
				} 
			}
		}
	}
}

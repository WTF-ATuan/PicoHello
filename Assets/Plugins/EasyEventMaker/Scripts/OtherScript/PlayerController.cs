using UnityEngine;
using System.Collections;
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Status))]
[RequireComponent(typeof(CharMotor))]

public class PlayerController : MonoBehaviour{
	[System.Serializable]
	public class LockOn{
		public bool enable = true;
		public float radius = 5.0f;  //this is radius to checks for other objects
		public float lockOnRange = 4.0f; //this is how far it checks for other objects
		[HideInInspector]
		public Transform lockTarget;
		[HideInInspector]
		public GameObject target;
	}

	public enum WhileAtk{
		MeleeFwd,
		Immobile
	}

	private CharMotor motor;
	private float moveHorizontal = 0;
	private float moveVertical = 0;
	public LockOn autoLockTarget;

	public AttackSetting attack;
	public WhileAtk whileAttack = WhileAtk.MeleeFwd;

	private float nextFire = 0.0f;
	private bool meleefwd = false;
	public bool cannotAttack = false;
	public bool combatMode = false;

	void Awake(){
		motor = GetComponent<CharMotor>();
		gameObject.tag = "Player";
		if(!attack.attackPoint){
			attack.attackPoint = new GameObject().transform;
			attack.attackPoint.parent = this.transform;
		}
		if(combatMode && GetComponent<Status>().animator){
			GetComponent<Status>().animator.SetBool("CombatMode" , true);
		}
	}
	
	void Update(){
		Status stat = GetComponent<Status>();
		CharacterController controller = GetComponent<CharacterController>();
		if(stat.animator){
			stat.animator.SetBool("Midair" , !motor.grounded);
		}
		if(stat.dead || stat.freeze || Time.timeScale == 0 || EventActivator.globalFreeze){
			motor.inputMoveDirection = Vector3.zero;
			if(stat.animator){
				stat.animator.SetBool("Moving" , false);
			}
			return;
		}
		if(stat.flinch){
			controller.Move(transform.TransformDirection(Vector3.back) * 4.3f * Time.deltaTime);
			return;
		}
		
		if(meleefwd){
			controller.Move(transform.TransformDirection(Vector3.forward) * 5 * Time.deltaTime);
		}
		if(Input.GetAxisRaw("Horizontal") != 0 && !onAttack || Input.GetAxisRaw("Vertical") != 0 && !onAttack){
			moveHorizontal = Input.GetAxis("Horizontal");
			moveVertical = Input.GetAxis("Vertical");
			//GetComponent<CharacterAnimation>().onMoving = true;
			if(stat.animator){
				stat.animator.SetBool("Moving" , true);
			}
		}else{
			moveHorizontal = 0;
			moveVertical = 0;
			//GetComponent<CharacterAnimation>().onMoving = false;
			if(stat.animator){
				stat.animator.SetBool("Moving" , false);
			}
		}
		//Attack
		if(Input.GetButton("Fire1") && !onAttack && Time.time > nextFire && !cannotAttack){
			if(autoLockTarget.enable){
				LockOnEnemy();
			}
			if(Time.time > (nextFire + 0.5f)){
				c = 0;
			}
			StartCoroutine(Attacking());
		}
		
		
		Vector3 forward = Camera.main.transform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		Vector3 right = new Vector3(forward.z, 0, -forward.x);
		//Vector3 targetDirection = Input.GetAxis("Horizontal") * right + Input.GetAxis("Vertical") * forward;
		
		Vector3 targetDirection = moveHorizontal * right + moveVertical * forward;
		
		//----------------------------------
		if(moveHorizontal != 0 && targetDirection.normalized != Vector3.zero || moveVertical != 0 && targetDirection.normalized != Vector3.zero){
			transform.rotation = Quaternion.LookRotation(targetDirection.normalized);
		}
		//-----------------------------------------------------------------------------
		if(motor.grounded && Input.GetButtonDown("Jump") && stat.animator){
			stat.animator.SetTrigger("Jump");
		}

		motor.inputMoveDirection = targetDirection.normalized;
		motor.inputJump = Input.GetButton("Jump");
		
	}

	private int c = 0;
	[HideInInspector]
	public bool onAttack = false;
	IEnumerator Attacking(){
		if(attack.attackPrefab){
			Status stat = GetComponent<Status>();
			onAttack = true;
			if(stat.animator){
				stat.animator.SetInteger("Combo" , c);
				stat.animator.SetTrigger("Attack");
			}
			if(whileAttack == WhileAtk.MeleeFwd){
				StartCoroutine(MeleeDash());
			}
			yield return new WaitForSeconds(attack.preAttackDelay);
			nextFire = Time.time + attack.attackDelay;
			
			GameObject bl = Instantiate(attack.attackPrefab , attack.attackPoint.position , attack.attackPoint.rotation) as GameObject;
			if(bl.GetComponent<BulletStatus>()){
				bl.GetComponent<BulletStatus>().Setting(stat.attack , "Player");
			}
			
			yield return new WaitForSeconds(attack.attackDelay);
			if(c >= attack.maxCombo - 1){
				yield return new WaitForSeconds(attack.lastComboDelay);
				c = 0;
			}else{
				c++;
			}
			onAttack = false;
		}else{
			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator MeleeDash(){
		meleefwd = true;
		yield return new WaitForSeconds(0.15f);
		meleefwd = false;
	}

	// Lock On the closest enemy 
	public void LockOnEnemy(){ 
		Vector3 checkPos = transform.position + transform.forward * autoLockTarget.lockOnRange;
		GameObject closest; 
		
		float distance = Mathf.Infinity; 
		Vector3 position = transform.position; 
		autoLockTarget.lockTarget = null; // Reset Lock On Target
		Collider[] objectsAroundMe = Physics.OverlapSphere(checkPos , autoLockTarget.radius);
		foreach(Collider obj in objectsAroundMe){
			if(obj.CompareTag("Enemy")){
				Vector3 diff = (obj.transform.position - position); 
				float curDistance = diff.sqrMagnitude; 
				if (curDistance < distance) { 
					//------------
					closest = obj.gameObject; 
					distance = curDistance;
					autoLockTarget.target = closest;
					autoLockTarget.lockTarget = closest.transform;
				} 
			}
		}
		//Face to the target
		if(autoLockTarget.lockTarget){
			Vector3 lookOn = autoLockTarget.lockTarget.position;
			lookOn.y = transform.position.y;
			transform.LookAt(lookOn);
		}
		
	}

}



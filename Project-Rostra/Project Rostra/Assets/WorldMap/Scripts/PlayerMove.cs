using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMove : MonoBehaviour {
	public Animator an;
	private Rigidbody2D rb;
	private ConversationTrigger ct;
	private DialogueManager dm;
	DialogueToBattle dtm;

	[Header("Movement Variables")]
	public float max_walk_speed;
	public float max_run_speed;
	public float accel;
	public float decel;
	public float perc;

	[HideInInspector]
	public float speedMultiplier;

	// input
	private bool Up => Input.GetButton("Up");
	private bool Down => Input.GetButton("Down");
	private bool Left => Input.GetButton("Left");
	private bool Right => Input.GetButton("Right");

	void Start() {
		speedMultiplier = 1f;
		rb = GetComponent<Rigidbody2D>();
		dm = DialogueManager.instance;
		an = GetComponent<Animator>();
	}

	bool NearZero(float value) {
		return Mathf.Abs(value) <= perc;
	}

	void LateUpdate() {
		if (gameObject.activeSelf) {
			if (dm.canWalk && !BattleManager.battleInProgress && !PauseMenuController.isPaused && !ItemShopUI.IsOpen && CutsceneManager.instance.isActive == false) {
				an.SetFloat("Horizontal", (Right ? 1f : 0f) - (Left ? 1f : 0f));
				an.SetFloat("Vertical", (Up ? 1f : 0f) - (Down ? 1f : 0f));
				an.SetFloat("Speed", rb.velocity.sqrMagnitude);
				TalkToNPC();
			} else if (dm.canWalk == false || PauseMenuController.isPaused == true) {
				an.SetFloat("Horizontal", 0);
				an.SetFloat("Vertical", 0);
				an.SetFloat("Speed", 0);
			}
			// these are debug commands, please please please remove once done testing

			//if (Input.GetKeyDown(KeyCode.G))
			//{
			//	DialogueManager.instance.metAllChars = true;
			//	DialogueManager.instance.SetChoice(ChoiceEnum.metAllChars, true);
			//	//DialogueManager.instance.sawJournal = true;
			//	//DialogueManager.instance.SetChoice(ChoiceEnum.sawJournal, true);
			//	//DialogueManager.instance.dwarf = true;
			//	//DialogueManager.instance.SetChoice(ChoiceEnum.dwarf, true);
			//	//DialogueManager.instance.talkedToContact = true;
			//	//DialogueManager.instance.SetChoice(ChoiceEnum.talkToContact, true);
			//	DialogueManager.instance.battleFarea = true;
			//	DialogueManager.instance.SetChoice(ChoiceEnum.battleFarea, true);
			//}
			//if (Input.GetKeyDown(KeyCode.U))
			//{
			//	DialogueManager.instance.kill = true;
			//	DialogueManager.instance.SetChoice(ChoiceEnum.kill, true);
			//}
		}
	}

	void FixedUpdate() {
		//Debug.Log(dm.canWalk + "" + BattleManager.battleInProgress + "" + PauseMenuController.isPaused + "" + ItemShopUI.IsOpen + "" + CutsceneManager.instance.isActive);
		if (dm.canWalk && !BattleManager.battleInProgress && !PauseMenuController.isPaused && !ItemShopUI.IsOpen && CutsceneManager.instance.isActive == false) {

			float speed = (Input.GetButton("Cancel") ? max_run_speed : max_walk_speed) * speedMultiplier;
			Vector2 vel = rb.velocity;

			#region Movement Garbo
			if (Up != Down) {
				if (Up) {
					if (NearZero(vel.y - speed)) vel.y = speed;
					else if (vel.y < speed) vel.y += accel * Time.fixedDeltaTime * speedMultiplier;
					else if (vel.y > speed) vel.y -= decel * Time.fixedDeltaTime * speedMultiplier;
				}
				if (Down) {
					if (NearZero(vel.x + speed)) vel.y = -speed;
					else if (vel.y > -speed) vel.y -= accel * Time.fixedDeltaTime * speedMultiplier;
					else if (vel.y < -speed) vel.y += decel * Time.fixedDeltaTime * speedMultiplier;
				}
			} else {
				if (NearZero(vel.y)) vel.y = 0f;
				else if (vel.y > 0f) vel.y -= decel * Time.fixedDeltaTime * speedMultiplier;
				else if (vel.y < 0f) vel.y += decel * Time.fixedDeltaTime * speedMultiplier;
			}

			if (Right != Left) {
				if (Right) {
					if (NearZero(vel.x - speed)) vel.x = speed;
					else if (vel.x < speed) vel.x += accel * Time.fixedDeltaTime * speedMultiplier;
					else if (vel.x > speed) vel.x -= decel * Time.fixedDeltaTime * speedMultiplier;
				}
				if (Left) {
					if (NearZero(vel.x + speed)) vel.x = -speed;
					else if (vel.x > -speed) vel.x -= accel * Time.fixedDeltaTime * speedMultiplier;
					else if (vel.x < -speed) vel.x += decel * Time.fixedDeltaTime * speedMultiplier;
				}
			} else {
				if (NearZero(vel.x)) vel.x = 0f;
				else if (vel.x > 0f) vel.x -= decel * Time.fixedDeltaTime * speedMultiplier;
				else if (vel.x < 0f) vel.x += decel * Time.fixedDeltaTime * speedMultiplier;
			}
			#endregion

			rb.velocity = vel;

		} else if (dm.canWalk == false  || PauseMenuController.isPaused == true) {
			rb.velocity = Vector3.zero;
		}
	}

	// this is all code for handeling getting the collision with the NPC and getting it's dialogue
	private void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag("NPC")) {
			ct = col.GetComponent<ConversationTrigger>();
			if (ct.pressZ != null) {
				ct.SetPressZ(true);
			}
		}
	}
	// set your CT to null once you exit the range of the NPC
	private void OnTriggerExit2D(Collider2D col) {
		if (col.CompareTag("NPC") && col.GetComponent<ConversationTrigger>() == ct) {
			if (ct.pressZ != null) {
				ct.SetPressZ(false);
			}
			ct = null;

		}
	}
	// this is will handle talking to the NPC 
	public void TalkToNPC() {
		if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetButtonDown("Confirm") && ct != null && ct.isChoiceDepend == false && !ItemShopUI.IsOpen) {
			ct.TriggerConvo();
			//Debug.Log("Talking");
		} else if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetButtonDown("Confirm") && ct != null && ct.dialogue.isChoice == true && ct.dialogue.hasPlayed == true) {
			ct.TriggerNormalDialogue();
			//Debug.Log("Talking");
		} else if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetButtonDown("Confirm") && ct != null && ct.isChoiceDepend == true) {
			ct.TriggerChoiceDependantConvo();
			//Debug.Log("Talking");
		}
	}

	public static void Test() {
		Debug.Log("event invoked");
	}
}




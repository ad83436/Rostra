using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float Speed;
    public Animator animator;
    private Rigidbody2D rb;
    private Vector2 moveVelocity;
    float horizontalMove;
    float verticalMove;
	private ConversationTrigger ct;
	private DialogueManager dm;
	DialogueToBattle dtm;
    private Vector2 moveInput;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveInput = new Vector2(0.0f, 0.0f);
        dm = DialogueManager.instance;
    }

    void LateUpdate()
    {
		if (dm.canWalk && !BattleManager.battleInProgress && !PauseMenuController.isPaused && !ItemShopUI.IsOpen && CutsceneManager.instance.isActive == false)
		{
			moveInput.x = Input.GetAxisRaw("Horizontal");
			moveInput.y = Input.GetAxisRaw("Vertical");
			moveVelocity = moveInput.normalized * Speed;
			horizontalMove = Input.GetAxisRaw("Horizontal") * Speed;
			verticalMove = Input.GetAxisRaw("Vertical") * Speed;

			animator.SetFloat("Horizontal", horizontalMove);
			animator.SetFloat("Vertical", verticalMove);
			animator.SetFloat("Speed", moveVelocity.sqrMagnitude);
			TalkToNPC();
		}
		else if(dm.canWalk == false)
		{
			moveVelocity = Vector2.zero;
			animator.SetFloat("Horizontal", 0);
			animator.SetFloat("Vertical", 0);
			animator.SetFloat("Speed", 0);
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			DialogueManager.instance.metAllChars = true;
			DialogueManager.instance.SetChoice(ChoiceEnum.metAllChars, true);
			DialogueManager.instance.dwarf = true;
			DialogueManager.instance.SetChoice(ChoiceEnum.dwarf, true);
			DialogueManager.instance.battleFarea = true;
			DialogueManager.instance.SetChoice(ChoiceEnum.battleFarea, true);
		}
	}


	void FixedUpdate()
    {
        if (!BattleManager.battleInProgress)
        {
            rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
        }
    }

	// this is all code for handeling getting the collision with the NPC and getting it's dialogue
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("NPC"))
		{
			ct = col.GetComponent<ConversationTrigger>();
			if (ct.pressZ != null)
			{
				ct.SetPressZ(true);
			}
		}
	}
	// set your CT to null once you exit the range of the NPC
	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("NPC") && col.GetComponent<ConversationTrigger>() == ct)
		{
			if (ct.pressZ != null)
			{
				ct.SetPressZ(false);
			}
			ct = null;
			
		}
	}
	// this is will handle talking to the NPC 
	public void TalkToNPC()
	{
		if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetButtonDown("Confirm") && ct != null && ct.isChoiceDepend == false && !ItemShopUI.IsOpen)
		{
			ct.TriggerConvo();
			Debug.Log("Talking");
		}
		else if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetButtonDown("Confirm") && ct != null && ct.dialogue.isChoice == true && ct.dialogue.hasPlayed == true)
		{
			ct.TriggerNormalDialogue();
			Debug.Log("Talking");
		}
		else if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetButtonDown("Confirm") && ct != null && ct.isChoiceDepend == true)
		{
			ct.TriggerChoiceDependantConvo();
			Debug.Log("Talking");
		}
	}

    public static void Test()
    {
        Debug.Log("event invoked");
    }
}
    



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
	private void Awake()
	{
		dm = FindObjectOfType<DialogueManager>();
	}
	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //moveInput = new Vector2(0.0f, 0.0f);
    }

    void Update()
    {
		if (dm.canWalk == true)
		{
			Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			moveVelocity = moveInput.normalized * Speed;
			horizontalMove = Input.GetAxisRaw("Horizontal") * Speed;
			verticalMove = Input.GetAxisRaw("Vertical") * Speed;

			animator.SetFloat("Horizontal", horizontalMove);
			animator.SetFloat("Horizontal", verticalMove);
			animator.SetFloat("Speed", moveVelocity.sqrMagnitude);
		}
		TalkToNPC();
	}
    
    void FixedUpdate()
    {
		rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }

	// this is all code for handeling getting the collision with the NPC and getting it's dialogue
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("NPC"))
		{
			ct = col.GetComponent<ConversationTrigger>();
		}
	}
	// set your CT to null once you exit the range of the NPC
	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("NPC"))
		{
			ct = null;
		}
	}
	// this is will handle talking to the NPC 
	public void TalkToNPC()
	{
		if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetKeyDown(KeyCode.Return) && ct != null && ct.isChoiceDepend == false && ct.dialogue.hasPlayed == false)
		{
			ct.TriggerConvo();
		}
		else if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetKeyDown(KeyCode.Return) && ct != null && ct.dialogue.isChoice == true && ct.dialogue.hasPlayed == true)
		{
			ct.TriggerNormalDialogue();
		}
		else if ((dm.nextDialogue == true && dm.isActive == false) && Input.GetKeyDown(KeyCode.Return) && ct != null && ct.isChoiceDepend == true)
		{
			ct.TriggerChoiceDependantConvo();
		}
	}
}
    



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempMove : MonoBehaviour
{
	private Rigidbody2D rb;
	private ConversationTrigger ct;
	private DialogueManager dm;
	public float speed;
    // Start is called before the first frame update
    void Awake()
    {
		rb = gameObject.GetComponent<Rigidbody2D>();
		dm = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
		TalkToNPC();
		MovePlayer();

	}

	public void MovePlayer()
	{
		if(dm.canWalk)
		{
			if (Input.GetKey(KeyCode.RightArrow))
			{
				rb.velocity = new Vector2(1 * speed, 0);
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				rb.velocity = new Vector2(-1 * speed, 0);
			}
			else if (Input.GetKey(KeyCode.UpArrow))
			{
				rb.velocity = new Vector2(0, 1 * speed);
			}
			else if (Input.GetKey(KeyCode.DownArrow))
			{
				rb.velocity = new Vector2(0, -1 * speed);
			}
			else
			{
				rb.velocity = Vector2.zero;
			}
		}
		
	}


	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("NPC"))
		{
			ct = col.GetComponent<ConversationTrigger>();
		}
	}
	private void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag("NPC"))
		{
			ct = null;
		}
	}

	public void TalkToNPC()
	{
		
		if ((dm.nextDialogue == true && dm.isActive == false)/* && canTalk == true*/ && Input.GetKeyDown(KeyCode.Return) && ct != null && ct.isChoiceDepend == false)
		{
				ct.TriggerConvo();
		}
		else if ((dm.nextDialogue == true && dm.isActive == false)/* && canTalk == true*/ && Input.GetKeyDown(KeyCode.Return) && ct != null && ct.isChoiceDepend == true)
		{
			Debug.Log("Triggered the convo");
				ct.TriggerChoiceDependantConvo();
		}
	}
}

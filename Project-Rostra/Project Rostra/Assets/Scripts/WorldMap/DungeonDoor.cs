using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
	public Dialogue success;
	public Dialogue failure;
	public Sprite open;
	private SpriteRenderer sr;
	public Collider2D collision;
	private bool isOpen;
	// Start is called before the first frame update
	void Start()
    {
		sr = GetComponent<SpriteRenderer>();
		isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player") && DialogueManager.instance.GetChoice(ChoiceEnum.dwarf) == true && Input.GetButtonDown("Confirm") && isOpen == false)
		{
			DialogueManager.instance.StartConversation(success);
			sr.sprite = open;
			isOpen = true;
			collision.enabled = false;
			this.enabled = false;
		}
		else if ((col.CompareTag("Player") && DialogueManager.instance.GetChoice(ChoiceEnum.dwarf) == false && Input.GetButtonDown("Confirm")))
		{
			DialogueManager.instance.StartConversation(failure);
		}
	}
}

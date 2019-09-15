using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempNPC : MonoBehaviour
{
	public float speed;
	public float changeDirection;
	private Rigidbody2D rb;
	private DialogueManager dm;
    // Start is called before the first frame update
    void Awake()
    {
		dm = FindObjectOfType<DialogueManager>();
		rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
		MoveNPC();

	}

	void MoveNPC()
	{
		float rand = Random.Range(0, 10);

		if (rand == changeDirection)
		{
			int rando = Random.Range(1, 4);
			switch (rando)
			{
				case 1:
					rb.velocity = new Vector2(1 * speed, 0);
					break;
				case 2:
					rb.velocity = new Vector2(-1 * speed, 0);
					break;
				case 3:
					rb.velocity = new Vector2(0, 1* speed);
					break;
				case 4:
					rb.velocity = new Vector2(0, -1 * speed);
					break;
				default:
					Debug.LogError("No direction was given to NPC");
					break;
			}
		}

	}
}

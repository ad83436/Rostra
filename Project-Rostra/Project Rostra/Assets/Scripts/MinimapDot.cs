using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapDot : MonoBehaviour
{
    public float Speed;


    private Rigidbody2D rb;
    private Vector2 moveVelocity;
    float horizontalMove;
    float verticalMove;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput.normalized * Speed;
        horizontalMove = Input.GetAxisRaw("Horizontal") * Speed;
        verticalMove = Input.GetAxisRaw("Vertical") * Speed;




    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
    }
}

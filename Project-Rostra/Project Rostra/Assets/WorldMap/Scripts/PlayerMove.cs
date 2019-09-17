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
    Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveInput = new Vector2(0.0f, 0.0f);
    }

    void Update()
    {
        
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveVelocity = moveInput.normalized * Speed;
        horizontalMove = Input.GetAxisRaw("Horizontal") * Speed;
        verticalMove = Input.GetAxisRaw("Vertical") * Speed;

        animator.SetFloat("Horizontal", horizontalMove);
        animator.SetFloat("Horizontal", verticalMove);
        animator.SetFloat("Speed", moveVelocity.sqrMagnitude);

    }

    void FixedUpdate()
            {
                rb.MovePosition(rb.position + moveVelocity * Time.fixedDeltaTime);
            }

 }
    



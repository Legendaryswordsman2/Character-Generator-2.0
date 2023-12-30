using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6.5f;

    [SerializeField] bool canMove = true;
    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public bool IsMoving { get; private set; } = false;

    Rigidbody2D rb;

    public Vector2 movement { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        movement = new Vector2(horizontal, vertical);

        if (movement != Vector2.zero)
            IsMoving = true;
        else
            IsMoving = false;
    }

    private void FixedUpdate()
    {
        if (canMove)
            rb.MovePosition(rb.position + moveSpeed * Time.fixedDeltaTime * movement.normalized);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box2d;
    Rigidbody2D rb2d;
    [SerializeField] float moveSpeed = 6.5f;
    [SerializeField] float jumpSpeed = 7f;

    float keyHorizontal;
    bool keyJump;
    bool keyShoot;
    bool isGrounded;
    bool isShooting;
    bool isFacingRight;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        box2d = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        isFacingRight = true;
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        // ground check
        Vector3 box_origin = box2d.bounds.center;
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, layerMask);

        //player box colliding with ground layer
        if (raycastHit.collider != null)
        {
            isGrounded = true;
        }

        //draw debug lines
        raycastColor = isGrounded ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
    }

    // Update is called once per frame
    void Update()
    {
        keyHorizontal = Input.GetAxisRaw("Horizontal");
        keyJump = Input.GetKeyDown(KeyCode.V);
        keyShoot = Input.GetKey(KeyCode.G);

        if (keyHorizontal < 0)
        {
            if (isGrounded)
            {
                animator.Play("Player_Run");
            }            
            rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y);
        }
        else if (keyHorizontal > 0)
        {
            if (isGrounded)
            {
                animator.Play("Player_Run");
            }
            rb2d.velocity = new Vector2(moveSpeed, rb2d.velocity.y);
        }
        else
        {
            if (isGrounded)
            {
                animator.Play("Player_Idle");
            }
            rb2d.velocity = new Vector2(0f, rb2d.velocity.y);
        }

        if (keyJump && isGrounded)
        {
            animator.Play("Player_Jump");
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
        }

        if (!isGrounded)
        {
            animator.Play("Player_Jump");
        }
    }
}

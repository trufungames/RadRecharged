using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class radMovement : MonoBehaviour
{
    [Header("Components")]

    private Rigidbody2D body;
    radGround ground;
    Animator animator;

    [Header("Movement Stats")]
    [SerializeField, Range(0f, 20f)][Tooltip("Maximum movement speed")] public float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed")] public float maxAcceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop after letting go")] public float maxDecceleration = 52f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction")] public float maxTurnSpeed = 80f;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to reach max speed when in mid-air")] public float maxAirAcceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop in mid-air when no direction is used")] public float maxAirDeceleration;
    [SerializeField, Range(0f, 100f)][Tooltip("How fast to stop when changing direction when in mid-air")] public float maxAirTurnSpeed = 80f;
    [SerializeField][Tooltip("Friction to apply against movement on stick")] private float friction;

    [Header("Options")]
    [Tooltip("When false, the charcter will skip acceleration and deceleration and instantly move and stop")] public bool useAcceleration;

    [Header("Calculations")]
    public float directionX;
    private Vector2 desiredVelocity;
    public Vector2 velocity;
    private float maxSpeedChange;
    private float acceleration;
    private float deceleration;
    private float turnSpeed;
    private bool isDucking;
    private float duckCooldownTime = 0f;
    private float duckCooldownTimeTotal = 0.15f;

    [Header("Current State")]
    public bool onGround;
    public bool pressingKey;

    private void Awake()
    {
        //Find the character's Rigidbody and ground detection script
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<radGround>();
        animator = GetComponent<Animator>();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        //if (!isDucking)
        //{
        //    directionX = context.ReadValue<float>();
        //}
        //else
        //{
        //    var dir = context.ReadValue<float>();

        //    if (dir != 0)
        //    {
        //        transform.localScale = new Vector3(dir > 0 ? 1 : -1, 1, 1);
        //    }
        //}
    }

    private void Update()
    {
        //Used to flip the character's sprite when she changes direction
        //Also tells us that we are currently pressing a direction button
        if (directionX != 0)
        {
            transform.localScale = new Vector3(directionX > 0 ? 1 : -1, 1, 1);
            pressingKey = true;
        }
        else
        {
            pressingKey = false;
        }

        if (duckCooldownTime < duckCooldownTimeTotal)
        {
            duckCooldownTime += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            if (!isDucking)
            {
                directionX = 1f;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (!isDucking)
            {
                directionX = -1f;
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        else
        {
            directionX = 0f;
        }

        if (onGround && Input.GetKey(KeyCode.S) && duckCooldownTime >= duckCooldownTimeTotal)
        {
            isDucking = true;
            var asi = animator.GetCurrentAnimatorStateInfo(0);

            if (!asi.IsName("Player_Duck") && !asi.IsName("Player_DuckShoot"))
            {
                animator.Play("Player_Duck");
            }

            directionX = 0f;  //stop any movement
        }
        else if (isDucking)
        {
            isDucking = false;

            var asi = animator.GetCurrentAnimatorStateInfo(0);

            if (asi.IsName("Player_Duck") || asi.IsName("Player_DuckShoot"))
            {
                animator.Play("Player_Idle");
            }

            duckCooldownTime = 0f;
        }

        //Calculate's the character's desired velocity - which is the direction you are facing, multiplied by the character's maximum speed
        //Friction is not used in this game
        desiredVelocity = new Vector2(directionX, 0f) * Mathf.Max(maxSpeed - friction, 0f);
    }

    private void FixedUpdate()
    {
        //Fixed update runs in sync with Unity's physics engine

        //Get Kit's current ground status from her ground script
        onGround = ground.GetOnGround();

        //Get the Rigidbody's current velocity
        velocity = body.velocity;

        //Calculate movement, depending on whether "Instant Movement" has been checked
        if (useAcceleration)
        {
            runWithAcceleration();
        }
        else
        {
            if (onGround)
            {
                runWithoutAcceleration();
            }
            else
            {
                runWithAcceleration();
            }
        }
    }

    private void runWithAcceleration()
    {
        //Set our acceleration, deceleration, and turn speed stats, based on whether we're on the ground on in the air=
        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        deceleration = onGround ? maxDecceleration : maxAirDeceleration;
        turnSpeed = onGround ? maxTurnSpeed : maxAirTurnSpeed;

        if (pressingKey)
        {
            //If the sign (i.e. positive or negative) of our input direction doesn't match our movement, it means we're turning around and so should use the turn speed stat.
            if (Mathf.Sign(directionX) != Mathf.Sign(velocity.x))
            {
                maxSpeedChange = turnSpeed * Time.deltaTime;
            }
            else
            {
                //If they match, it means we're simply running along and so should use the acceleration stat
                maxSpeedChange = acceleration * Time.deltaTime;
            }
        }
        else
        {
            //And if we're not pressing a direction at all, use the deceleration stat
            maxSpeedChange = deceleration * Time.deltaTime;
        }

        //Move our velocity towards the desired velocity, at the rate of the number calculated above
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        //Update the Rigidbody with this new velocity
        body.velocity = velocity;

    }

    private void runWithoutAcceleration()
    {
        //If we're not using acceleration and deceleration, just send our desired velocity (direction * max speed) to the Rigidbody
        velocity.x = desiredVelocity.x;

        body.velocity = velocity;
    }
}

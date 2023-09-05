using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class radGround : MonoBehaviour
{
    [SerializeField] BoxCollider2D box2d;

    private bool onGround;

    [Header("Collider Settings")]
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 0.95f;
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 colliderOffset;

    [Header("Layer Masks")]
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;

    private void Start()
    {
        box2d = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        float raycastDistance = 0.05f;
        Vector3 box_origin = box2d.bounds.center;
        box_origin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        var raycastHit = Physics2D.BoxCast(box_origin, box_size, 0f, Vector2.down, raycastDistance, groundLayer);

        //player box colliding with ground layer
        if (raycastHit.collider != null)
        {
            onGround = true;
        }
        else
        {
            onGround = false;
        }

        var raycastColor = onGround ? Color.green : Color.red;
        Debug.DrawRay(box_origin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_origin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.right * (box2d.bounds.extents.x * 2), raycastColor);
        //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
        //onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
    }

    private void OnDrawGizmos()
    {
        ////Draw the ground colliders on screen for debug purposes
        //if (onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        //Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        //Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    //Send ground detection to other scripts
    public bool GetOnGround() { return onGround; }
}

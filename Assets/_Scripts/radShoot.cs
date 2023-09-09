using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class radShoot : MonoBehaviour
{
    private radJuice juice;
    private Animator animator;
    private bool pressingShoot = false;
    private bool canShoot = true;
    private float shootTime = 0f;
    private float shootTimeTotal = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        juice = GetComponentInChildren<radJuice>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressingShoot && canShoot)
        {
            canShoot = false;
            shootTime = 0;
            animator.SetTrigger("Shoot");
            animator.SetBool("isShooting", true);
            Invoke("StopShooting", 0.15f);
            //TODO Fire bullet
        }

        if (!canShoot && !pressingShoot)
        {
            shootTime += Time.deltaTime;

            if (shootTime >= shootTimeTotal)
            {
                canShoot = true;
                StopShooting();
            }
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            pressingShoot = true;
        }

        if (context.canceled)
        {
            pressingShoot = false;
        }
    }
    private void StopShooting()
    {
        if (!pressingShoot)
        {
            animator.SetBool("isShooting", false);
            canShoot = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class radShoot : MonoBehaviour
{
    private radJuice juice;
    private radMovement movement;
    private Animator animator;
    private bool pressingShoot = false;
    private bool canShoot = true;
    private float canShootTime = 0f;
    private float canShootTimeTotal = 0.15f;
    private bool isShooting = false;
    private float shootTime = 0f;
    private float shootTimeTotal = 0.25f;

    [SerializeField] int bulletDamage = 1;
    [SerializeField] float bulletSpeed = 25;
    [SerializeField] Transform gunBarrelPos;
    [SerializeField] GameObject bulletPrefab;

    // Start is called before the first frame update
    void Start()
    {
        juice = GetComponentInChildren<radJuice>();
        movement = GetComponentInChildren<radMovement>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pressingShoot && canShoot)
        {
            canShoot = false;
            isShooting = true;
            canShootTimeTotal = 0f;
            animator.SetTrigger("Shoot");
            animator.SetBool("isShooting", true);
            Invoke("StopShooting", 0.25f);

            var asi = animator.GetCurrentAnimatorStateInfo(0);

            if (asi.IsName("Player_Idle"))
            {
                animator.Play("Player_Shoot");
            }
            else if (asi.IsName("Player_Run"))
            {
                animator.Play("Player_RunShoot", 0, asi.normalizedTime);
            }
            else if (asi.IsName("Player_Jump_Ascend"))
            {
                animator.Play("Player_JumpShoot_Ascend");
            }
            else if (asi.IsName("Player_Jump_Apex"))
            {
                animator.Play("Player_JumpShoot_Apex");
            }
            else if (asi.IsName("Player_Jump_Fall"))
            {
                animator.Play("Player_JumpShoot_Fall");
            }
            else if (asi.IsName("Player_Duck"))
            {
                animator.Play("Player_DuckShoot");
            }

            ShootBullet();
        }

        if (!canShoot && !pressingShoot)
        {
            canShootTime += Time.deltaTime;

            if (canShootTime >= canShootTimeTotal)
            {
                canShoot = true;
            }
        }

        if (isShooting)
        {
            shootTime += Time.deltaTime;

            if (shootTime >= shootTimeTotal)
            {
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
        animator.SetBool("isShooting", false);
        isShooting = false;

        var asi = animator.GetCurrentAnimatorStateInfo(0);

        if (asi.IsName("Player_Shoot"))
        {
            animator.Play("Player_Idle");
        }
        else if (asi.IsName("Player_RunShoot"))
        {
            animator.Play("Player_Run", 0, asi.normalizedTime);
        }
        else if (asi.IsName("Player_JumpShoot_Ascend"))
        {
            animator.Play("Player_Jump_Ascend");
        }
        else if (asi.IsName("Player_JumpShoot_Apex"))
        {
            animator.Play("Player_Jump_Apex");
        }
        else if (asi.IsName("Player_JumpShoot_Fall"))
        {
            animator.Play("Player_Jump_Fall");
        }
        else if (asi.IsName("Player_DuckShoot"))
        {
            animator.Play("Player_Duck");
        }
    }

    void ShootBullet()
    {
        juice.ShootEffects(movement.transform.localScale.x);

        GameObject bullet = Instantiate(bulletPrefab, gunBarrelPos.position, Quaternion.identity);
        bullet.name = bulletPrefab.name;
        bullet.GetComponent<BulletScript>().SetDamageValue(bulletDamage);
        bullet.GetComponent<BulletScript>().SetBulletSpeed(bulletSpeed);
        bullet.GetComponent<BulletScript>().SetBulletDirection(new Vector2(movement.transform.localScale.x, 0));
        bullet.GetComponent<BulletScript>().Shoot();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBlastScript : MonoBehaviour
{
    [SerializeField] Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        var animationInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (animationInfo.normalizedTime > 1)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(string animation)
    {
        animator.Play(animation, -1, 0f);
    }
}

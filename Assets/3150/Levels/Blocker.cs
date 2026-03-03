using System;
using UnityEngine;

public class Blocker : MonoBehaviour
{

    public BlockerUpgrade blockerUpgrade;
    public Animator animator;

    internal BlockerUpgrade GetUpdateRequired()
    {
        return blockerUpgrade;
    }


    public void RemoveBlocker()
    {
        animator.SetTrigger("destroy");
    }

    public void DestroyAnimationOver()
    {
        Destroy(gameObject);
    }
}

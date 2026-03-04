using System;
using UnityEditor.Search;
using UnityEngine;

public class Blocker : MonoBehaviour
{

    public BlockerUpgrade blockerUpgrade;
    public Animator animator;
    public bool needsAnimator;
    public Level level;
    public GameObject failMessage;

    internal BlockerUpgrade GetUpdateRequired()
    {
        return blockerUpgrade;
    }


    public void RemoveBlocker()
    {
        if (needsAnimator)
        {
            animator.SetTrigger("destroy");
        }else
        {
            Destroy(gameObject);
        }
    }

    public void DestroyAnimationOver()
    {
        Destroy(gameObject);
    }

    internal Level GetLevel()
    {
        return level;
    }

    public void ShowFailMessage()
    {
        failMessage.SetActive(true);
    }

    public void HideFailMessage()
    {
        failMessage.SetActive(false);
    }
}

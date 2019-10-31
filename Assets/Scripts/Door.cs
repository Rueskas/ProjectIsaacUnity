using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    public enum DoorType { Normal, Treasure, Boss};
    private bool isOpened;
    private bool needKey = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetType(DoorType type)
    {
        StartCoroutine("SetTypeWaitingToAnimator", type);

    }

    IEnumerator SetTypeWaitingToAnimator(DoorType type)
    {
        yield return new WaitUntil(() => animator != null);
        int level = FindObjectOfType<GameController>().GetLevel();
        animator.SetInteger("Level", level);
        switch (type)
        {
            case DoorType.Normal:
                animator.SetBool("IsNormalDoor", true);
                animator.Play("Tree");
                break;
            case DoorType.Treasure:
                animator.SetBool("IsTreasureDoor", true);
                animator.Play("Tree");
                break;
            case DoorType.Boss:
                animator.SetBool("IsBossDoor", true);
                animator.Play("Tree");
                break;
        }
        if (type == DoorType.Treasure && level > 1)
        {
            needKey = true;
        }
    }

    public void Open()
    {
        if (!needKey)
        {
            isOpened = true;
            animator.SetBool("Opened", true);
        }
       
    }
    public void SetIsOpened(bool isOpened)
    {
        this.isOpened = isOpened;
    }

    public bool GetIsOpened()
    {
        return isOpened;
    }

    public bool GetNeedKey()
    {
        return needKey;
    }

    public void OpenWithKey()
    {
        isOpened = true;
        animator.SetBool("Opened", true);
    }
}
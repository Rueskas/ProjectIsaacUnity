using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool opened = false;
    private Animator animator;
    public enum DoorType { Normal, Treasure, Boss};
    private DoorType doorType;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetType(doorType);
    }

    public void SetType(DoorType type)
    {
        StartCoroutine("SetTypeWaitingToAnimator", type);

    }

    IEnumerator SetTypeWaitingToAnimator(DoorType type)
    {
        yield return new WaitUntil(() => animator != null);
        switch (type)
        {
            case DoorType.Normal:
                animator.SetBool("IsNormalDoor", true);
                break;
            case DoorType.Treasure:
                animator.SetBool("IsTreasureDoor", true);
                break;
            case DoorType.Boss:
                animator.SetBool("IsBossDoor", true);
                break;
        }
    }

    public void Open()
    {
        opened = true;
        animator.SetBool("Opened", true);
    }

}
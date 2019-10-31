using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] Sprite fullContainer;
    [SerializeField] Sprite halfContainer;
    [SerializeField] Sprite emptyContainer;
    public enum containerState { Full, Half, Empty};
    private int currentPosition;
    private containerState currentContainerState;
    Image[] hearthImages;

    void Start()
    {
        hearthImages = GetComponentsInChildren<Image>();
        currentPosition = hearthImages.Length-1;
        currentContainerState = containerState.Full;
        foreach(Image image in hearthImages)
        {
            image.sprite = fullContainer;
        }
    }

    public void Damaged()
    {
        Image currentImage = hearthImages[currentPosition];
        switch (currentContainerState)
        {
            case containerState.Full:
                currentImage.sprite = halfContainer;
                currentContainerState = containerState.Half;
                break;
            case containerState.Half:
                currentImage.sprite = emptyContainer;
                currentContainerState = containerState.Full;
                currentPosition--;
                break;
        }
    }

    public void Healthed(int amountInHalfHealths)
    {
        for(int i = 0; i < amountInHalfHealths; i++)
        {
            Image currentImage = hearthImages[currentPosition];
            switch (currentContainerState)
            {
                case containerState.Half:
                    currentImage.sprite = fullContainer;
                    currentContainerState = containerState.Empty;
                    currentPosition++;
                    break;
                case containerState.Empty:
                    currentImage.sprite = halfContainer;
                    currentContainerState = containerState.Half;
                    break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Healthed(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Healthed(2);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Damaged();
        }
    }

    public int GetLevel()
    {
        return level;
    }
}

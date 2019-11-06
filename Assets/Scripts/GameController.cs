using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField] private int level = 1;
    [SerializeField] private Sprite fullContainer;
    [SerializeField] private Sprite halfContainer;
    [SerializeField] private Sprite emptyContainer;
    [SerializeField] private GameObject waitingStartImage;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject bossFightPanel;
    [SerializeField] private Text informationText;
    [SerializeField] private AudioClip mainSong;
    [SerializeField] private AudioClip bossSong;
    [SerializeField] private AudioClip BossFightAudio;
    [SerializeField] private AudioClip waitingAudio;
    [SerializeField] private TextMeshProUGUI bossNameTitle;
    [SerializeField] private Sprite[] imageBossFight;
    private AudioSource audioSource;
    public enum containerState { Full, Half, Empty};
    private int currentPosition;
    private containerState currentContainerState;
    private List<Image> hearthImages;
    private const float countFade = 0.01f;
    private bool startedLevel = false;
    private bool CheatMode = false;
    private bool gamePaused = false;
    private bool gameFinished = false;

    public static int damagePasiveItems = 40;
    private static GameController _instance;

    void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hearthImages = new List<Image>();
        Image[] images = GetComponentsInChildren<Image>();
        foreach(Image image in images)
        {
            if(image.tag == "Container")
            {
                hearthImages.Add(image);
                image.sprite = fullContainer;
            }
        }
        currentPosition = hearthImages.Count - 1;
        currentContainerState = containerState.Full;
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
                case containerState.Full:
                    currentPosition++;
                    currentImage = hearthImages[currentPosition];
                    currentImage.sprite = halfContainer;
                    currentContainerState = containerState.Half;
                    break;
                case containerState.Half:
                    currentImage.sprite = fullContainer;
                    currentContainerState = containerState.Full;
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
        if (Input.GetKeyDown(KeyCode.Escape) && !gameFinished)
        {
            if(Time.timeScale == 0)
            {
                gamePaused = false;
                Time.timeScale = 1;
                audioSource.Play();
                pausePanel.SetActive(false);
            }
            else
            {
                gamePaused = true;
                Time.timeScale = 0;
                audioSource.Pause();
                pausePanel.SetActive(true);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && gameFinished)
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(FindObjectOfType<Player>().gameObject);
            Time.timeScale = 1;
            Destroy(this.gameObject);
        }

        if(Input.GetKeyDown(KeyCode.Space) && gamePaused)
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
                Input.GetKeyDown(KeyCode.Return))
        {
            CheatMode = true;
        }

        if (CheatMode)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                Healthed(1);

            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                Healthed(2);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                FindObjectOfType<Player>().GetTears().GetComponent<Tear>().SetDamage(200);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                FindObjectOfType<Player>().GetTears().GetComponent<Tear>().SetDamage(20);
            }
        }
    }

    public int GetLevel()
    {
        return level;
    }

    IEnumerator FadeOutWaitingStartImage()
    {
        audioSource.clip = waitingAudio;
        audioSource.Play();
        yield return new WaitForSeconds(1.5f);
        Image[] waitingImages = 
            waitingStartImage.GetComponentsInChildren<Image>();

        while (waitingImages[0].color.a > 0)
        {
            foreach(Image image in waitingImages)
            {
                image.color =
                    new Color(image.color.r, image.color.g, image.color.b, 
                        image.color.a - countFade);
                yield return new WaitForSeconds(0.0001f);
            }
        }
        startedLevel = true;

        audioSource.clip = mainSong;
        audioSource.loop = true;
        audioSource.Play();
    }

    IEnumerator SetMessageItem(string message)
    {
        informationText.gameObject.SetActive(true);
        informationText.text = message;
        yield return new WaitForSecondsRealtime(1.25f);
        informationText.gameObject.SetActive(false);
    }

    IEnumerator ShowImageBossFight(GameObject boss)
    {
        audioSource.clip = BossFightAudio;
        audioSource.loop = false;
        audioSource.Play();
        GameObject panel = bossFightPanel.transform.Find("BossImage").gameObject;
        panel.GetComponent<Image>().sprite = GetImageBossWithName(boss.name);
        bossNameTitle.text =
            boss.name.Replace("(Clone)", "");
        bossFightPanel.SetActive(true);
        Time.timeScale = 0;
        yield return new WaitWhile(() => audioSource.isPlaying);
        Time.timeScale = 1;
        bossFightPanel.SetActive(false);
        audioSource.clip = bossSong;
        audioSource.Play();
        boss.SetActive(true);
    }

    private void FinishRun()
    {
        gameFinished = true;
        gamePaused = true;
        Time.timeScale = 0;
        audioSource.Pause();
        TextMeshProUGUI textTitle =
            pausePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        textTitle.text = "GAME OVER";

        pausePanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>()
            .text = "Press escape to main menu";
        pausePanel.SetActive(true);
    }

    public bool GetStartedLevel()
    {
        return startedLevel;
    }

    public bool GetGameIsPaused()
    {
        return gamePaused;
    }

    public void NextLevel()
    {
        FindObjectOfType<Player>().transform.position = Vector3.zero;
        level++;
        damagePasiveItems = level * 40;
        startedLevel = false;
        FindObjectOfType<Player>().SetHasTreasureRoomKey(false);
        Image[] waitingImages =
             waitingStartImage.GetComponentsInChildren<Image>();

        SceneManager.LoadScene("NewGame");
        foreach (Image image in waitingImages)
        {
            image.color = Color.black;
        }
        FindObjectOfType<CameraController>().SendMessage("ResetPosition");
        transform.position = new Vector3(0, 0, 15);
    }

    private Sprite GetImageBossWithName(string nameBoss)
    {
        nameBoss = nameBoss.Replace("(Clone)", "");
        switch (nameBoss)
        {
            case "Dingle":
                return imageBossFight[0];
            case "The Haunt":
                return imageBossFight[1];
        }
        return null;
    }
}

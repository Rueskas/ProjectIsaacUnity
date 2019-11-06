using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Image imageMonsters;
    [SerializeField] private Image imageTitle;
    [SerializeField] private Image imageIsaac;
    [SerializeField] private TextMeshProUGUI textNewGame;
    private bool menuShown = false;
    private const float countFade = 0.01f;
    void Start()
    {
        StartCoroutine("Animate");
    }

    private void Update()
    {
        if (menuShown == false && Input.anyKeyDown)
        {
            StopCoroutine("Animate");
            imageTitle.color = new Color(1, 1, 1, 1);
            imageIsaac.color = new Color(1, 1, 1, 1);
            imageMonsters.color = new Color(1, 1, 1, 1);
            ShowMenu();
        }
        else if (Input.GetKeyDown(KeyCode.Space) ||
                Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine("SelectItem");
        }
    }

    IEnumerator SelectItem()
    {
        StartCoroutine("FadeOutImage", imageIsaac);
        StartCoroutine("FadeOutImage", imageTitle);
        StartCoroutine("FadeOutImage", imageMonsters);
        yield return new WaitUntil(() => imageMonsters.color.a <= 0);
        SceneManager.LoadScene("NewGame");
    }

    private void ShowMenu()
    {
        textNewGame.gameObject.SetActive(true);
        menuShown = true;
        InvokeRepeating("BlinkMenuItem", 0, 0.5f);
    }

    private void BlinkMenuItem()
    {
        textNewGame.color = new Color(1, 1, 1, -textNewGame.color.a);
    }

    IEnumerator Animate()
    {
        yield return new WaitUntil(() => imageIsaac.color.a <= 0);
        StartCoroutine("FadeInImage", imageIsaac);

        yield return new WaitUntil(() => imageIsaac.color.a >= 1);

        StartCoroutine("FadeOutImage", imageIsaac);

        yield return new WaitUntil(() => imageIsaac.color.a <= 0);

        StartCoroutine("FadeInImage", imageMonsters);

        yield return new WaitUntil(() => imageMonsters.color.a >= 1);

        StartCoroutine("FadeOutImage", imageMonsters);

        yield return new WaitUntil(() => imageMonsters.color.a <= 0);

        StartCoroutine("FadeInImage", imageIsaac);
        StartCoroutine("FadeInImage", imageMonsters);
        StartCoroutine("FadeInImage", imageTitle);

        yield return new WaitUntil(() => imageTitle.color.a >= 1);
        ShowMenu();
    }

    IEnumerator FadeInImage(Image image)
    {
        yield return new WaitForSeconds(0.0001f);
        while (image.color.a < 1)
        {
            image.color =
                new Color(1, 1, 1, image.color.a + countFade);
            yield return new WaitForSeconds(0.0001f);
        }
    }

    IEnumerator FadeOutImage(Image image)
    {
        yield return new WaitForSeconds(0.0001f);
        while (image.color.a > 0)
        {
            image.color =
                new Color(1, 1, 1, image.color.a - countFade);
            yield return new WaitForSeconds(0.0001f);
        }
    }

}

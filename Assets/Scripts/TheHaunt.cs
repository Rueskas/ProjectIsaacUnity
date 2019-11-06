using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheHaunt : MonoBehaviour
{
    private float baseLive = 600;
    private float liveBonus = 200;
    private float totalLive;
    private float currentLive;
    private GameObject imageCurrentLive;
    private float offsetSubstract = 0.048f;
    private float offsetPercent = 0.02666f;
    [SerializeField] private float distanceChange = 1f;
    [SerializeField] private int currentPosition = 0;
    [SerializeField] private GameObject tear;
    GameObject[] wayPoints;
    Vector2 nextPosition;
    [SerializeField] private List<MiniHaunt> miniHaunts;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private float speed = 2f;
    private bool isActive = false;
    private int miniHauntsAlive = 3;
    [SerializeField] private float shotSpeed = 50f;
    [SerializeField] private float timeShot = 1;

    void Start()
    {
        int level = FindObjectOfType<GameController>().GetLevel();
        currentLive = totalLive = baseLive + (liveBonus * level);
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Invoke("FindImageBar", 1f);
        Invoke("ActiveMiniHaunts", Random.Range(2, 5));
    }

    void Update()
    {
        Transform player = FindObjectOfType<Player>().transform;
        if (isActive)
        {
            transform.position =
                Vector2.MoveTowards(
                    transform.position, nextPosition, speed * Time.deltaTime);

            if (isActive &&
                wayPoints[0].transform.position.y - player.position.y < 1f)
            {
                speed = 6f;
                if(animator.GetCurrentAnimatorStateInfo(0)
                    .IsName("Moving"))
                {
                    animator.Play("PrepareCharge");
                }
            }
            else
            {
                speed = 2f;
            }
            
            if (Vector2.Distance(
                transform.position, nextPosition) < distanceChange){
                currentPosition++;
                if (currentPosition >= wayPoints.Length)
                {
                    currentPosition = 0;
                }
                nextPosition = wayPoints[currentPosition].transform.position;
            }
        }
        else
        {
            if (Vector2.Distance(
                transform.position, nextPosition) < distanceChange)
            {
                nextPosition = RandomPointInBounds();
            }
            transform.position =
               Vector2.MoveTowards(
                   transform.position, nextPosition, speed * Time.deltaTime);
        }
    }

    private void ActiveMiniHaunts()
    {
        miniHaunts[miniHaunts.Count - 1].Active();
        miniHaunts.RemoveAt(miniHaunts.Count - 1);
    }

    public void DiedMiniHaunt()
    {
        miniHauntsAlive--;
        if(miniHauntsAlive == 2)
        {
            for(int i = 0; i < 2; i++)
            {
                ActiveMiniHaunts();
            }
        }
        if (miniHauntsAlive == 0)
        {
            spriteRenderer.color = Color.white;
            animator.Play("FinishImmortal");
            wayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
            nextPosition = wayPoints[0].transform.position;
            isActive = true;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Shot()
    {
        yield return new WaitForSeconds(Random.Range(3,6));
        animator.Play("Shot");
        for(int i = 0; i < 7; i++)
        {
            Transform player = FindObjectOfType<Player>().transform;
            Vector2 direction = player.position - transform.position;
            GameObject tearInstantiated =
                Instantiate(tear, transform.position - new Vector3(0, 0.3f, 0), Quaternion.identity);
            tearInstantiated.GetComponent<Rigidbody2D>().AddForce(direction * shotSpeed);
            tearInstantiated.GetComponent<EnemyTear>().Invoke("StartAnim", timeShot);
            yield return new WaitForSeconds(0.05f);
        }

        StartCoroutine("Shot");
    }

    public void FindImageBar()
    {
        imageCurrentLive =
            GameObject.Find("CurrentLive").gameObject;
        GameObject parent = transform.parent.gameObject;
        transform.parent.DetachChildren();
        Destroy(parent);
        nextPosition = RandomPointInBounds();
    }

    IEnumerator ChangeColorDamaged()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);
        spriteRenderer.color = Color.white;

    }

    public void Destroy()
    {
        GameObject bossRoom = GameObject.Find("BossRoom(Clone)").gameObject;
        bossRoom.GetComponent<Room>().SendMessage("EnemyDeath");
        Destroy(this.gameObject);
    }

    public void DecreaseLiveBar(float damage)
    {
        float newScaleX = (currentLive - damage) / totalLive;
        float substract =
            ((damage / totalLive) * offsetSubstract) / offsetPercent;
        currentLive -= damage;
        imageCurrentLive.transform.localScale =
            new Vector3(newScaleX,
                imageCurrentLive.transform.localScale.y);
        imageCurrentLive.transform.position =
            new Vector3(imageCurrentLive.transform.position.x - substract,
                imageCurrentLive.transform.position.y,
                imageCurrentLive.transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.gameObject.tag == "Tear")
        {
            StartCoroutine("ChangeColorDamaged");
            float damage =
                    collision.gameObject.GetComponent<Tear>().GetDamage();
            DecreaseLiveBar(damage);
            if (currentLive <= 0)
            {
                imageCurrentLive.SetActive(false);
                animator.Play("Death");
                Invoke("Destroy", 1f);
            }
        }
        if (isActive && collision.gameObject.tag == "ItemPasiveDamage")
        {
            StartCoroutine("ChangeColorDamaged");
            float damage = 
                FindObjectOfType<GameController>().GetLevel() *
                     GameController.damagePasiveItems;
            DecreaseLiveBar(damage);
            if (currentLive <= 0)
            {
                imageCurrentLive.SetActive(false);
                animator.Play("Death");
            }
        }
    }


    private Vector2 RandomPointInBounds()
    {
        GameObject bossRoom = GameObject.Find("BossRoom(Clone)").gameObject;
        Bounds bounds = bossRoom.transform.GetChild(0).
                GetComponent<Collider2D>().bounds;
        Vector2 randomPosition = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
        return randomPosition;
    }
}

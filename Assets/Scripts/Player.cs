using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float shotSpeed = 180f;
    [SerializeField] private float timeShot = 1;
    [SerializeField] private float shotsPerInput = 1;
    [SerializeField] private float lastShotTime = 0;
    [SerializeField] private float timeBeweenShots = 0.5f;
    [SerializeField] private float maxLives;
    [SerializeField] private float currentLives;
    [SerializeField] private float halfHearth = 0.5f;
    [SerializeField] private GameObject tearShot;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject head;
    [SerializeField] private AudioClip[] damagedClips;
    [SerializeField] private AudioClip deathClip;
    private GameController game;
    private AudioSource audioSource;
    private Animator animatorBody;
    private Animator animatorHead;
    private SpriteRenderer spriteBody;
    private SpriteRenderer spriteHead;
    private bool isDamaged = false;
    private bool isAlive = true;
    private bool hasTreasureRoomKey = false;
    private bool blockedMovement = false;
    private enum Direction { ToLeft, ToRight, ToUp, ToDown, Idle};
    private Direction direction;

    private static Player _instance;

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
        game = FindObjectOfType<GameController>();
        audioSource = GetComponent<AudioSource>();
        animatorBody = body.GetComponent<Animator>();
        animatorHead = head.GetComponent<Animator>();
        spriteBody = body.GetComponent<SpriteRenderer>();
        spriteHead = head.GetComponent<SpriteRenderer>();
        direction = Direction.ToDown;
        currentLives = maxLives = 3;
    }

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (isAlive && !blockedMovement)
        {
            transform.Translate(horizontal * Time.deltaTime * speed, vertical * Time.deltaTime * speed, 0);
            if (!isDamaged)
            {
                Animate();
            }

            StartCoroutine("InputShot");
        }
        
    }

    IEnumerator InputShot()
    {
        if(Time.time - lastShotTime > timeBeweenShots)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                for(int i = 0; i < shotsPerInput; i++)
                {
                    GameObject tear = Instantiate(
                        tearShot, transform.position, Quaternion.identity);
                    tear.GetComponent<Rigidbody2D>()
                        .AddForce(new Vector2(0, shotSpeed));
                    tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                    lastShotTime = Time.time;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                for (int i = 0; i < shotsPerInput; i++)
                {
                    GameObject tear = Instantiate(
                        tearShot, transform.position, Quaternion.identity);
                    tear.GetComponent<Rigidbody2D>()
                        .AddForce(new Vector2(-shotSpeed, 0));
                    tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                    lastShotTime = Time.time;
                    spriteHead.flipX = true;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                for (int i = 0; i < shotsPerInput; i++)
                {
                    GameObject tear = Instantiate(
                        tearShot, transform.position, Quaternion.identity);
                    tear.GetComponent<Rigidbody2D>()
                        .AddForce(new Vector2(0, -shotSpeed));
                    tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                    lastShotTime = Time.time;
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                for (int i = 0; i < shotsPerInput; i++)
                {
                    GameObject tear = Instantiate(
                            tearShot, transform.position, Quaternion.identity);
                    tear.GetComponent<Rigidbody2D>()
                        .AddForce(new Vector2(shotSpeed, 0));
                    tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                    lastShotTime = Time.time;
                    spriteHead.flipX = false;
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }

    private void Animate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        if (vertical > 0)
        {
            direction = Direction.ToUp;
        }
        else if (vertical < 0)
        {
            direction = Direction.ToDown;
        }
        else if (horizontal > 0)
        {
            direction = Direction.ToRight;
        }
        else if (horizontal < 0)
        {
            direction = Direction.ToLeft;
        }
        else
        {
            direction = Direction.Idle;
        }
        AnimateBody();
        AnimateHead();
    }
    private void AnimateBody()
    {
        
        if (direction == Direction.ToUp || direction == Direction.ToDown)
        {
            animatorBody.Play("WalkUpDown");
        }
        else if (direction == Direction.ToLeft)
        {
            animatorBody.Play("WalkLeftRight");
            spriteBody.flipX = true;
        }
        else if (direction == Direction.ToRight)
        {
            animatorBody.Play("WalkLeftRight");
            spriteBody.flipX = false;
        }
        else
        {
            animatorBody.Play("Idle");
        }
    }

    private void AnimateHead()
    {
        if(direction == Direction.ToUp)
        {
            animatorHead.Play("LookToUp");
        }
        else if (direction == Direction.ToLeft)
        {
            animatorHead.Play("LookToLeftRight");
            spriteHead.flipX = true;
        } 
        else if (direction == Direction.ToRight)
        {
            animatorHead.Play("LookToLeftRight");
            spriteHead.flipX = false;
        }
        else if (direction == Direction.ToDown ||direction == Direction.ToDown)
        {
            animatorHead.Play("LookToDown");
        }
    }

    public void SetActiveCollider(bool isActive)
    {
        GetComponent<Collider2D>().enabled = isActive;
    }

    IEnumerator ChangeColorDamaged()
    {
        spriteBody.color = Color.red;
        yield return new WaitForSecondsRealtime(0.3f);

        spriteBody.color = Color.white;

        animatorHead.gameObject.SetActive(true);
        isDamaged = false;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        animatorBody.SetBool("IsDamaged", isDamaged);
    }

    IEnumerator ItemTaken(string message)
    {
        game.StartCoroutine("SetMessageItem", message);
        animatorHead.gameObject.SetActive(false);
        animatorHead.enabled = false;
        animatorBody.Play("ItemTaken");
        blockedMovement = true;
        yield return new WaitForSecondsRealtime(1.25f);
        animatorHead.gameObject.SetActive(true);
        animatorHead.enabled = true;
        blockedMovement = false;
    }

    public GameObject GetTears()
    {
        return tearShot;
    }

    public void IsDamaged()
    {
        currentLives -= halfHearth;
        if(currentLives != 0)
        {
            audioSource.clip = 
                damagedClips[Random.Range(0, damagedClips.Length)];
            audioSource.Play();
            animatorHead.gameObject.SetActive(false);
            StartCoroutine("ChangeColorDamaged");
            animatorBody.SetBool("IsDamaged", isDamaged);
        }
        else
        {
            audioSource.clip = deathClip;
            audioSource.Play();
            animatorHead.gameObject.SetActive(false);
            isAlive = false;
            animatorBody.SetBool("IsAlive", isAlive);
            animatorBody.Play("Death");
            game.SendMessage("FinishRun");
        }
    }

    public bool GetHasTreasureRoomKey()
    {
        return hasTreasureRoomKey;
    }

    public void SetHasTreasureRoomKey(bool hasTreasureRoomKey)
    {
        this.hasTreasureRoomKey = hasTreasureRoomKey;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Door")
        {
            Door door = collision.GetComponent<Door>();
            if(door.GetIsOpened()== true)
            {
                switch (collision.name)
                {
                    case "DoorPointLeft":
                        transform.position = transform.position + new Vector3(
                             LevelController.offsetBetweenDoorsX, 0, 0);
                        break;
                    case "DoorPointRight":
                        transform.position = transform.position + new Vector3(
                            -LevelController.offsetBetweenDoorsX, 0, 0);
                        break;
                    case "DoorPointUp":
                        transform.position = transform.position + new Vector3(
                            0, LevelController.offsetBetweenDoorsY, 0);
                        break;
                    case "DoorPointDown":
                        transform.position = transform.position + new Vector3(
                            0, -LevelController.offsetBetweenDoorsY, 0);
                        break;
                }
            }
            else
            {
                bool needKey = door.GetNeedKey();
                if (needKey && hasTreasureRoomKey)
                {
                    door.SendMessage("OpenWithKey");
                }
            }
        }
        else if(collision.tag == "RoomFloor")
        {
            Room room = collision.gameObject.GetComponentInParent<Room>();
            GameObject pointZero = room.GetPointZero();

            CameraController camera = FindObjectOfType<CameraController>();
            camera.Move(pointZero.transform);
            room.EnterFocus();
        }
        else if (collision.tag.Contains("Item"))
        {
            animatorHead.enabled = false;
            switch (collision.tag)
            {
                case "SpeedBallItem":
                    StartCoroutine("ItemTaken", "Speed Ball: Speed shot up!");
                    timeBeweenShots -= (timeBeweenShots / 3f);
                    Destroy(collision.gameObject);
                    break;
                case "PolyphemusItem":
                    StartCoroutine(
                        "ItemTaken", "Polyphemus: Look that tears!");
                    Vector2 currentScale = 
                        tearShot.GetComponent<Tear>().GetScale();
                    tearShot.GetComponent<Tear>().AddScale(
                        new Vector3(
                            currentScale.x * 1.15f,
                            currentScale.y * 1.15f, 0));
                    Destroy(collision.gameObject);
                    break;
                case "DoubleShotItem":
                    StartCoroutine(
                        "ItemTaken", "Double Shot: I am seeing double?");
                    shotsPerInput *= 2;
                    Destroy(collision.gameObject);
                    break;
                case "CubeOfMeatItem":
                    StartCoroutine(
                        "ItemTaken", "Cube of meat: It's time to eat");
                    collision.GetComponent<CubeOfMeat>()
                        .SetPlayer(this.gameObject);
                    collision.tag = "ItemPasiveDamage";
                    collision.transform.position = 
                        (collision.transform.position - transform.position)
                        .normalized/1.5f + collision.transform.position;
                    collision.transform.parent = this.transform;
                    break;
                case "InnerEyeItem":
                    StartCoroutine(
                        "ItemTaken", "Inner Eye: shot, shot, shot");
                    shotsPerInput += 2;
                    int currentDamage = 
                        tearShot.GetComponent<Tear>().GetDamage();
                    tearShot.GetComponent<Tear>().SetDamage(currentDamage-7);
                    Destroy(collision.gameObject);
                    break;


            }
        }
        else if (collision.tag == "TreasureRoomKey")
        {
            hasTreasureRoomKey = true;
            Destroy(collision.gameObject);
        }
        else if (collision.tag == "NextLevelDoor")
        {
            game.SendMessage("NextLevel");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Enemy" ||
                collision.gameObject.tag == "EnemyTear") && !isDamaged)
        {
            print("holaa22a");
            isDamaged = true;
            game.SendMessage("Damaged");
            IsDamaged();
        }
        else if(collision.gameObject.tag == "FullHearth" 
                && currentLives <= (maxLives - (halfHearth*2)))
        {
            currentLives += halfHearth*2;
            game.Healthed(2);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "HalfHearth" &&
                currentLives <= maxLives - halfHearth)
        {
            currentLives += halfHearth;
            game.Healthed(1);
            Destroy(collision.gameObject);
        }
    }

}

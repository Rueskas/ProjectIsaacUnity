using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float shotSpeed = 180f;
    [SerializeField] private float timeShot = 1;
    [SerializeField] private float lastShotTime = 0;
    [SerializeField] private float timeBeweenShots = 0.5f;
    [SerializeField] private float maxLives;
    [SerializeField] private float currentLives;
    [SerializeField] private float halfHearth = 0.5f;
    [SerializeField] private GameObject tearShot;
    [SerializeField] private GameObject body;
    [SerializeField] private GameObject head;
    private Animator animatorBody;
    private Animator animatorHead;
    private SpriteRenderer spriteBody;
    private SpriteRenderer spriteHead;
    private bool isDamaged = false;
    private bool isAlive = true;
    private bool hasTreasureRoomKey = false;
    private enum Direction { ToLeft, ToRight, ToUp, ToDown, Idle};
    private Direction direction;

    void Start()
    {
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

        if (isAlive)
        {
            transform.Translate(horizontal * Time.deltaTime * speed, vertical * Time.deltaTime * speed, 0);
            if (!isDamaged)
            {
                Animate();
            }

            InputShot();
        }
        
    }

    private void InputShot()
    {
        if(Time.time - lastShotTime > timeBeweenShots)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, shotSpeed));
                tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                Destroy(tear, timeShot);
                lastShotTime = Time.time;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(-shotSpeed, 0));
                tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                lastShotTime = Time.time;
                spriteHead.flipX = true;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -shotSpeed));
                tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                lastShotTime = Time.time;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(shotSpeed, 0));
                tear.GetComponent<Tear>().Invoke("StartAnim", timeShot);
                lastShotTime = Time.time;
                spriteHead.flipX = false;
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

    public void IsDamaged()
    {
        currentLives -= halfHearth;
        if(currentLives != 0)
        {
            animatorHead.gameObject.SetActive(false);
            StartCoroutine("ChangeColorDamaged");
            animatorBody.SetBool("IsDamaged", isDamaged);
        }
        else
        {
            animatorHead.gameObject.SetActive(false);
            isAlive = false;
            animatorBody.SetBool("IsAlive", isAlive);
            animatorBody.Play("Death");
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
        else if(collision.tag == "SpawnZone")
        {
            Room room = collision.gameObject.GetComponentInParent<Room>();
            GameObject pointZero = room.GetPointZero();

            CameraController camera = FindObjectOfType<CameraController>();
            camera.Move(pointZero.transform);

            room.EnterFocus();
        }
        else if (collision.tag.Contains("Item"))
        {
            switch (collision.tag)
            {
                case "SpeedBallItem":
                    timeBeweenShots -= timeBeweenShots / 2;
                    Destroy(collision.gameObject);
                    break;
            }
        }
        else if (collision.tag == "TreasureRoomKey")
        {
            hasTreasureRoomKey = true;
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && !isDamaged)
        {
            isDamaged = true;
            FindObjectOfType<GameController>().Damaged();
            IsDamaged();
        }
        else if(collision.gameObject.tag == "FullHearth" 
                && currentLives <= (maxLives - (halfHearth*2)))
        {
            currentLives += halfHearth*2;
            FindObjectOfType<GameController>().Healthed(2);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "HalfHearth" &&
                currentLives <= maxLives - halfHearth)
        {
            currentLives += halfHearth;
            FindObjectOfType<GameController>().Healthed(1);
            Destroy(collision.gameObject);
        }
    }

}

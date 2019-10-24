using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float shotSpeed = 150f;
    [SerializeField] private float timeShot = 1;
    [SerializeField] private float lastShotTime = 0;
    [SerializeField] private float timeBeweenShots = 0.5f;
    [SerializeField] GameObject tearShot;


    // Update is called once per frame
    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        transform.Translate(horizontal * Time.deltaTime * speed, vertical * Time.deltaTime * speed, 0);

        InputShot();
        
    }

    private void InputShot()
    {
        if(Time.time - lastShotTime > timeBeweenShots)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, shotSpeed));
                Destroy(tear, timeShot);
                lastShotTime = Time.time;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(-shotSpeed, 0));
                Destroy(tear, timeShot);
                lastShotTime = Time.time;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -shotSpeed));
                Destroy(tear, timeShot);
                lastShotTime = Time.time;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
                tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(shotSpeed, 0));
                Destroy(tear, timeShot);
                lastShotTime = Time.time;
            }
        }
    }
}

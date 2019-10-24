using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float shotSpeed = 150f;
    [SerializeField] private float timeShot = 1;
    [SerializeField] GameObject tearShot;

    void Start()
    {
        
    }

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
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
            tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, shotSpeed));
            Destroy(tear, timeShot);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
            tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(-shotSpeed, 0));
            Destroy(tear, timeShot);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameObject tear = Instantiate(tearShot, transform.position, Quaternion.identity);
            tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -shotSpeed));
            Destroy(tear, timeShot);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameObject tear = Instantiate(tearShot, transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
            tear.GetComponent<Rigidbody2D>().AddForce(new Vector2(shotSpeed, 0));
            Destroy(tear, timeShot);
        }
    }
}

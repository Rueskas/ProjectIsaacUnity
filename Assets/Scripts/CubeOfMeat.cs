using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOfMeat : MonoBehaviour
{
    GameObject player;
    void Update()
    {
        if(player != null)
        {
            transform.RotateAround(
                player.transform.position, 
                player.transform.forward, 
                100 * Time.deltaTime);
        }
    }

    public void SetPlayer(GameObject player)
    {
        this.player = player;
    }
}

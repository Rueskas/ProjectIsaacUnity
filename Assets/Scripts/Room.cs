﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Bounds bounds;
    [SerializeField] GameObject[] nextRoomsPoints;
    [SerializeField] Door[] doorPoints;
    [SerializeField] private GameObject pointZero;
    private List<GameObject> itemsToDrop;
    Dictionary<string, GameObject> connectedDoorsRooms;
    private LevelController levelController;
    private Door originDoorInRoom = null;
    
    private int quantityEnemies = 0;
    private bool ready = false;
    private bool isFocused = false;

    void Awake()
    {
        itemsToDrop = new List<GameObject>();
        levelController = FindObjectOfType<LevelController>();
        bounds = transform.GetChild(0).GetComponent<BoxCollider2D>().bounds;
    }

    void Start()
    {
        connectedDoorsRooms = new Dictionary<string, GameObject>();
        for (int i = 0; i < doorPoints.Length; i++)
        {
            connectedDoorsRooms.Add(doorPoints[i].name, nextRoomsPoints[i]);
        }
        ready = true;
    }

    public Door GetOriginDoorInRoom()
    {
        return originDoorInRoom;
    }

    public Door[] GetDoorPoints()
    {
        return doorPoints;
    }

    public GameObject GetRoomPointFromDoorPoint(string doorPointName)
    {
        GameObject roomPoint;
        connectedDoorsRooms.TryGetValue(
            doorPointName, out roomPoint);
        return roomPoint;
    }

    public bool IsReady()
    {
        return ready;
    }
    public void InstantiateEnemies()
    {
        int numberEnemies = Random.Range(1, 5);
        GameObject enemyPrefab = levelController.GetRandomEnemy();
        for (int i = 0; i < numberEnemies; i++)
        {
            GameObject enemy = 
                Instantiate(enemyPrefab, RandomPointInBounds(), Quaternion.identity, transform);
            quantityEnemies++;
            enemy.SetActive(false);
        }
    }
    
    public void SetOriginDoor(string position)
    {
        switch (position)
        {
            case "Left":
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointRight").First();
                doorPoints = doorPoints.Where(d => d.name != "DoorPointRight").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointRight").ToArray();
                break;
            case "Down":
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointUp").First();
                doorPoints = doorPoints.Where(d => d.name != "DoorPointUp").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointUp").ToArray();
                break;
            case "Up":
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointDown").First();
                doorPoints = doorPoints.Where(d => d.name != "DoorPointDown").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointDown").ToArray();
                break;
            case "Right":
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointLeft").First();
                doorPoints = doorPoints.Where(d => d.name != "DoorPointLeft").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointLeft").ToArray();
                break;
        }
    }

    public void EnterFocus()
    {
        if(quantityEnemies > 0)
        {

            Transform[] allChildren = 
                GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if(child.tag == "Enemy")
                {
                    child.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            OpenDoors();
        }
    }

    public void EnemyDeath()
    {
        quantityEnemies--;
        if (quantityEnemies == 0)
        {
            foreach(GameObject itemToDrop in itemsToDrop)
            {
                Instantiate(itemToDrop, transform);
            }
            OpenDoors();
        }
    }

    public void OpenDoors()
    {
        foreach(Door door in doorPoints)
        {
            if (door.isActiveAndEnabled == true)
            {
                door.Open();
            }
        }
        print(originDoorInRoom);
        if (originDoorInRoom != null)
        {
            originDoorInRoom.GetComponent<Door>().Open();
        }
    }

    public Vector2 RandomPointInBounds()
    {
        Vector2 randomPosition = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
        return randomPosition;
    }

    public GameObject GetPointZero()
    {
        return pointZero;
    }

    public void SetIsFocused(bool isFocused)
    {
        this.isFocused = isFocused;
        if (isFocused)
        {
            EnterFocus();
        }
    }

    public void AddItemToDrop(GameObject itemToDrop)
    {
        this.itemsToDrop.Add(itemToDrop);
    }
    
}

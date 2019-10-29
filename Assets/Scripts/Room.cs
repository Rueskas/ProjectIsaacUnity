using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Bounds bounds;
    [SerializeField] GameObject[] nextRoomsPoints;
    [SerializeField] Door[] doorPoints;
    Dictionary<string, GameObject> connectedDoorsRooms;
    private LevelController levelController;
    private GameObject originDoorInRoom = null;
    private GameObject pointZero;
    private int quantityEnemies = 0;
    private bool ready = false;
    private bool isFocused = false;

    void Awake()
    {
        levelController = FindObjectOfType<LevelController>();
    }

    void Start()
    {
        bounds = GetComponentInChildren<BoxCollider2D>().bounds;
        connectedDoorsRooms = new Dictionary<string, GameObject>();
        for (int i = 0; i < doorPoints.Length; i++)
        {
            connectedDoorsRooms.Add(doorPoints[i].name, nextRoomsPoints[i]);
        }
        ready = true;
    }

    public GameObject GetOriginDoorInRoom()
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
    
    public void SetOriginDoor(string position, GameObject doorOutRoom)
    {
        switch (position)
        {
            case "Left":
                doorPoints = doorPoints.Where(d => d.name != "DoorPointRight").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointRight").ToArray();
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointLeft").First().gameObject;
                originDoorInRoom.GetComponent<Door>().SetType(Door.DoorType.Normal);
                break;
            case "Down":
                doorPoints = doorPoints.Where(d => d.name != "DoorPointUp").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointUp").ToArray();
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointDown").First().gameObject;
                originDoorInRoom.GetComponent<Door>().SetType(Door.DoorType.Normal);
                break;
            case "Up":
                doorPoints = doorPoints.Where(d => d.name != "DoorPointDown").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointDown").ToArray();
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointUp").First().gameObject;
                originDoorInRoom.GetComponent<Door>().SetType(Door.DoorType.Normal);
                break;
            case "Right":
                doorPoints = doorPoints.Where(d => d.name != "DoorPointLeft").ToArray();
                nextRoomsPoints = nextRoomsPoints.Where(r => r.name != "RoomPointLeft").ToArray();
                originDoorInRoom = doorPoints.Where(d => d.name == "DoorPointRight").First().gameObject;
                originDoorInRoom.GetComponent<Door>().SetType(Door.DoorType.Normal);
                break;
        }
    }

    public void EnterFocus()
    {
        if(quantityEnemies > 0)
        {
            foreach(GameObject child in transform)
            {
                if(child.tag == "Enemy")
                {
                    child.SetActive(true);
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
    }

    public Vector2 RandomPointInBounds()
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
        );
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
    
}

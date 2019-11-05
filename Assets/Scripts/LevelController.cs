using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject[] roomsPrefabs;
    [SerializeField] private GameObject[] enemiesPrefabs;
    [SerializeField] private GameObject[] bossPrefabs;
    [SerializeField] private GameObject[] itemsToDropPrefabs;
    [SerializeField] private GameObject[] treasureItemsPrefabs;
    [SerializeField] private GameObject keyTreasureRoomPrefab;
    private int maxRooms;
    private int currentRooms;
    private bool needKeyTreasure = false;
    private int doorNumberToDropKey;
    private List<Room> rooms;
    private GameController game;
    public static float offsetBetweenDoorsY = 3.25f;
    public static float offsetBetweenDoorsX = -3.60f;
    void Start()
    {
        rooms = new List<Room>();
        game = FindObjectOfType<GameController>();
        currentRooms = 0;
        int level = game.GetLevel();
        maxRooms = System.Convert.ToInt32(System.Math.Sqrt(level) * 7);
        if (level > 1)
        {
            needKeyTreasure = true;
            doorNumberToDropKey = Random.Range(1, maxRooms-1);
        }
        GameObject startRoom = Instantiate(
            roomsPrefabs[0], transform.position, Quaternion.identity);
        StartCoroutine("Generate", startRoom);
        StartCoroutine("StartGame", startRoom);
    }

    IEnumerator Generate(GameObject roomObject)
    {
        rooms.Add(roomObject.GetComponent<Room>());
        int numberOfRooms, maxNumberOfRooms;
        int minNumberOfRooms = 1;
        Room room = roomObject.GetComponent<Room>();
        if(currentRooms == 0)
        {
            maxNumberOfRooms = 4;
        }
        else
        {
            room.InstantiateEnemies();
            if(maxRooms - currentRooms > 3)
            {
                maxNumberOfRooms = 3;
            }
            else
            {
                maxNumberOfRooms = maxRooms - currentRooms;
            }
            yield return new WaitWhile(() => 
            room.GetOriginDoorInRoom() == null);
        }
        if (needKeyTreasure && currentRooms > doorNumberToDropKey)
        {
            room.AddItemToDrop(keyTreasureRoomPrefab);
            needKeyTreasure = false;
        }
        minNumberOfRooms =
            (maxRooms - currentRooms > 0) ? 1 : 0;
        numberOfRooms = Random.Range(minNumberOfRooms, maxNumberOfRooms);


        Door[] shuffledDoorPoints = room.GetDoorPoints()
            .OrderBy(doorPoint => Random.value).ToArray();

        currentRooms += numberOfRooms;
        GameObject roomPointToInstantiate;
        float maxDistance = 4f;
        for (int i = 0; i < shuffledDoorPoints.Length; i++)
        {
            shuffledDoorPoints[i].SetType(Door.DoorType.Normal);
            if (i < numberOfRooms)
            {
                yield return new WaitUntil(() => room.IsReady());
                roomPointToInstantiate = 
                    room.GetRoomPointFromDoorPoint(shuffledDoorPoints[i].name);

                Vector2 originRay = shuffledDoorPoints[i].transform.position;
                Vector2 directionRay =
                    roomPointToInstantiate.transform.position - 
                        shuffledDoorPoints[i].transform.position;
                RaycastHit2D[] hits = new RaycastHit2D[3];

                int touchs =
                    shuffledDoorPoints[i].GetComponent<Collider2D>().Raycast(
                        directionRay, hits, maxDistance);

                if (touchs <= 1)
                {
                    if (roomPointToInstantiate != null)
                    {
                        string position = 
                            shuffledDoorPoints[i].gameObject.
                                name.Replace("DoorPoint", "");

                        GameObject newRoom = Instantiate(GetNormalRoom(),
                            roomPointToInstantiate.transform.position, 
                                Quaternion.identity);
                        newRoom.GetComponent<Room>().SetOriginDoor(position);
                        newRoom.GetComponent<Room>().GetOriginDoorInRoom()
                            .SetType(Door.DoorType.Normal);

                        int setItem = Random.Range(-2, 2);
                        if(setItem > 0)
                        {
                            int indexItem = 
                                Random.Range(0, itemsToDropPrefabs.Length);
                            newRoom.GetComponent<Room>().
                                AddItemToDrop(itemsToDropPrefabs[indexItem]);
                        }
                        StartCoroutine("Generate", newRoom);
                    }
                }
                else
                {
                    currentRooms--;
                    shuffledDoorPoints[i].gameObject.SetActive(false);
                }
            }
            else
            {
                shuffledDoorPoints[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator StartGame(GameObject startRoom)
    {
        Player player = FindObjectOfType<Player>();
        player.gameObject.SetActive(false);
        yield return new WaitWhile(() => currentRooms < maxRooms);

        GenerateBossRoom();
        GenerateTreasureRoom();
        game.StartCoroutine("FadeOutWaitingStartImage");
        yield return new WaitUntil(() => game.GetStartedLevel() == true);
        player.gameObject.SetActive(true);
        player.SetActiveCollider(true);
        startRoom.GetComponent<Room>().SetIsFocused(true);
    }

    public void GenerateTreasureRoom()
    {
        List<Room> shuffledRooms = 
            rooms.OrderBy(room => Random.value).ToList();
        int count = 0;
        bool treasureRoomInstantiated = false;
        float maxDistance = 4f;
        do
        {
            Room room = shuffledRooms[count];
            Door[] doorPoints = room.GetDoorPoints();
            foreach (Door door in doorPoints)
            {
                if (door.isActiveAndEnabled == false && 
                        treasureRoomInstantiated == false)
                {

                    GameObject roomPointToInstantiate =
                        room.GetRoomPointFromDoorPoint(door.name);

                    Vector2 originRay = door.transform.position;
                    Vector2 directionRay =
                        roomPointToInstantiate.transform.position -
                            door.transform.position;
                    RaycastHit2D[] hits = new RaycastHit2D[3];

                    int touchs =
                       door.GetComponent<Collider2D>().Raycast(
                            directionRay, hits, maxDistance);

                    if (touchs <= 1)
                    {
                        door.gameObject.SetActive(true);
                        door.SetType(Door.DoorType.Treasure);
                        if (roomPointToInstantiate != null)
                        {
                            string position =
                                door.gameObject.name.Replace("DoorPoint", "");

                            GameObject newRoom = Instantiate(GetTreasureRoom(),
                                roomPointToInstantiate.transform.position,
                                    Quaternion.identity);
                            newRoom.GetComponent<Room>().SetOriginDoor(position);
                            newRoom.GetComponent<Room>().GetOriginDoorInRoom()
                                .SetType(Door.DoorType.Treasure);
                            Door[] doorPointsOfNewRoom =
                                newRoom.GetComponent<Room>().GetDoorPoints();
                            foreach (Door newDoor in doorPointsOfNewRoom)
                            {
                                newDoor.gameObject.SetActive(false);
                            }
                            int treasureIndex =
                                Random.Range(0, treasureItemsPrefabs.Length);
                            GameObject treasureItem =
                                treasureItemsPrefabs[treasureIndex];
                            Instantiate(treasureItem, 
                                newRoom.transform.position, Quaternion.identity);
                            treasureRoomInstantiated = true;
                        }
                    }
                }
            }
            count++;
        } while (treasureRoomInstantiated == false);
    }

    public void GenerateBossRoom()
    {
        List<Room> shuffledRooms =
            rooms.OrderBy(room => Random.value).ToList();
        int count = 0;
        bool bossRoomInstantiated = false;
        float maxDistance = 4f;
        do
        {
            Room room = shuffledRooms[count];
            Door[] doorPoints = room.GetDoorPoints();
            foreach (Door door in doorPoints)
            {
                if (door.isActiveAndEnabled == false &&
                        bossRoomInstantiated == false)
                {
                    GameObject roomPointToInstantiate =
                        room.GetRoomPointFromDoorPoint(door.name);

                    Vector2 originRay = door.transform.position;
                    Vector2 directionRay =
                        roomPointToInstantiate.transform.position -
                            door.transform.position;
                    RaycastHit2D[] hits = new RaycastHit2D[3];

                    int touchs =
                       door.GetComponent<Collider2D>().Raycast(
                            directionRay, hits, maxDistance);

                    if (touchs <= 1)
                    {
                        door.gameObject.SetActive(true);
                        door.SetType(Door.DoorType.Boss);
                        if (roomPointToInstantiate != null)
                        {
                            string position =
                                door.gameObject.name.Replace("DoorPoint", "");

                            GameObject newRoom = Instantiate(GetBossRoom(),
                                roomPointToInstantiate.transform.position,
                                    Quaternion.identity);
                            newRoom.GetComponent<Room>().SetOriginDoor(position);
                            newRoom.GetComponent<Room>().GetOriginDoorInRoom()
                                .SetType(Door.DoorType.Boss);
                            Door[] doorPointsOfNewRoom =
                                newRoom.GetComponent<Room>().GetDoorPoints();
                            GameObject boss = 
                                Instantiate(GetRandomBoss(), 
                                    newRoom.transform.position,
                                        Quaternion.identity, 
                                            newRoom.transform);
                            boss.SetActive(false);
                            int treasureIndex =
                                Random.Range(0, treasureItemsPrefabs.Length);
                            GameObject treasureItem =
                                treasureItemsPrefabs[treasureIndex];
                            newRoom.GetComponent<Room>().
                                AddItemToDrop(treasureItem);
                            foreach (Door newDoor in doorPointsOfNewRoom)
                            {
                                newDoor.gameObject.SetActive(false);
                            }
                            bossRoomInstantiated = true;
                        }
                    }
                }
            }
            count++;
        } while (bossRoomInstantiated == false);


    }

    public GameObject GetNormalRoom()
    {
        return roomsPrefabs[0];
    }

    public GameObject GetTreasureRoom()
    {
        return roomsPrefabs[2];
    }

    public GameObject GetBossRoom()
    {
        return roomsPrefabs[1];
    }

    public GameObject GetRandomEnemy()
    {
        return enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)];
    }

    public GameObject GetRandomBoss()
    {
        return bossPrefabs[1];
        return bossPrefabs[Random.Range(0, bossPrefabs.Length)];
    }

}

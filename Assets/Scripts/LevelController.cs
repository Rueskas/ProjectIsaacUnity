using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject[] roomsPrefabs;
    [SerializeField] private GameObject[] enemiesPrefabs;
    private int maxRooms;
    private int currentRooms;
    private List<Room> rooms;
    private GameController game;
    public static float offsetBetweenDoorsY = 3f;
    public static float offsetBetweenDoorsX = 3f;
    public static float offsetBetweenCameraPointsY = 3.5f;
    public static float offsetBetweenCameraPointsX = 3.5f;
    void Start()
    {
        rooms = new List<Room>();
        game = FindObjectOfType<GameController>();
        currentRooms = 0;
        int level = game.GetLevel();
        maxRooms = System.Convert.ToInt32(System.Math.Sqrt(level)*5);
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
                        newRoom.GetComponent<Room>()
                            .SetOriginDoor(position, 
                                shuffledDoorPoints[i].gameObject);
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
        yield return new WaitWhile(() => currentRooms != maxRooms);
        startRoom.GetComponent<Room>().SetIsFocused(true);
    }

    public GameObject GetNormalRoom()
    {
        return roomsPrefabs[0];
    }

    public GameObject GetTreasureRoom()
    {
        return roomsPrefabs[1];
    }

    public GameObject GetBossRoom()
    {
        return roomsPrefabs[2];
    }

    public GameObject GetRandomEnemy()
    {
        return enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)];
    }

}

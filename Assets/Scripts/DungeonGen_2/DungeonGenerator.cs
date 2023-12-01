using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    public class Cell
    {
        public bool visited = false;
        public bool[] status = new bool[4];
    }

    [System.Serializable]
    public struct Room
    {
        public GameObject room;
        public GameObject startRoom;
        public GameObject endRoom;
    }

    public Vector2 size;
    public int startPos = 0;
    public List<Room> room = new List<Room>();
    public Vector2 offset;
    public Vector3 spawnLocation;

    public TextMeshProUGUI text;
    public GameObject transitionImage;

    ScoreManager sm;

    List<Cell> board;
    List<RoomBehaviour> rooms = new List<RoomBehaviour>();

    void Start()
    {
        sm = FindObjectOfType<ScoreManager>();
        text.text = PlayerPrefs.GetInt("Score").ToString();
        MazeGenerator();
    }

    void Update()
    {
        
    }

    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[Mathf.FloorToInt(i + j * size.x)];

                if (currentCell.visited)
                {
                    RoomBehaviour newRoom;
                    int index = Random.Range(0, room.Count);

                    if(i == 0 && j == 0)
                    {
                        newRoom =
                            Instantiate(room[index].startRoom, new Vector3(i * offset.x, 0, -j * offset.y),
                            Quaternion.identity, transform).GetComponent<RoomBehaviour>();

                        spawnLocation = newRoom.transform.GetChild(0).position;
                    }
                    else if(i == size.x - 1 && j == size.y - 1)
                    {
                        newRoom =
                            Instantiate(room[index].endRoom, new Vector3(i * offset.x, 0, -j * offset.y),
                            Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    }
                    else
                    {
                        newRoom =
                            Instantiate(room[index].room, new Vector3(i * offset.x, 0, -j * offset.y),
                            Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    }

                    newRoom.UpdateRoom(currentCell.status);

                    newRoom.name += " " + i + "-" + j;
                    rooms.Add(newRoom);
                }

            }
        }
    }

    public void MazeGenerator()
    {
        board = new List<Cell>();

        for(int i = 0; i < size.x; i++)
        {
            for(int j = 0; j < size.y; j++)
            {
                board.Add(new Cell());
            }
        }

        int currentCell = startPos;

        Stack<int> path = new Stack<int>();

        int k = 0;

        while(k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if (currentCell == board.Count - 1)
                break;

            // Check cell's 
            List<int> neighbours = CheckNeighbours(currentCell);

            if(neighbours.Count == 0)
            {
                if(path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbours[Random.Range(0, neighbours.Count)];

                if (newCell > currentCell)
                {
                    if(newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true;
                        currentCell = newCell;
                        board[currentCell].status[3] = true;
                    }
                    else
                    {
                        board[currentCell].status[1] = true;
                        currentCell = newCell;
                        board[currentCell].status[0] = true;
                    }
                }
                else
                {
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true;
                        currentCell = newCell;
                        board[currentCell].status[2] = true;
                    }
                    else
                    {
                        board[currentCell].status[0] = true;
                        currentCell = newCell;
                        board[currentCell].status[1] = true;
                    }
                }
            }
        }

        GenerateDungeon();
    }

    List<int> CheckNeighbours(int cell)
    {
        List<int> neighbours = new List<int>();


        if(cell - size.x >= 0 && !board[Mathf.FloorToInt(cell-size.x)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell - size.x));
        }

        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell + size.x));
        }

        if ((cell + 1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell + 1));
        }

        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited)
        {
            neighbours.Add(Mathf.FloorToInt(cell - 1));
        }


        return neighbours;
    }

    public void Clear()
    {
        sm.AddScore();
        text.text = PlayerPrefs.GetInt("Score").ToString();


        foreach (var room in rooms)
        {
            room.DestroyRoom();
        }

        rooms.Clear();
    }
}

using UnityEngine;


public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static DungeonGenerator generator;
    public static ThirdPersonController player;

    private void Start()
    {
        instance = this;
        generator = FindObjectOfType<DungeonGenerator>();
        player = FindObjectOfType<ThirdPersonController>();
    }

    public static void NewLevel()
    {
        generator.Clear();
        generator.MazeGenerator();
        player.gameObject.SetActive(false);
        player.transform.position = generator.spawnLocation;
        player.gameObject.SetActive(true);
    }
}

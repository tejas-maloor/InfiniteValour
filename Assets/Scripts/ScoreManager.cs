using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    int floors = 0;

    private void Start()
    {
        PlayerPrefs.SetInt("Score", floors);
    }

    public void AddScore()
    {
        floors++;
        PlayerPrefs.SetInt("Score", floors);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpdate : MonoBehaviour
{
    void DoUpdate()
    {
        LevelManager.NewLevel();
    }
}

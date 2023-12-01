using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    TextMeshProUGUI text;
    ScoreManager sm;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        text = GetComponent<TextMeshProUGUI>();
        text.text = PlayerPrefs.GetInt("Score").ToString();
    }
}

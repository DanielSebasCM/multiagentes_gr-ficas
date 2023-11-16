using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Schedule : MonoBehaviour
{
    public GameObject boss;

    public GameObject enemy;

    public PlayerController player;

    public GUIStyle endSkin;

    public GUIStyle healthSkin;

    int enemyCount = 0;

    bool showWin = false;
    // Start is called before the first frame update
    void Start()
    {
        endSkin.fontSize = 80;
        endSkin.fontStyle = FontStyle.Bold;
        endSkin.normal.textColor = Color.blue;

        healthSkin.fontSize = 40;
        healthSkin.normal.textColor = Color.blue;

        player = FindAnyObjectByType<PlayerController>();
        StartCoroutine(Run());
    }

    void OnGUI()
    {
        if (showWin)
        {
            endSkin.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "You WIN!!", endSkin);
        }

        GUI.Label(new Rect(10, Screen.height - 60, 100, 50), "Enemy Count:" + (enemyCount), healthSkin);
    }

    IEnumerator Run()
    {
        Quaternion rotation = Quaternion.Euler(0, -90, 0);
        Vector3 basePosition = new(30, -70, -135);
        Vector3 positionDiff = new(0, 0, 15);

        List<GameObject> enemies = new()
        {
            Instantiate(enemy, basePosition + positionDiff, rotation),
            Instantiate(enemy, basePosition, rotation),
            Instantiate(enemy, basePosition - positionDiff, rotation)
        };

        // wait for all to be destroyed
        while (enemies.Count > 0)
        {
            enemies.RemoveAll(item => item == null);
            enemyCount = enemies.Count;
            yield return null;
        }

        GameObject bossInstance = Instantiate(boss, basePosition, rotation);
        enemyCount += 1;
        while (bossInstance != null)
        {
            yield return null;
        }
        enemyCount -= 1;

        if (player.alive)
        {
            showWin = true;
        }
    }

}

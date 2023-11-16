using UnityEngine;

public class HealthController : MonoBehaviour
{
    // textures
    // public Texture2D healthBackground; // back segment
    // public Texture2D healthForeground; // front segment
    public GUIStyle healthSkin;
    public GUIStyle endSkin;

    public float curHP; // current HP
    public float maxHP = 3; // maximum HP

    public int posX = 10;
    public int posY = 10;
    public int height = 15;

    void Start()
    {
        curHP = maxHP;
        healthSkin.fontSize = 40;
        healthSkin.normal.textColor = Color.green;

        endSkin.fontSize = 80;
        endSkin.fontStyle = FontStyle.Bold;
        endSkin.normal.textColor = Color.red;
    }

    void Update()
    {
        adjustCurrentHealth(0);
    }

    public void adjustCurrentHealth(float value)
    {

        /**Deduct the current health value from its damage**/
        curHP += value;
        if (curHP > maxHP)
        {
            curHP = maxHP;
        }

        if (curHP <= 0)
        {
            // show you died on the center of the screen
            curHP = 0;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(30, 28, 100, 50), curHP.ToString() + "/" + maxHP.ToString(), healthSkin);
        if (curHP <= 0)
        {
            endSkin.alignment =  TextAnchor.MiddleCenter;
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "You Died", endSkin);
        }
    }
}


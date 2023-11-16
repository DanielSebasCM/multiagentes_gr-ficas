using UnityEngine;
using TMPro;

public class BulletCounter : MonoBehaviour
{
    // Start is called before the first frame update
    public int bulletCount = 0;
    public TextMeshProUGUI text;

    // Update is called once per frame
    void Update()
    {
        text.text = "Bullets: " + bulletCount;        
    }
}

using UnityEngine;

public class Bullet : MonoBehaviour
{
    public BulletCounter bulletCounter;

    void Awake()
    {
        bulletCounter = FindAnyObjectByType<BulletCounter>();
    }
    void Start()
    {
        bulletCounter.bulletCount++;
        Destroy(gameObject, 10f);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        bulletCounter.bulletCount--;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.SendMessage("adjustHealh", -1f);
            Destroy(gameObject);
        }
    }
}

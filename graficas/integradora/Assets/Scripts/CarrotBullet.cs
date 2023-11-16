using UnityEngine;

public class CarrotBullet : MonoBehaviour
{
    public BulletCounter bulletCounter;

    void Awake()
    {
        bulletCounter = FindAnyObjectByType<BulletCounter>();
    }
    void Start()
    {
        bulletCounter.bulletCount++;
        Destroy(gameObject, 20f);
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
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}

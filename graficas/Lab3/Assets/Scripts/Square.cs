using System.Collections;
using UnityEngine;

public class Square : MonoBehaviour
{

    // Start is called before the first frame update
    public void OnEnable()
    {
        TimeManager.OnMinuteChanged += TimeCheck;
    }

    public void OnDisable()
    {
        TimeManager.OnMinuteChanged -= TimeCheck;
    }
    private void TimeCheck()
    {
        if (TimeManager.Minute % 10 == 0)
        {
            StartCoroutine(MoveSquare());
        }

    }

    private IEnumerator MoveSquare()
    {
    
        transform.position = new(-100f, 100f, 0);
        Vector3 targetPos = new(900f, 100f, 0);

        Vector3 currentPos = transform.position;

        float timeElapsed = 0;
        float timeToMove = 5;

        while (timeElapsed < timeToMove)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}

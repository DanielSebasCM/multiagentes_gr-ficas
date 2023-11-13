using System.Collections;
using UnityEngine;

public class Triangle : MonoBehaviour
{
    public Square square;
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
        if (TimeManager.Minute % 3 == 0)
        {
            StartCoroutine(Jump());
        }

    }

    private void Update()
    {
        transform.position = new Vector3(square.transform.position.x, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    private IEnumerator Jump()
    {

        float yStart = transform.position.y;
        float yEnd = yStart + 100f;

        float startAngle = transform.rotation.eulerAngles.z;
        float rotationAmount = 120f;
        float timeElapsed = 0;
        float timeToMove = 0.5f;

        while (timeElapsed < timeToMove)
        {
            transform.SetPositionAndRotation(
                new Vector3(
                    transform.position.x,
                    Mathf.Lerp(yStart, yEnd, timeElapsed / timeToMove),
                    transform.position.z),
                Quaternion.Euler(
                    0,
                    0,
                    Mathf.Lerp(
                        startAngle,
                        startAngle + rotationAmount / 2,
                        timeElapsed / timeToMove) % 360));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.SetPositionAndRotation(
            new Vector3(
                transform.position.x,
                yEnd,
                transform.position.z),
            Quaternion.Euler(
                0,
                0,
                (startAngle + rotationAmount / 2) % 360
            ));

        timeElapsed = 0;
        while (timeElapsed < timeToMove)
        {
            transform.SetPositionAndRotation(
                new Vector3(
                    transform.position.x,
                    Mathf.Lerp(yEnd, yStart, timeElapsed / timeToMove),
                    transform.position.z),
                Quaternion.Euler(
                    0, 
                    0, 
                    Mathf.Lerp(
                        startAngle + rotationAmount / 2, 
                        startAngle + rotationAmount, 
                        timeElapsed / timeToMove) % 360));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.SetPositionAndRotation(
            new Vector3(
                transform.position.x,
                yStart,
                transform.position.z),
            Quaternion.Euler(
                0,
                0,
                (startAngle + rotationAmount) % 360
            ));
    }
}

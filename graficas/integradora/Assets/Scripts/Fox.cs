using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


enum State
{
    Enter,
    Attack,
    Die
}

public class Fox : MonoBehaviour
{
    [SerializeField]
    public GameObject bulletPrefab;

    public Animator myAnim;

    readonly float timeBetween = 10f;

    readonly float timeRest = 2f;

    List<Func<IEnumerator>> attacks;

    List<string> animations;

    int currentAttack;

    private Coroutine attackCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
        attacks = new List<Func<IEnumerator>> { Swirl, Waves, Swirl };
        animations = new List<string> { "Fox_Attack_Paws", "Fox_Attack_Tail", "Fox_Attack_Paws" };
        currentAttack = 0;

        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return StartCoroutine(EnterCoroutine());
        yield return StartCoroutine(AttackCoroutine());
        yield return StartCoroutine(DieCoroutine());
    }
    IEnumerator EnterCoroutine()
    {
        myAnim.Play("Fox_Run_InPlace");
        float timeToEnter = 3f;
        float timeElapsed = 0f;
        Vector3 initialPosition = transform.position;
        Vector3 finalPosition = initialPosition;
        finalPosition.x = -25;
        while (timeElapsed < timeToEnter)
        {
            transform.position = Vector3.Lerp(
                initialPosition,
                finalPosition,
                timeElapsed / timeToEnter
            );
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator AttackCoroutine()
    {
        SetAttack(0);
        while (true)
        {
            yield return new WaitForSeconds(timeBetween);
            StopAttack();
            yield return new WaitForSeconds(timeRest);
            int newAttack = currentAttack + 1;
            if (newAttack >= attacks.Count) break;
            SetAttack(newAttack);
        }
        StopAttack();
    }

    IEnumerator DieCoroutine()
    {
        myAnim.Play("Fox_Falling_Left");
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }


    void SetAttack(int attack)
    {
        currentAttack = attack;
        StopAttack();
        attackCoroutine = StartCoroutine(attacks[currentAttack]());
    }

    void StopAttack()
    {
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
    }

    IEnumerator Waves()
    {
        return Shoot(30, 8f, 1f, 5);
    }

    IEnumerator Swirl()
    {
        return Shoot(3, 8f, 0.2f, 43f);
    }

    private IEnumerator Shoot(
        int spikes,
        float speed,
        float delay,
        float rotationSpeed = 0,
        Func<float, float> calcAddedSpeed = null
        )
    {
        float rotationStep = rotationSpeed * delay;
        int counter = 0;
        Debug.Log(calcAddedSpeed);
        while (true)
        {
            myAnim.Play(animations[currentAttack]);
            for (int i = 0; i < spikes; i++)
            {
                float baseAngle = 360f / spikes * i;
                float spintAngle = counter * rotationStep;
                Vector3 position = transform.position;
                position.y += 0.2f;
                Quaternion rotation = Quaternion.Euler(90, 0, baseAngle + spintAngle);
                GameObject bullet = Instantiate(bulletPrefab, position, rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                float addedSpeed = 0;
                if (calcAddedSpeed != null)
                {
                    addedSpeed = calcAddedSpeed(baseAngle);
                }
                rb.velocity = bullet.transform.up * (speed + addedSpeed);
            }
            counter++;
            yield return new WaitForSeconds(delay);
        }
    }
}

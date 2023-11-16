using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // <value> This is the speed of the vehicle </value>
    public float speed = 10.0f;

    public GameObject carrotBullet;

    public float speedMult = 1f;

    public Animator myAnim;

    public bool alive = true;

    public HealthController healthController;
    // <value> This is the turning speed of the vehicle </value>
    public float horizontalInput;
    public float forwardInput;

    private float elapsedTimeSinceShoot = 0f;
    private float delayBetweenShoots = 5f;


    void Start()
    {
        myAnim = GetComponent<Animator>();
        myAnim.Play("Idle");
    }

    public void adjustHealh(float value)
    {
        healthController.adjustCurrentHealth(value);
    }

    /// <summary>
    /// This method is called once per frame
    /// </summary>
    void Update()
    {
        if (!alive) return;

        elapsedTimeSinceShoot += Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
            if (delayBetweenShoots < elapsedTimeSinceShoot)
                Shoot();


        if (horizontalInput != 0 || forwardInput != 0)
            myAnim.Play("Run 1");

        horizontalInput = Input.GetAxis("Horizontal");
        forwardInput = Input.GetAxis("Vertical");

        speedMult = Input.GetKey(KeyCode.LeftShift) ? 0.5f : 1f;

        transform.Translate(
            forwardInput * speed * speedMult * Time.deltaTime * Vector3.left +
            horizontalInput * speed * speedMult * Time.deltaTime * Vector3.forward
        );

        if (healthController.curHP <= 0)
        {
            alive = false;
            myAnim.Play("Dead");
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<CapsuleCollider>());
        }
    }


    void Shoot()
    {
        elapsedTimeSinceShoot = 0f;
        Vector3 position = transform.position;
        position.x += 1;
        GameObject bullet = Instantiate(carrotBullet, position, Quaternion.Euler(90, -90, 0));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.right * 10;
    }
}

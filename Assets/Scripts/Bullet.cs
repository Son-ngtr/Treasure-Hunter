using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float timeToDestroyBullet = 2f;
    float xSpeed;
    Rigidbody2D myRigitbody;

    // the scripts must be unique
    PlayerMovementScripts player;

    void Start()
    {
        myRigitbody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovementScripts>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
    }

    void Update()
    {
        myRigitbody.velocity = new Vector2(xSpeed, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Destroy(collision.gameObject);
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject, timeToDestroyBullet);
    }
}

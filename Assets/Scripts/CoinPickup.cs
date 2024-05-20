using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSfx;
    [SerializeField] int pointsForCoinPickup = 100;

    bool wasCollected = false;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameManagement>().AddToScore(pointsForCoinPickup);
            AudioSource.PlayClipAtPoint(coinPickupSfx, Camera.main.transform.position);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

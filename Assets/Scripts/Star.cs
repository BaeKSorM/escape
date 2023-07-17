using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject star in GameManager.instance.stars)
            {
                if (!star.activeSelf)
                {
                    star.SetActive(true);
                    gameObject.SetActive(false);
                    break;
                }
            }
        }
    }
}

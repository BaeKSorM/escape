using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowButton : MonoBehaviour
{
    [SerializeField] internal GameObject hidenGround;
    [SerializeField] internal GameObject obstacle;
    [SerializeField] internal bool rare;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box") && rare)
        {
            obstacle.SetActive(true);
        }
        else if (other.CompareTag("Box"))
        {
            StartCoroutine(ShowHG());
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Box") && rare)
        {
            obstacle.SetActive(false);
        }
        else if (other.CompareTag("Box"))
        {
            hidenGround.SetActive(false);
        }
    }
    IEnumerator ShowHG()
    {
        hidenGround.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        hidenGround.SetActive(false);
    }
}

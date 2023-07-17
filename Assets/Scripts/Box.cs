using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] internal Vector2 resetPos;
    void Start()
    {
        resetPos = transform.position;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("DeadZone"))
        {
            transform.position = resetPos;
            transform.rotation = Quaternion.Euler(Vector2.zero);
        }
    }
}

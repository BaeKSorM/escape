using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform Player;
    [SerializeField] private float startPos;
    [SerializeField] private float endPos;
    [SerializeField] private Vector3 movePos;
    void Update()
    {
        if (Player.position.x < startPos)
        {
            movePos = new Vector3(startPos, transform.position.y, -10);
            transform.position = Vector3.Lerp(transform.position, movePos, 0.2f);
        }
        else if (Player.position.x > endPos)
        {
            movePos = new Vector3(endPos, transform.position.y, -10);
            transform.position = Vector3.Lerp(transform.position, movePos, 0.2f);
        }
        else
        {
            movePos = new Vector3(Player.position.x, transform.position.y, -10);
            transform.position = Vector3.Lerp(transform.position, movePos, 0.2f);
        }
    }
}

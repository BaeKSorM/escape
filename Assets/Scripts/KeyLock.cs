using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLock : MonoBehaviour
{
    public static KeyLock instance;
    [SerializeField] internal int needKeyCount;
    [SerializeField] internal int throwPower;
    [SerializeField] internal bool touched;
    [SerializeField] internal bool breaking;
    [SerializeField] internal bool deleting;
    [SerializeField] internal Rigidbody2D lockRB;
    [SerializeField]
    internal Rigidbody2D[] delTiles;
    void Awake()
    {
        instance = this;
        // delTiles = transform.GetComponentsInChildren<Rigidbody2D>();
    }
    void Update()
    {
        if (breaking)
        {
            breaking = false;
            BreakLock();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            touched = true;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            touched = false;
        }
    }
    void BreakLock()
    {
        lockRB.bodyType = RigidbodyType2D.Dynamic;
        lockRB.AddForce(new Vector2(-transform.localScale.x * throwPower, 1 * throwPower), ForceMode2D.Impulse);
        StartCoroutine(RotateBrokenTile(lockRB, -transform.localScale.x));
        StartCoroutine(DeleteTiles(delTiles));
    }

    IEnumerator RotateBrokenTile(Rigidbody2D lockRB, float LR)
    {
        while (true)
        {
            lockRB.transform.Rotate(Vector3.forward, 5 * LR);
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator DeleteTiles(Rigidbody2D[] tiles)
    {
        deleting = true;
        float alpha = 1;
        while (tiles[0].GetComponent<SpriteRenderer>().color.a > 0)
        {
            foreach (var tile in tiles)
            {
                tile.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, alpha);
            }
            yield return new WaitForSeconds(0.01f);
            alpha -= 0.1f;
        }
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}

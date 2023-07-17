using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] internal int keyCount = 0;
    [SerializeField] internal GameObject[] keys;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] boxes;
    [SerializeField] private GameObject[] unLocks;
    [SerializeField] internal GameObject[] stars;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] internal GameObject jumpButton;
    [SerializeField] internal GameObject unLockButton;
    [SerializeField] internal bool start;
    [SerializeField] internal GameObject startBG;
    [SerializeField] internal GameObject UI;
    [SerializeField] internal AudioSource audioSource;
    void Awake()
    {
        instance = this;
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void GetKey()
    {
        foreach (GameObject key in keys)
        {
            if (key.GetComponent<Key>().touched)
            {
                keyText.text = "x" + ++keyCount;
                key.SetActive(false);
            }
        }
    }
    public void GameStart()
    {
        start = true;
        startBG.SetActive(false);
        UI.SetActive(true);
        audioSource.Play();
    }
    public void UnLock()
    {
        foreach (GameObject unLock in unLocks)
        {
            KeyLock keyLock = unLock.GetComponent<KeyLock>();
            if (keyLock.touched && !keyLock.deleting && keyCount >= keyLock.needKeyCount)
            {
                keyCount -= keyLock.needKeyCount;
                keyLock.breaking = true;
                break;
            }
        }
        keyText.text = "x" + keyCount;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            player.transform.position = player.GetComponent<PlayerController>().checkPoint;
            foreach (var box in boxes)
            {
                box.transform.position = box.GetComponent<Box>().resetPos;
                box.transform.rotation = Quaternion.Euler(Vector2.zero);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}

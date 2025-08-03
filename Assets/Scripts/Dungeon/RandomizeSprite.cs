using NUnit.Framework;
using UnityEngine;

public class RandomizeSprite : MonoBehaviour
{
    [SerializeField]
    private Sprite[] spriteList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int spriteIndex = Random.Range(0, spriteList.Length);
        GetComponent<SpriteRenderer>().sprite = spriteList[spriteIndex];
    }
}

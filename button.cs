using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour
{
    public Sprite btn;
    public GameObject[] Objects;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "boxMark")
        {
            GetComponent<SpriteRenderer>().sprite = btn;
            GetComponent<CircleCollider2D>().enabled = false;

            foreach (GameObject obj in Objects)
            {
                Destroy(obj);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float speed = 3f;
    public float TimeToDisabled = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }


    IEnumerator SetDisabled()
    {
        yield return new WaitForSeconds(TimeToDisabled);
        gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        StopCoroutine(SetDisabled());
        gameObject.SetActive(false);
    }
}


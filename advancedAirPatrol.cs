using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class advancedAirPatrol : MonoBehaviour
{


    public Transform[] points;
    public float speed = 4f;
    public float waitTime = 4f;
    bool CanGo = true;
    int i = 1;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.position = new Vector3(points[0].transform.position.x, points[0].transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (CanGo)
            transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        if (transform.position == points[i].position)// меняем позицию перебирая  индексы массива, когда индекс станет больше предпоследнего индекса он обнулиться
        {
            if (i < points.Length - 1)
                i++;
            else
                i = 0;
            
            CanGo = false;
            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(waitTime);

            /*if (transform.rotation.y == 0) этот флип здесь не нужен вот и закомментил
                transform.eulerAngles = new Vector3(0, 180, 0);
            else
                transform.eulerAngles = new Vector3(0, 0, 0);*/
            CanGo = true;
        }
    }
}


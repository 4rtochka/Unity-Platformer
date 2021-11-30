using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kamera : MonoBehaviour
{
    float speed = 3f;
    public Transform target; //target cell
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = target.position;
        position.z = transform.position.z;
        transform.position = Vector3.Lerp(transform.position, position, speed * Time.deltaTime);
    }
}

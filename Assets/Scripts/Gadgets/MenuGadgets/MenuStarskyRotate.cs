using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStarskyRotate : MonoBehaviour
{
    // Start is called before the first frame update
    public float minSpeed = 1;
    public float maxSpeed = 5;
    float tarSpeed = 1;
    float speed = 0;
    public float duration = 4;
    float timer = -1;
    void Start()
    {
        tarSpeed = Random.Range(minSpeed, maxSpeed);
        timer = Random.Range(duration * 0.5f, duration * 2f);
        speed = tarSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < 0)
        {
            tarSpeed = Random.Range(minSpeed, maxSpeed);
            timer = Random.Range(duration * 0.5f, duration * 2f);
        }
        speed = Mathf.MoveTowards(speed, tarSpeed, Time.deltaTime * 0.5f);
        timer -= Time.deltaTime;

        transform.Rotate(Vector3.forward *speed* Time.deltaTime,Space.Self);


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public Population population;
    public float threshold;
    public float speed;
    public float distance;

    private Vector3 target;
    private Vector3 lookingAt;

    // Start is called before the first frame update
    void Start()
    {
        lookingAt = Vector3.zero;
        target = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        target = population.avgPosition();
        lookingAt += (target - lookingAt) * (1f / speed);
        transform.LookAt(lookingAt);
        Debug.DrawLine(lookingAt, lookingAt + Vector3.up * 5, Color.red);
        Debug.DrawLine(lookingAt, lookingAt + Vector3.right * 5, Color.green);
        Debug.DrawLine(lookingAt, lookingAt + Vector3.forward * 5, Color.blue);
    }
}

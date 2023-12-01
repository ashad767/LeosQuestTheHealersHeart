using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTemp : MonoBehaviour
{
    public int speed = 5;
    public Rigidbody2D RB;
    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(speed * -Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, speed * -Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, speed * Time.deltaTime, 0);
        }
    }
}

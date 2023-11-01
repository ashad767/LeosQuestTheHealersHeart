using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] GameObject[] hoverPoints;
    int hoverIndex = 0;


    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Wizard>().idle)
        {
            hoverPoints[0].transform.SetParent(transform.parent);
            hoverPoints[1].transform.SetParent(transform.parent);

            transform.position = Vector3.MoveTowards(transform.position, hoverPoints[hoverIndex].transform.position, 0.3f * Time.deltaTime);

            if (Vector2.Distance(transform.position, hoverPoints[hoverIndex].transform.position) < 0.1f)
            {
                hoverIndex++;
                if (hoverIndex == hoverPoints.Length)
                {
                    hoverIndex = 0;
                }
            }
        }

        else
        {
            hoverPoints[0].transform.SetParent(transform);
            hoverPoints[1].transform.SetParent(transform);

            // Same behaviour as:
            //hoverPoints[0].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            //hoverPoints[1].transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }

    }
}
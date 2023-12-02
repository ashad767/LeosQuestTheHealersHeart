using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    public Transform follow;

    private void LateUpdate()
    {
        transform.position = follow.position + new Vector3(0, 0, -10);
    }
}

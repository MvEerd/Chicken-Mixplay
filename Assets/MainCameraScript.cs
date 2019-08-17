using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    public GameObject target;
    float mFollowRate = 1f;
    float mFollowHeight = 40f;


    void LateUpdate()
    {
        //Follow target GameObject from above using a top-down camera
        transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(0f, (mFollowHeight - target.transform.position.y), 0f), Time.deltaTime * mFollowRate);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScroller : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime);
    }
}

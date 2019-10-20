using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_World_Canvas_Following : MonoBehaviour
{
    Vector3 localPos = new Vector3(0f, 0f, 2f);

    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
            transform.position = Camera.main.transform.TransformPoint(localPos);
        }
    }
}

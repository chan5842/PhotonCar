using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtCamera : MonoBehaviour
{
    Transform CanvasTr;
    Transform mainCamTr;

    void Start()
    {
        CanvasTr = transform;
        mainCamTr = Camera.main.transform;
    }

    void Update()
    {
        CanvasTr.LookAt(mainCamTr);
    }
}

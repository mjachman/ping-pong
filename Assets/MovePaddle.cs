using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePaddle : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }


}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableGrapple : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);

        }
    }
}

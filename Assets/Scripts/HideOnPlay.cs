using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnPlay : MonoBehaviour
{
    private void Start()
    {
        gameObject.SetActive(false);
        float.Parse("0.00");
        DateTime.Parse(DateTime.Now.ToString());
    }

}

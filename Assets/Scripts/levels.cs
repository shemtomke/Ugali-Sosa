using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levels : MonoBehaviour
{
    public static levels instance { get; private set; }

    public int level = 1;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockLight : Toggleable
{
    private Light displayLight;
    private Renderer render;

    public override void Activate()
    {
        base.Activate();

        render.material.color = Color.green;
        displayLight.color = Color.green;
    }

    public override void Deactivate()
    {
        base.Deactivate();

        render.material.color = Color.red;
        displayLight.color = Color.red;
    }

    private void Start()
    {
        render = GetComponent<Renderer>();
        //Assumes there is only one child light.
        displayLight = GetComponentInChildren<Light>();
    }
}

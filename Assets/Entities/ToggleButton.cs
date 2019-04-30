using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : Toggleable, IInteractable
{
    public Toggleable[] connectedToggles;

    [SerializeField] Renderer buttonRenderer = null;

    public void Interact()
    {
        Toggle();
    }

    override public void Activate()
    {
        base.Activate();

        buttonRenderer.material.color = Color.green;

        foreach(Toggleable toggle in connectedToggles)
        {
            toggle.Activate();
        }
    }

    override public void Deactivate()
    {
        base.Deactivate();

        buttonRenderer.material.color = Color.red;

        foreach(Toggleable toggle in connectedToggles)
        {
            toggle.Deactivate();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.SendMessage("AddInteractable", this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.SendMessage("RemoveInteractable", this);
        }
    }
}

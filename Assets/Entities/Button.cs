using UnityEngine;

//A button that has its state toggled when it is interacted with, changing the colour to red.
public class Button : Toggle, IInteractable
{
    public Toggle[] controlledToggles; //The toggles that will be matched to the state of the button.

    [SerializeField] Renderer buttonRenderer = null; //The render for the button part of the object.

    //Toggles the state when the button is pressed.
    public void Interact()
    {
        IsActive = !IsActive;
    }

    //Causes the button to change to green, and activate the connected toggles.
    protected override void OnSelfActivated()
    {
        buttonRenderer.material.color = Color.green;

        foreach(Toggle toggle in controlledToggles)
        {
            toggle.IsActive = true;
        }
    }

    //Causes the button to change to red, and deactivate the connected toggles.
    protected override void OnSelfDeactivated()
    {
        buttonRenderer.material.color = Color.red;

        foreach(Toggle toggle in controlledToggles)
        {
            toggle.IsActive = false;
        }
    }

    //Add this button to the player's list of interactables when they enter the button's trigger area.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.gameObject.SendMessage("AddInteractable", this);
        }
    }

    //Remove this button from the player's list of interactables when they leave the button's trigger area.
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.SendMessage("RemoveInteractable", this);
        }
    }
}
using UnityEngine;

//Toggle that turns itself, and its light, to green when the toggle is activated; and red when deactivated.
[RequireComponent(typeof(Light))]
public class LockLight : Toggleable
{
    private Renderer render; //Renderer for the game object.
    private Light displayLight; //The child light of the object.

    //Object, and light, become green when activated.
    protected override void OnSelfActivated()
    {
        render.material.color = Color.green;
        displayLight.color = Color.green;
    }

    //Object, and light, become red when deactivated.
    protected override void OnSelfDeactivated()
    {
        render.material.color = Color.red;
        displayLight.color = Color.red;
    }

    private void Start()
    {
        render = GetComponent<Renderer>();
        displayLight = GetComponent<Light>();
    }
}
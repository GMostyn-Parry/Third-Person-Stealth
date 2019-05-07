//A class that acts as a logical AND gate for toggles; activating the controlled toggles when all input toggles are in the active state.
public class AndCheck : Toggle
{
    public Toggle[] inputToggles; //The toggles that must be activated for the gate to be set to active.
    public Toggle[] controlledToggles; //The toggles that will be matched to the state of the logic gate.

    //Sets the controlled toggles to true when the gate is activated.
    protected override void OnSelfActivated()
    {
        foreach(Toggle toggle in controlledToggles)
        {
            toggle.IsActive = true;
        }
    }

    //Sets the controlled toggles to false when the gate is deactivated.
    protected override void OnSelfDeactivated()
    {
        foreach(Toggle toggle in controlledToggles)
        {
            toggle.IsActive = false;
        }
    }

    //Link functions to events of input toggles to control when the gate is active, and deactive.
    protected override void OnEnable()
    {
        base.OnEnable();

        foreach(Toggle toggle in inputToggles)
        {
            toggle.OnActivated += OnInputToggleActivated;
            toggle.OnDeactivated += OnInputToggleDeactivated;
        }
    }

    //Unlink functions to events of input toggles when the script is disabled.
    protected override void OnDisable()
    {
        base.OnDisable();

        foreach(Toggle toggle in inputToggles)
        {
            toggle.OnActivated -= OnInputToggleActivated;
            toggle.OnDeactivated -= OnInputToggleDeactivated;
        }
    }

    //Checks if all toggles have entered the active state, and activates the gate, if they have done so.
    private void OnInputToggleActivated()
    {
        //Doesn't activate the gate if any of the toggles are inactive.
        foreach(Toggle toggle in inputToggles)
        {
            if(!toggle.IsActive) return;
        }

        IsActive = true;
    }

    //Deactivates the gate.
    //All linked toggles must be in the correct state for the gate to be active; thus if an event calls this function, then the gate should be inactive.
    private void OnInputToggleDeactivated()
    {
        IsActive = false;
    }
}
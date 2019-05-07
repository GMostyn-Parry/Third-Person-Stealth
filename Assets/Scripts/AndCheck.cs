//A class that acts as a logical AND gate for toggles; activating the controlled toggles when all input toggles are in the correct state.
public class AndCheck : Toggle
{
    public Toggle[] requiredActivated; //The toggles that must be activated for the gate to be active.
    public Toggle[] requiredDeactivated; //The toggles that must be deactivated for the gate to be active.

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

    //Add functions to events of linked toggles to control when the gate is active, and deactive.
    protected override void OnEnable()
    {
        base.OnEnable();

        foreach(Toggle toggle in requiredActivated)
        {
            toggle.OnActivated += OnToggleEnteredDesiredState;
            toggle.OnDeactivated += OnToggleEnteredWrongState;
        }

        foreach(Toggle toggle in requiredDeactivated)
        {
            toggle.OnActivated += OnToggleEnteredWrongState;
            toggle.OnDeactivated += OnToggleEnteredDesiredState;
        }
    }

    //Removed functions from events of linked toggles when the gate is active.
    protected override void OnDisable()
    {
        base.OnDisable();

        foreach(Toggle toggle in requiredActivated)
        {
            toggle.OnActivated -= OnToggleEnteredDesiredState;
            toggle.OnDeactivated -= OnToggleEnteredWrongState;
        }

        foreach(Toggle toggle in requiredDeactivated)
        {
            toggle.OnActivated -= OnToggleEnteredWrongState;
            toggle.OnDeactivated -= OnToggleEnteredDesiredState;
        }
    }

    //Checks if all toggles have entered the desired state, and activates the gate, if they have done so.
    private void OnToggleEnteredDesiredState()
    {
        //Doesn't activate the gate if any of the toggles that must be active aren't active.
        foreach(Toggle toggle in requiredActivated)
        {
            if(!toggle.IsActive) return;
        }

        //Doesn't activate the gate if any of the toggles that shouldn't be active are active.
        foreach(Toggle toggle in requiredDeactivated)
        {
            if(toggle.IsActive) return;
        }

        IsActive = true;
    }

    //Deactivates the gate, if is is already open.
    //All linked toggles must be in the correct state for the gate to be active; thus if an event calls this function, then the gate should be inactive.
    private void OnToggleEnteredWrongState()
    {
        IsActive = false;
    }
}

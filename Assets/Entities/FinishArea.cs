using UnityEngine;

public class FinishArea : MonoBehaviour
{
    public delegate void Reached();
    public static event Reached OnPlayerFinished;    

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerFinished?.Invoke();
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "AI/GuardStats")]
public class GuardStats : ScriptableObject
{
    [Range(0f, 180f)] public float maxVisionAngle = 60f; //Maximum angle the agent can see the target at.
    [Range(0f, 5f)] public float distanceToCatch = 2f; //How close the agent can catch the target.
    [Range(0f, 5f)] public float samplePositionRange = 5f; //How far the agent will sample a position on the NavMesh from the source position.

    [Range(0f, 200f)] public float searchRadius = 5f; //Radius to search for the target in.
    [Range(0f, 300f)] public float timeToSearch = 20f; //Length of time to actively search for the player after losing line-of-sight.
}

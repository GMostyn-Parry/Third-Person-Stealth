using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to represent a patrol point location for a patrolling agent; can return a random connected patrol point.
public class PatrolPoint : MonoBehaviour
{
    public PatrolPoint[] connectedPoints; //The points this patrol point connects to.

    //Returns a random connected point, or the last point if there are no other connected points.
    //Last point is the point the agent was previously at.
    public PatrolPoint ChooseConnectedPoint(PatrolPoint lastPoint)
    {
        //Create a list that doesn't include the previously visted point.
        List<PatrolPoint> possiblePoints = new List<PatrolPoint>(connectedPoints);
        possiblePoints.Remove(lastPoint);

        //Return the last point, if there are no remaining connected points.
        if (possiblePoints.Count == 0)
        {
            return lastPoint;
        }  

        //Return a random point from the list.
        return possiblePoints[Random.Range(0, possiblePoints.Count)];
    }

    //Draw a blue single-unit radius wire sphere to represent the gizmo.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    //Draw green lines to represent the connections to other patrol points, when the game object is selected.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        //Draw a line from the patrol point to every other connected patrol point.
        foreach (PatrolPoint point in connectedPoints)
        {
            Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }
}

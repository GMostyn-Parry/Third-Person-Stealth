using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        foreach (PatrolPoint point in connectedPoints)
        {
            Gizmos.DrawLine(transform.position, point.transform.position);
        }
    }
}

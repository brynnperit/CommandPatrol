using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Visibility  {
    public static VisibilityCone DetermineVisibilityConeToUse(VisibilityCone[] coneSet, float angle)
    {
        VisibilityCone toReturn = new VisibilityCone(0,180,0);
        foreach(VisibilityCone currentAngle in coneSet)
        {
            if (currentAngle.minAngle < angle && currentAngle.maxAngle > angle)
            {
                toReturn = currentAngle;
                break;
            }
        }
        return toReturn;
    }

    public static bool HasLineOfSight(Transform start, Transform end, float maxVisibilityDistance)
    {
        if (start != null && end != null)
        {
            Vector3 rayDirection = end.position - start.position;
            if (rayDirection.magnitude < maxVisibilityDistance)
            {

                RaycastHit hitResult = new RaycastHit();

                if (Physics.Raycast(start.position, rayDirection, out hitResult))
                {

                    if (hitResult.transform == end)
                    {
                        return true;
                    }

                }
            }
        }
        return false;
    }

    public static Agent UpdateGroupVisibility(Agent toCheckFrom, List<Agent> toCheck, Dictionary<Agent, float> visibilityList, Func<Agent, float> getAddedVisibilityValue, float visibilityFadeSpeed, float maxVisibilityDistance)
    {
        foreach (Agent currentAgent in toCheck)
        {
            if (HasLineOfSight(toCheckFrom.transform, currentAgent.transform, maxVisibilityDistance))
            {
                float currentTransformDistance = Vector3.Distance(toCheckFrom.transform.position, currentAgent.transform.position);
                if (!visibilityList.ContainsKey(currentAgent))
                {
                    visibilityList.Add(currentAgent, 0);
                }
                //The addition of (visibilityFadeSpeed * Time.deltaTime) is done so that objects that are currently within line of sight don't have their visibility fade away
                //TODO: Change this around so that objects can be more or less visible to guards, enemies, etc.
                visibilityList[currentAgent] += (currentAgent.getAddedVisibilityValue(toCheckFrom) * toCheckFrom.getPerception()) + (visibilityFadeSpeed * Time.deltaTime);
            }
        }
        //Decrementing the visibility values of all enemies prevents memory leaks, since references to removed enemies will shortly disappear from here.
        List<Agent> visibleAgentList = new List<Agent>(visibilityList.Keys);
        Agent mostNoticedAgent = null;
        foreach (Agent visible in visibleAgentList)
        {
            if (visible != null)
            {
                visibilityList[visible] -= (visibilityFadeSpeed * Time.deltaTime);
                if (visibilityList[visible] < 0)
                {
                    visibilityList.Remove(visible);
                }
                else if (mostNoticedAgent == null || visibilityList[visible] > visibilityList[mostNoticedAgent])
                {
                    mostNoticedAgent = visible;
                }

            }
            else
            {
                visibilityList.Remove(visible);
            }
        }

        return mostNoticedAgent;
    }
}

public struct VisibilityCone
{
    public float minAngle;
    public float maxAngle;
    public float Distance;

    public VisibilityCone (float minAngle, float maxAngle, float Distance)
    {
        this.minAngle = minAngle;
        this.maxAngle = maxAngle;
        this.Distance = Distance;
    }
}
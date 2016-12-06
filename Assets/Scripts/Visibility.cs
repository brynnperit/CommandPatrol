using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Visibility  {
    public static bool hasLineOfSight(Transform start, Transform end, float maxVisibilityDistance)
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

    public static Transform UpdateGroupVisibility(Transform initialPosition, Transform[] toCheck, int checkOffset, Dictionary<Transform, float> visibilityList, Func<float, float> getAddedVisibilityValue, float visibilityFadeSpeed, float maxVisibilityDistance)
    {
        if (toCheck.Length > checkOffset)
        {
            for (int currentTransformNum = checkOffset; currentTransformNum < toCheck.Length; currentTransformNum++)
            {
                Transform currentTransform = toCheck[currentTransformNum];
                if (hasLineOfSight(initialPosition, currentTransform, maxVisibilityDistance))
                {
                    float currentTransformDistance = Vector3.Distance(initialPosition.position, currentTransform.position);
                    if (!visibilityList.ContainsKey(currentTransform))
                    {
                        visibilityList.Add(currentTransform, 0);
                    }
                    //The addition of (visibilityFadeSpeed * Time.deltaTime) is done so that objects that are currently in view don't have their visibility fade away
                    //TODO: Change this around so that objects can be more or less visible to guards, enemies, etc.
                    visibilityList[currentTransform] += getAddedVisibilityValue(currentTransformDistance) + (visibilityFadeSpeed * Time.deltaTime);
                }
            }
        }
        //Decrementing the visibility values of all enemies prevents memory leaks, since references to removed enemies will shortly disappear from here.
        List<Transform> visibleTransformList = new List<Transform>(visibilityList.Keys);
        Transform mostNoticedTransform = null;
        foreach (Transform visible in visibleTransformList)
        {
            if (visible != null)
            {
                visibilityList[visible] -= (visibilityFadeSpeed * Time.deltaTime);
                if (visibilityList[visible] < 0)
                {
                    visibilityList.Remove(visible);
                }
                else if (mostNoticedTransform == null || visibilityList[visible] > visibilityList[mostNoticedTransform])
                {
                    mostNoticedTransform = visible;
                }

            }
            else
            {
                visibilityList.Remove(visible);
            }
        }

        return mostNoticedTransform;
    }
}

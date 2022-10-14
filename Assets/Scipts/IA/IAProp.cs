using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IAProp : PropertyAttribute
{
    public readonly Type IaPoints;

   
    public IAProp(Type iaPoints)
    {
        IaPoints = iaPoints.GetType();
    }
}

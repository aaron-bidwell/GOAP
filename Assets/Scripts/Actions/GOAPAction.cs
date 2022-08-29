using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Abstract class, all Classes inheriting from this must implement all methods below
public abstract class GOAPAction 
{
    // This is what is required in order to complete this action
    public abstract KeyValuePair<Goals, bool> getPrecondition();

    // This is what will be given after you complete this action
    public abstract KeyValuePair<Goals, bool> getEffects();

    // Action preformed when this state is achieved
    public abstract IEnumerator useAction(GameObject gameObject);
   
    // The Cost Associated with performing this Action
    public abstract float getCost(GameObject gameObject);
   
    // Name of Action
    public abstract string getName();
}

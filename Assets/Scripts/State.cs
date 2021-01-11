using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]
public class State : ScriptableObject
{

    [SerializeField] string gameText;
    [SerializeField] string name;
    [SerializeField] State[] otherStates;
    [SerializeField] int actions;
    [SerializeField] int addItems;
    [SerializeField] bool triggerMatches;
    [SerializeField] bool triggerLantern;
    [SerializeField] bool triggerGun;
    [SerializeField] bool triggerKnife;
    [SerializeField] bool triggerGear;
    public string GetStateText()
    {
        return gameText;
    }

    // Update is called once per frame
    public State[] GetOtherStates()
    {
        return otherStates;
    }

    public int GetActionValue()
    {
        return actions;
    }

    public string GetNameValue()
    {
        return name;
    }

    public int GetItemAddValue()
    {
        return addItems;
    }

    public bool TriggerMatches()
    {
        return triggerMatches;
    }

    public bool TriggerLantern()
    {
        return triggerMatches;
    }

    public bool TriggerGun()
    {
        return triggerMatches;
    }

    public bool TriggerKnife()
    {
        return triggerMatches;
    }

    public bool TriggerGear()
    {
        return triggerMatches;
    }


    //override comparaison operators
}

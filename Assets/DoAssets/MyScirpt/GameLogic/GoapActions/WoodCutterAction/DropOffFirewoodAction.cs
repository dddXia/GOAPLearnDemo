using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffFirewoodAction : MyGoapAction
{
    private bool droppedOffFirewood = false;
    private MySupplyPileComponent targetSupplyPile; // where we drop off the ore

    DropOffFirewoodAction()
    {
        //前置条件，有木材
        addPrecondition("hasFirewood", true); // can't drop off logs if we don't already have some
        //DropOffFirewoodAction后产生的效果
        addEffect("hasFirewood", false); // we now have no logs
        addEffect("collectFirewood", true); // we collected logs
    }

    public override void reset()
    {
        droppedOffFirewood = false;
        targetSupplyPile = null;
    }

    public override bool isDone()
    {
        return droppedOffFirewood;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a supply pile so we can drop off the logs
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest supply pile
        MySupplyPileComponent[] supplyPiles = (MySupplyPileComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(MySupplyPileComponent));
        MySupplyPileComponent closest = null;
        float closestDist = 0;

        foreach (MySupplyPileComponent supply in supplyPiles) {
            if (closest == null) {
                // first one, so choose it for now
                closest = supply;
                closestDist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
            } else {
                // is this one closer than the last?
                float dist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist) {
                    // we found a closer one, use it
                    closest = supply;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetSupplyPile = closest;
        target = targetSupplyPile.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        MyBackpackComponent backpack = (MyBackpackComponent)agent.GetComponent(typeof(MyBackpackComponent));
        targetSupplyPile.numFirewood += backpack.numFirewood;
        droppedOffFirewood = true;
        backpack.numFirewood = 0;

        return true;
    }
}

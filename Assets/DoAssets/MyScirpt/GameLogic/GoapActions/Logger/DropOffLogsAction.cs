using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOffLogsAction : MyGoapAction {
    private bool droppedOffLogs = false;
    private MySupplyPileComponent targetSupplyPile; // where we drop off the logs

    public DropOffLogsAction()
    {
        //条件：有新原木
        //如果我们没有原木，就不能放弃原木
        addPrecondition("hasLogs", true); // can't drop off logs if we don't already have some
                                          //效果：没有新原木了，继续收集原木
        addEffect("hasLogs", false); // we now have no logs
        addEffect("collectLogs", true); // we collected logs
    }


    public override void reset()
    {
        droppedOffLogs = false;
        targetSupplyPile = null;
    }

    public override bool isDone()
    {
        return droppedOffLogs;
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
        targetSupplyPile.numLogs += backpack.numLogs;
        droppedOffLogs = true;
        backpack.numLogs = 0;

        return true;
    }
}

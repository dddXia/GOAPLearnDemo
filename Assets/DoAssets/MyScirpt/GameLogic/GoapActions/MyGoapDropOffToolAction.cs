using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGoapDropOffToolAction : MyGoapAction {

    private bool droppedOffTools = false;
    private MySupplyPileComponent targetSupplyPile; // where we drop off the  tools

    public MyGoapDropOffToolAction()
    {
        //条件：有新工具
        //如果我们没有一些工具，就不能放弃工具
        addPrecondition("hasNewTools", true); // can't drop off tools if we don't already have some
                                              //效果：没有新工具了，继续收集工具
        addEffect("hasNewTools", false); // we now have no tools
        addEffect("collectTools", true); // we collected tools
    }


    public override void reset()
    {
        droppedOffTools = false;
        targetSupplyPile = null;
    }

    public override bool isDone()
    {
        return droppedOffTools;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a supply pile so we can drop off the tools
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest supply pile that has spare tools
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
        targetSupplyPile.numTools += 2;
        droppedOffTools = true;
        //TODO play effect, change actor icon

        return true;
    }
}

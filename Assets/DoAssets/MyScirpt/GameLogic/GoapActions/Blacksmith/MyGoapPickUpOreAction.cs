using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGoapPickUpOreAction : MyGoapAction {
    private bool hasOre = false;
    private MySupplyPileComponent targetSupplyPile; // where we get the ore from

    public MyGoapPickUpOreAction()
    {
        //条件：身上没有铁矿石
        addPrecondition("hasOre", false); // don't get a ore if we already have one
        //效果：有铁矿石
        addEffect("hasOre", true); // we now have a ore
    }


    public override void reset()
    {
        hasOre = false;
        targetSupplyPile = null;
    }

    public override bool isDone()
    {
        return hasOre;
    }

    public override bool requiresInRange()
    {
        //是的，我们需要在一个供应堆附近，这样我们就可以把矿石捡起来了
        return true; // yes we need to be near a supply pile so we can pick up the ore
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest supply pile that has spare ores
        MySupplyPileComponent[] supplyPiles = (MySupplyPileComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(MySupplyPileComponent));
        MySupplyPileComponent closest = null;
        float closestDist = 0;

        foreach (MySupplyPileComponent supply in supplyPiles) {
            // 我们需要3个矿石
            if (supply.numOre >= 3) { // we need to take 3 ore
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
        }
        if (closest == null)
            return false;

        targetSupplyPile = closest;
        target = targetSupplyPile.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (targetSupplyPile.numOre >= 3) {
            targetSupplyPile.numOre -= 3;
            hasOre = true;
            //TODO play effect, change actor icon
            MyBackpackComponent backpack = (MyBackpackComponent)agent.GetComponent(typeof(MyBackpackComponent));
            backpack.numOre += 3;

            return true;
        } else {
            // we got there but there was no ore available! Someone got there first. Cannot perform action
            return false;
        }
    }
}

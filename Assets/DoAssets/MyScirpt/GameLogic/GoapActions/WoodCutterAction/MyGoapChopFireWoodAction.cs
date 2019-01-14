using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGoapChopFireWoodAction : MyGoapAction {
    private bool chopped = false;
    private MyChoppingBlockComponent targetChoppingBlock; // where we chop the firewood

    private float startTime = 0;
    public float workDuration = 2; // seconds

    public MyGoapChopFireWoodAction()
    {
        //条件：有新工具,没有材料
        // 如果我们有柴，我们不想要更多
        addPrecondition("hasTool", true); // we need a tool to do this
        addPrecondition("hasFirewood", false); // if we have firewood we don't want more
        //效果：有柴
        addEffect("hasFirewood", true);
    }


    public override void reset()
    {
        chopped = false;
        targetChoppingBlock = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return chopped;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a chopping block
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest chopping block that we can chop our wood at
        MyChoppingBlockComponent[] blocks = (MyChoppingBlockComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(MyChoppingBlockComponent));
        MyChoppingBlockComponent closest = null;
        float closestDist = 0;

        foreach (MyChoppingBlockComponent block in blocks) {
            if (closest == null) {
                // first one, so choose it for now
                closest = block;
                closestDist = (block.gameObject.transform.position - agent.transform.position).magnitude;
            } else {
                // is this one closer than the last?
                float dist = (block.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist) {
                    // we found a closer one, use it
                    closest = block;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetChoppingBlock = closest;
        target = targetChoppingBlock.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > workDuration) {
            // finished chopping
            MyBackpackComponent backpack = (MyBackpackComponent)agent.GetComponent(typeof(MyBackpackComponent));
            backpack.numFirewood += 5;
            chopped = true;
            MyToolComponent tool = backpack.tool.GetComponent(typeof(MyToolComponent)) as MyToolComponent;
            if (tool) {
                tool.use(0.34f);
                if (tool.destroyed()) {
                    Destroy(backpack.tool);
                    backpack.tool = null;
                }
            }
        }
        return true;
    }

}

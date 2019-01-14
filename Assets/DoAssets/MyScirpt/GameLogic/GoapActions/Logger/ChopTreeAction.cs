using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChopTreeAction : MyGoapAction {
    private bool chopped = false;
    private MyTreeComponent targetTree; // where we get the logs from

    private float startTime = 0;
    public float workDuration = 2; // seconds

    public ChopTreeAction()
    {
        //条件：有新工具,没有原木
        addPrecondition("hasTool", true); // we need a tool to do this
        addPrecondition("hasLogs", false); // if we have logs we don't want more
                                           //效果：有原木
        addEffect("hasLogs", true);
    }


    public override void reset()
    {
        chopped = false;
        targetTree = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return chopped;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a tree
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest tree that we can chop
        MyTreeComponent[] trees = (MyTreeComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(MyTreeComponent));
        MyTreeComponent closest = null;
        float closestDist = 0;

        foreach (MyTreeComponent tree in trees) {
            if (closest == null) {
                // first one, so choose it for now
                closest = tree;
                closestDist = (tree.gameObject.transform.position - agent.transform.position).magnitude;
            } else {
                // is this one closer than the last?
                float dist = (tree.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist) {
                    // we found a closer one, use it
                    closest = tree;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetTree = closest;
        target = targetTree.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > workDuration) {
            // finished chopping
            MyBackpackComponent backpack = (MyBackpackComponent)agent.GetComponent(typeof(MyBackpackComponent));
            backpack.numLogs += 1;
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

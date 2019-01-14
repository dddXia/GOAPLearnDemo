using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGoapForgeToolAction : MyGoapAction
{
    private bool forged = false;
    //锻造工具的地方
    private MyForgeComponent targetForge; // where we forge tools

    private float startTime = 0;
    public float forgeDuration = 2; // seconds


    public MyGoapForgeToolAction()
    {
        //条件：有铁矿石
        addPrecondition("hasOre", true);
        //效果：产生工具
        addEffect("hasNewTools", true);
    }


    public override void reset()
    {
        forged = false;
        targetForge = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return forged;
    }

    public override bool requiresInRange()
    {
        // 是的，我们需要在一个熔炉附近
        return true; // yes we need to be near a forge 
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest forge
        // 找到最近的锻造地点
        MyForgeComponent[] forges = (MyForgeComponent[])UnityEngine.GameObject.FindObjectsOfType(typeof(MyForgeComponent));
        MyForgeComponent closest = null;
        float closestDist = 0;

        foreach (MyForgeComponent forge in forges) {
            if (closest == null) {
                // first one, so choose it for now
                closest = forge;
                closestDist = (forge.gameObject.transform.position - agent.transform.position).magnitude;
            } else {
                // is this one closer than the last?
                float dist = (forge.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist) {
                    // we found a closer one, use it
                    closest = forge;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetForge = closest;
        target = targetForge.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > forgeDuration) {
            // finished forging a tool
            // 完成锻造
            MyBackpackComponent backpack = (MyBackpackComponent)agent.GetComponent(typeof(MyBackpackComponent));
            backpack.numOre = 0;
            forged = true;
        }
        return true;
    }
}
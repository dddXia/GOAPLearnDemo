
using System;
using UnityEngine;

//开采矿石
public class MyGoapMineOreAction : MyGoapAction
{
    private bool mined = false;
    private MyIronRockComponent targetRock; // where we get the ore from

    private float startTime = 0;
    public float miningDuration = 2; // seconds

    public MyGoapMineOreAction()
    {
        //条件：有新工具,没有矿石
        addPrecondition("hasTool", true); // we need a tool to do this
        addPrecondition("hasOre", false); // if we have ore we don't want more
                                          //效果：有铁矿石
        addEffect("hasOre", true);
    }


    public override void reset()
    {
        mined = false;
        targetRock = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return mined;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a rock
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest rock that we can mine
        MyIronRockComponent[] rocks = FindObjectsOfType(typeof(MyIronRockComponent)) as MyIronRockComponent[];
        MyIronRockComponent closest = null;
        float closestDist = 0;

        foreach (MyIronRockComponent rock in rocks) {
            if (closest == null) {
                // first one, so choose it for now
                closest = rock;
                closestDist = (rock.gameObject.transform.position - agent.transform.position).magnitude;
            } else {
                // is this one closer than the last?
                float dist = (rock.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist) {
                    // we found a closer one, use it
                    closest = rock;
                    closestDist = dist;
                }
            }
        }
        targetRock = closest;
        target = targetRock.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
            startTime = Time.time;

        if (Time.time - startTime > miningDuration) {
            // finished mining
            MyBackpackComponent backpack = (MyBackpackComponent)agent.GetComponent(typeof(MyBackpackComponent));
            backpack.numOre += 2;
            mined = true;
        
            if (backpack.tool) {
                MyToolComponent tool = backpack.tool.GetComponent(typeof(MyToolComponent)) as MyToolComponent;
                    if (tool) {
                        tool.use(0.5f);
                        if (tool.destroyed()) {
                            Console.WriteLine("--------------------{0}", backpack.tool);
                            Destroy(backpack.tool);
                            backpack.tool = null;
                        }
                    }
            }
        }
        return true;
    }

}
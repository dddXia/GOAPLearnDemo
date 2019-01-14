using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;

public class Miner : Labourer
{
    /**
 * Our only goal will ever be to mine ore.
 * The MineOreAction will be able to fulfill this goal.
 * 目标采矿
 */
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();
        goal.Add( new KeyValuePair<string, object>("collectOre", true) );
        return goal;
    }
}



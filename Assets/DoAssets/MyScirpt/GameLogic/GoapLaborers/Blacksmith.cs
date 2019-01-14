using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;

public class Blacksmith : Labourer {
    /**
         * Our only goal will ever be to make tools.
         * The ForgeTooldAction will be able to fulfill this goal.
         * 
         * 我们唯一的目标就是制造工具。
         * ForgeTooldAction 将能够实现这个目标。
         */
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("collectTools", true));
        return goal;
    }
}

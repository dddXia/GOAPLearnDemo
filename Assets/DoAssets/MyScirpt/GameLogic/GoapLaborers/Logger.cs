﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GOAP;

public class Logger : Labourer {
    public override HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("collectLogs", true));
        return goal;
    }
}

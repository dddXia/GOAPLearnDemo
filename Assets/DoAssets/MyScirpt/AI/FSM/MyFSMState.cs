using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MyFSMState {
    void Update(MyFSM fsm, GameObject gameObject);
}
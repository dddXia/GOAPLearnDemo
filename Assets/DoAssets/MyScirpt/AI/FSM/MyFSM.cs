using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFSM {
    public delegate void MyFSMState_(MyFSM fsm, GameObject gameObject);
    private Stack<MyFSMState_> stateStack = new Stack<MyFSMState_>();//使用栈保存

    public void Update(GameObject gameObject)
    {
        if (stateStack.Peek() != null)
            stateStack.Peek().Invoke(this, gameObject);
    }

    public void pushState(MyFSMState_ state)
    {
        stateStack.Push(state);
    }

    public void popState()
    {
        stateStack.Pop();
    }
}
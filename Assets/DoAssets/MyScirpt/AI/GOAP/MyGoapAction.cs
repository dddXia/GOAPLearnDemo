using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MyGoapAction：代理类需要执行的行动，
/// 代理类具体Action类继承自该类
/// </summary>
public abstract class MyGoapAction : MonoBehaviour {

    private HashSet<KeyValuePair<string, object>> preconditions;
    private HashSet<KeyValuePair<string, object>> effects;

    private bool inRange = false;

    public float cost = 1f;//行动成本

    //通常一个操作需要在一个对象上执行，可以是null
    public GameObject target = null;

    public MyGoapAction()
    {
        preconditions = new HashSet<KeyValuePair<string, object>>();
        effects = new HashSet<KeyValuePair<string, object>>();
    }

    public void doReset()
    {
        inRange = false;
        target = null;
        reset();
    }

    //重置其他需要重置的变量
    public abstract void reset();

    //判断行动是否完成
    public abstract bool isDone();

    /// <summary>
    ///  对于代理的一些行动，需要通过检查前置条件确定行动是否可以执行
    /// </summary>
    public abstract bool checkProceduralPrecondition(GameObject agent);

    /// <summary>
    /// 执行该行动，并判断行动是否执行成功。
    /// 如果，行动不再执行，应该清除行动队列，并且目标无法达成
    /// </summary>
    public abstract bool perform(GameObject agent);

    //判断目标是否需要在访问内
    public abstract bool requiresInRange();

    //设定是否在范围内
    public void setInRange(bool inRange)
    {
        this.inRange = inRange;
    }

    public bool isInRange()
    {
        return inRange;
    }

        //添加前置条件
        public void addPrecondition(string key, object value)
    {
        preconditions.Add(new KeyValuePair<string, object>(key, value));
    }

    //移除前置条件
    public void removePrecondition(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in preconditions) {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            preconditions.Remove(remove);
    }

    //添加效果
    public void addEffect(string key, object value)
    {
        effects.Add(new KeyValuePair<string, object>(key, value));
    }

    //移除效果
    public void removeEffect(string key)
    {
        KeyValuePair<string, object> remove = default(KeyValuePair<string, object>);
        foreach (KeyValuePair<string, object> kvp in effects) {
            if (kvp.Key.Equals(key))
                remove = kvp;
        }
        if (!default(KeyValuePair<string, object>).Equals(remove))
            effects.Remove(remove);
    }

    //获取所有前置条件
    public HashSet<KeyValuePair<string, object>> Preconditions
    {
        get
        {
            return preconditions;
        }
    }

    public HashSet<KeyValuePair<string, object>> Effects
    {
        get
        {
            return effects;
        }
    }

}
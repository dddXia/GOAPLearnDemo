using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该接口由labor继承，而代理类继承自labor
/// </summary>
public interface IMyGoap {

    /**
	 * The starting state of the Agent and the world.
	 * Supply what states are needed for actions to run.
	 * 
	 * 代理和世界的起始状态，对于执行行动提供一些必要的状态
	 */
    HashSet<KeyValuePair<string, object>> getWorldState();

    /**
	 * Give the planner a new goal so it can figure out 
	 * the actions needed to fulfill it.
	 * 
	 *给规划器提供一个新的目标决策，以便让规划出需要执行的行动
	 */
    HashSet<KeyValuePair<string, object>> createGoalState();

    /**
	 * No sequence of actions could be found for the supplied goal.
	 * You will need to try another goal
	 * 
	 *对于目标规划器没有规划出计划，需要重新制定另一个目标
	 */
    void planFailed(HashSet<KeyValuePair<string, object>> failedGoal);

    /**
	 * A plan was found for the supplied goal.
	 * These are the actions the Agent will perform, in order.
	 * 
	 * 找到一个可以达成目标的计划，代理将依次执行这些计划
	 */
    void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<MyGoapAction> actions);

    /**
	 * All actions are complete and the goal was reached. Hooray!
	 * 
	 * 所有的行动都完成了
	 */
    void actionsFinished();

    /**
	 * One of the actions caused the plan to abort.
	 * That action is returned.
	 * 
	 *其中一个行动导致计划失败。这一行动被返回。
	 */
    void planAborted(MyGoapAction aborter);

    /**
	 * Called during Update. Move the agent towards the target in order
	 * for the next action to be able to perform.
	 * Return true if the Agent is at the target and the next action can perform.
	 * False if it is not there yet.
	 * 
	 * 在更新过程中调用。将代理向目标移动，以便执行下一步操作。
	 * 如果代理是在目标内并且下一个动作可以执行，返回true。
	 * 否则返回false
	 */
    bool moveAgent(MyGoapAction nextAction);
}

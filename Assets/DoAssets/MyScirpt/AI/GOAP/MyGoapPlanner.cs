using System;
using System.Collections.Generic;
using UnityEngine;
namespace GOAP
{

    /// <summary>
    ///  Plans what actions can be completed in order to fulfill a goal state.
    ///  为完成目标规划动作
    /// </summary>
    public class MyGoapPlanner
    {
        /// <summary>
        ///规划好行动计划以达到目标，如果规划器未能找到行动计划或为实现目标的操作列表，则返回null
        ///  Returns null if a plan could not be found, or a list of the actions
        ///  that must be performed, in order, to fulfill the goal.
        /// </summary>
        public Queue<MyGoapAction> plan(GameObject agent,
                                      HashSet<MyGoapAction> availableActions,
                                      HashSet<KeyValuePair<string, object>> worldState,
                                      HashSet<KeyValuePair<string, object>> goal)
        {
            // 重置行动，以便重新开始
            foreach (MyGoapAction a in availableActions) {
                a.doReset();
            }

            // check what actions can run using their checkProceduralPrecondition
            //检查在代理的前置条件下，哪些行动是可以执行的
            HashSet<MyGoapAction> usableActions = new HashSet<MyGoapAction>();
            foreach (MyGoapAction a in availableActions) {
                if (a.checkProceduralPrecondition(agent))
                    usableActions.Add(a);
            }

            // we now have all actions that can run, stored in usableActions
            // build up the tree and record the leaf nodes that provide a solution to the goal.
            //已经获取到了在当前前置条件下可以执行的所有行动，并将其保存在usableActions中
            //将达成目标的解决方案保存在叶子节点，建立该树
            List<Node> leaves = new List<Node>();

            // build graph
            // 生成图
            Node start = new Node(null, 0, worldState, null);
            bool success = buildGraph(start, leaves, usableActions, goal);

            if (!success) {
                Debug.Log("NO PLAN");
                return null;
            }

            // get the cheapest leaf
            // 获得代价最低的叶子
            Node cheapest = null;
            foreach (Node leaf in leaves) {
                if (cheapest == null)
                    cheapest = leaf;
                else {
                    if (leaf.runningCost < cheapest.runningCost)
                        cheapest = leaf;
                }
            }

            // get its node and work back through the parents
            // 获取代价最低的行动路径
            List<MyGoapAction> result = new List<MyGoapAction>();
            Node n = cheapest;
            while (n != null) {
                if (n.action != null) {
                    // 插入前面的动作
                    result.Insert(0, n.action); // insert the action in the front
                }
                n = n.parent;
            }
            // we now have this action list in correct order
            // 已建立行动队列

            Queue<MyGoapAction> queue = new Queue<MyGoapAction>();
            foreach (MyGoapAction a in result) {
                queue.Enqueue(a);
            }

            // 将计划返回
            return queue;
        }

        /**
         * Returns true if at least one solution was found.
         * The possible paths are stored in the leaves list. Each leaf has a
         * 'runningCost' value where the lowest cost will be the best action
         * sequence.
         * 递归的构建树，（可行计划）
         * 如果找到至少一个解决方案，返回true。
         * 可能的路径被存储在列表中。每片叶子都有一个“运行成本的价值，最低的成本将是最好的行动序列。
         * parent为下一个节点的父节点，Node保存前驱节点
         */
        private bool buildGraph(Node parent, List<Node> leaves, HashSet<MyGoapAction> usableActions, HashSet<KeyValuePair<string, object>> goal)
        {
            bool foundOne = false;

            // go through each action available at this node and see if we can use it here
            // 查看该节点上的每一个可行的行动，确认这些行动是否可以在当前前置条件下使用
            foreach (MyGoapAction action in usableActions) {

                // if the parent state has the conditions for this action's preconditions, we can use it here
                // 如果父节点的条件为这个动作的前置条件，那么该行动在当签前置条件下可行
                if (inState(action.Preconditions, parent.state)) {
                    // apply the action's effects to the parent state
                    // 将行动的效果应用于父节点状态、、parent.state（代理状态）
                    HashSet<KeyValuePair<string, object>> currentState = populateState(parent.state, action.Effects);
                    //Debug.Log(GoapAgent.prettyPrint(currentState));
                    Node node = new Node(parent, parent.runningCost + action.cost, currentState, action);

                    if (inState(goal, currentState)) {
                        // we found a solution!
                        leaves.Add(node);
                        foundOne = true;
                    } else {
                        // not at a solution yet, so test all the remaining actions and branch out the tree
                      
                        //(subset为行动序列，被移除的行动集合)目前还没有解决方案，测试剩余的所有行动和该树分支
                        HashSet<MyGoapAction> subset = actionSubset(usableActions, action);
                        bool found = buildGraph(node, leaves, subset, goal);//当前方案不可行，构建另外一个行动方案
                        if (found)
                            foundOne = true;
                    }
                }
            }
            return foundOne;
        }

        /**
         * Create a subset of the actions excluding the removeMe one. Creates a new set.
         * 
         * 创造的行为排除removeme一子集。创建一个新的集合。
         */
        private HashSet<MyGoapAction> actionSubset(HashSet<MyGoapAction> actions, MyGoapAction removeMe)
        {
            HashSet<MyGoapAction> subset = new HashSet<MyGoapAction>();
            foreach (MyGoapAction a in actions) {
                if (!a.Equals(removeMe))
                    subset.Add(a);
            }
            return subset;
        }

        /**
         * Check that all items in 'test' are in 'state'. If just one does not match or is not there
         * then this returns false.
         * 
         * 检查所有先决条件是否都在世界状态内（世界状态是否满足所有的先决条件是否）
         */
        private bool inState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
        {
            bool allMatch = true;
            foreach (KeyValuePair<string, object> t in test) {
                bool match = false;
                foreach (KeyValuePair<string, object> s in state) {
                    if (s.Equals(t)) {
                        match = true;
                        break;
                    }
                }
                if (!match)
                    allMatch = false;
            }
            return allMatch;
        }

        /**
         * Apply the stateChange to the currentState
         * 改变当前状态，返回代理应用effect之后的状态
         *
         */
        private HashSet<KeyValuePair<string, object>> populateState(HashSet<KeyValuePair<string, object>> currentState, HashSet<KeyValuePair<string, object>> stateChange)
        {
            HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();
            // copy the key-Vale  over as new objects
            // 复制kVps作为新的对象
            foreach (KeyValuePair<string, object> s in currentState) {
                state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
            }

            foreach (KeyValuePair<string, object> change in stateChange) {
                // if the key exists in the current state, update the Value
                // 如果key存在于当前状态，更新值
                bool exists = false;

                foreach (KeyValuePair<string, object> s in state) {
                    if (s.Equals(change)) {
                        exists = true;
                        break;
                    }
                }

                //将效果应用到状态上，有则修改，没有则添加
                if (exists) {
                    state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(change.Key); });//移除旧的状态
                    KeyValuePair<string, object> updated = new KeyValuePair<string, object>(change.Key, change.Value);//效果改变了状态
                    state.Add(updated);
                }
                // if it does not exist in the current state, add it
                // 如果key不存在于当前状态，添加它
                else {
                    state.Add(new KeyValuePair<string, object>(change.Key, change.Value));
                }
            }
            return state;
        }

        /**
         * Used for building up the graph and holding the running costs of actions.
         * 
         * 用于建立用于规划行动的图表，
         */
        private class Node
        {
            public Node parent;
            public float runningCost;
            public HashSet<KeyValuePair<string, object>> state;
            public MyGoapAction action;

            public Node(Node parent, float runningCost, HashSet<KeyValuePair<string, object>> state, MyGoapAction action)
            {
                this.parent = parent;
                this.runningCost = runningCost;
                this.state = state;
                this.action = action;
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using System.Linq;
using UnityEngine;


public class GOAP : MonoBehaviour
{
    [SerializeField] public GameObject workerPrefab;

    void Start()
    {
        // Spawn all the initial game elements
        var workerScript = workerPrefab.GetComponent<Worker>();
        SpawnRandom(workerPrefab, 3);
        SpawnRandom(workerScript.treePrefab, 15);
        SpawnRandom(workerScript.woodPilePrefab, 3);
        SpawnRandom(workerScript.axePrefab, 5);
       
        // Build a list of acceptable actions an NPC can choose from
        var useableActions = new List<GOAPAction>();
        useableActions.Add(new ChopTree());
        useableActions.Add(new GrabWoodFromPile());
        useableActions.Add(new BuildFire());
        useableActions.Add(new GetAxe());
       
        // Create a Starting Goal for the NPC's
        GOAPAction goalAction = useableActions.Find(action => action.getEffects().Key == Goals.is_warm); 
       
        // Find all workers in the level and start the cycle of building plans and executing plans
        var workers = GameObject.FindGameObjectsWithTag("Worker");
        foreach (var worker in workers)
        {
            AssignPlan(worker, goalAction, useableActions);
        }
    }

    // This will build all possible plans, find the cheapest one, than assign it to that worker, once that individual worker 
    // has finished its plan it will start all over
    void AssignPlan(GameObject worker, GOAPAction goalAction, List<GOAPAction> useableActions)
    {
        Node headNode = new Node(null, goalAction, false, worker); 
        BuildGraph(headNode, Goals.is_warm, useableActions);
        var actions = Enumerable.Reverse(GetCheapestPlan(headNode)).ToList();
        StartCoroutine(RunPlan(actions, worker, (finished) => AssignPlan(worker, goalAction, useableActions)));
    }

    // Coroutine and that will execute the list of actions and reset its self with a call back when finished
    IEnumerator RunPlan(List<GOAPAction> actions, GameObject worker, System.Action<bool> callback)
    {
        foreach (var action in actions)
        {
            yield return action.useAction(worker);
        }
        callback(true);
    }

     void BuildGraph(Node node, Goals goal, List<GOAPAction> actions)
     {
         // If there are no preconditions this means we have found an end node
        if (node.action.getPrecondition().Key == Goals.none)
        {
            return;
        }
        else
        {
            List<GOAPAction> goalActions = actions.FindAll(action => action.getEffects().Key == node.action.getPrecondition().Key);
            List<GOAPAction> filteredActions = actions.FindAll(action => action.getEffects().Key != node.action.getPrecondition().Key);

            // for all possible goals at this level build out a branching list
            var edges = new List<Node>();
            goalActions.ForEach(action =>
            {
                edges.Add(new Node(null, action, false, node.worker));
            });
                
            node.edges = edges;
            
            node.edges.ForEach(edge =>
            {
                // Recursively call the next graph and pass in all actions that have not be utilized yet
                BuildGraph(edge, edge.action.getPrecondition().Key, filteredActions);
            });
        }
    }

    List<GOAPAction> GetCheapestPlan(Node node)
    {
        Node tempNode = node;
        Node headNode = node;
        List<GOAPAction> tempList = new List<GOAPAction>();
        List<List<GOAPAction>> result = new List<List<GOAPAction>>();
           
        // Keep Executing until all Nodes are Visisted
        while (headNode.visisted == false)
        {
            // Build a Temp List, Note will be discarded if it reaches a point with all visisted children
            tempList.Add(tempNode.action);
            
            if (tempNode.edges == null || tempNode.edges.Count == 0)
            {
                // No more children, Path found add it to our list, reset it and mark it as visited
                result.Add(new List<GOAPAction>(tempList));
                tempList.Clear();
                tempNode.visisted = true;
                tempNode = headNode;
            }
            else
            {
                // If any child node is not visisted than set that first unvisisted node as are next node
                if (tempNode.edges.Any(x => x.visisted == false)) {
                    tempNode = tempNode.edges.FirstOrDefault(x => x.visisted == false);
                }
                else
                {
                    // All child nodes are visisted, mark it and reset are list
                    tempList.Clear();
                    tempNode.visisted = true;
                    tempNode = headNode;
                }
            }
        }

        // All Paths are built, search through are list and find the cheapest one
        var smallestIndex = 0;
        var index = 0;
        result.ForEach(list =>
        {
            if (result[smallestIndex].Sum(action => action.getCost(node.worker)) > list.Sum(action => action.getCost(node.worker)))
            {
                smallestIndex = index;
            }
            index++;
        });
        return result[smallestIndex];
    }

    // Handles spaning any GameObject in a Random Location in Camera View
    void SpawnRandom(GameObject gameObject, int count)
    {
        for(int i = 0; i < count; i++)
        {
            float spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - 10)).y);
            float spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - 10, 0)).x);
            Vector3 randomPosition = new Vector3(spawnX,spawnY,0);
            Quaternion quaternion = new Quaternion();
            Instantiate(gameObject, randomPosition, quaternion);
        }
    }
    
    private class Node {
        public List<Node> edges;
        public GOAPAction action;
        public bool visisted;
        public GameObject worker;

        public Node(List<Node> edges, GOAPAction action, bool visisted, GameObject worker) {
            this.edges = edges;
            this.action = action;
            this.visisted = visisted;
            this.worker = worker;
        }
    }
}

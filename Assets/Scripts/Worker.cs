using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : MonoBehaviour
{
    [SerializeField] public GameObject firePrefab;
    [SerializeField] public GameObject treePrefab;
    [SerializeField] public GameObject axePrefab;
    [SerializeField] public GameObject woodPilePrefab;
    
    // Coroutine to travel to destination with a fixed speed
    public IEnumerator MoveTo(Vector3 end)
    {
        var speed = 0.001f;
        while (Vector3.Distance(this.transform.position,end)>speed){
            this.transform.position = Vector3.MoveTowards(this.transform.position, end, speed);
            yield return 0;
        }
        this.transform.position = end;
    }

    // Query are game world and return the closest Object that contains passed in tag
    public GameObject FindCloest(string tag)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}

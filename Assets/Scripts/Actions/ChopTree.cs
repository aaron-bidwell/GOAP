using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class ChopTree : GOAPAction
{
    public override KeyValuePair<Goals, bool> getPrecondition()
    {
        return new KeyValuePair<Goals, bool>(Goals.has_axe, false);
    }
    
    public override KeyValuePair<Goals, bool> getEffects()
    {
        return new KeyValuePair<Goals, bool>(Goals.has_wood, true);
    }

    // Find the Nearest Tree and chop it down, than a new one will spawn
    public override IEnumerator useAction(GameObject gameObject)
    {
        var worker = gameObject.GetComponent<Worker>();
        var tree = worker.FindCloest("Tree");
        yield return worker.StartCoroutine(worker.MoveTo(tree.transform.position));
        yield return new WaitForSeconds(3f);
        Object.Destroy(tree);
        
        float spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - 10)).y);
        float spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - 10, 0)).x);
        Vector3 randomPosition = new Vector3(spawnX,spawnY,0);
        Quaternion quaternion = new Quaternion();
        GameObject.Instantiate(worker.treePrefab, randomPosition, quaternion);
    }

    public override float getCost(GameObject gameObject)
    {
        var worker = gameObject.GetComponent<Worker>();
        var tree = worker.FindCloest("Tree");
        return Vector2.Distance(worker.transform.position, tree.transform.position) / 5;
    }
    
    public override string getName()
    {
        return "Chop_Tree";
    }
}

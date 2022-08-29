using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class GrabWoodFromPile : GOAPAction
{
    public override KeyValuePair<Goals, bool> getPrecondition()
    {
        return new KeyValuePair<Goals, bool>(Goals.none, false);
    }
    
    public override KeyValuePair<Goals, bool> getEffects()
    {
        return new KeyValuePair<Goals, bool>(Goals.has_wood, true);
    }

    // Find the closest one, than a new one will spawn
    public override IEnumerator useAction(GameObject gameObject)
    {
        var worker = gameObject.GetComponent<Worker>();
        var woodPile = worker.FindCloest("WoodPile");
        woodPile = worker.FindCloest("WoodPile");
        yield return worker.StartCoroutine(worker.MoveTo(woodPile.transform.position));
        yield return new WaitForSeconds(3f);
        Object.Destroy(woodPile);
        float spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - 10)).y);
        float spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - 10, 0)).x);
        Vector3 randomPosition = new Vector3(spawnX,spawnY,0);
        Quaternion quaternion = new Quaternion();
        GameObject.Instantiate(worker.woodPilePrefab, randomPosition, quaternion);
    }

    public override float getCost(GameObject gameObject)
    {
        var worker = gameObject.GetComponent<Worker>();
        var woodPile = worker.FindCloest("WoodPile");
        return Vector2.Distance(worker.transform.position, woodPile.transform.position) / 5;
    }
    
    public override string getName()
    {
        return "Grab_Wood_From_Pile";
    }
}

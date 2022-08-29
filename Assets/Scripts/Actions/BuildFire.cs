using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

public class BuildFire : GOAPAction
{
    public override KeyValuePair<Goals, bool> getPrecondition()
    {
        return new KeyValuePair<Goals, bool>(Goals.has_wood, true);
    }
    
    public override KeyValuePair<Goals, bool> getEffects()
    {
        return new KeyValuePair<Goals, bool>(Goals.is_warm, true);
    }

    // Use your aquired wood to place a fire in a random spot on the screen
    public override IEnumerator useAction(GameObject gameObject)
    {
        var worker = gameObject.GetComponent<Worker>();
        float spawnY = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height - 10)).y);
        float spawnX = Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - 10, 0)).x);
        Vector3 randomPosition = new Vector3(spawnX,spawnY,0);
        Quaternion quaternion = new Quaternion();
        yield return worker.StartCoroutine(worker.MoveTo(randomPosition));
        yield return new WaitForSeconds(3f);
        GameObject.Instantiate(worker.firePrefab, randomPosition, quaternion);
    }

    public override float getCost(GameObject gameObject)
    {
        return 3;
    }

    public override string getName()
    {
        return "Build_Fire";
    }
}

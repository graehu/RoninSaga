using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    #region public
    public List<TeamManager> teamManagers;
    public float spawnInterval = 5;
    #endregion
    float spawnTick = 0;
	// Update is called once per frame
	void Update ()
    {
        spawnTick += Time.deltaTime;
        if (spawnTick > spawnInterval)
        {
            Debug.Log("Spawn!");
            for (int i = 0; i < teamManagers.Count; i++)
            {
                teamManagers[i].Spawn();
            }

            spawnTick = spawnInterval - spawnInterval;
        }
	}
}

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

    public float GetTeamMoral(TeamMember.Team _team)
    {
        float totalMoral = 0;
        for(int i = 0; i < TeamMember.TeamMembers.Count; i++)
        {
            totalMoral = TeamMember.TeamMembers[i].moral;
        }
        return totalMoral;
    }
	// Update is called once per frame
	void Update ()
    {
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

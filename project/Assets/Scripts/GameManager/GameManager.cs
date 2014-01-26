using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	#region static variables

	static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }

	#endregion

    #region public variables

    #endregion

	#region private variables

    float spawnTick = 0;

	#endregion


	#region public methods

	public void OnTeamMemberDeath(TeamMember _teamMember)
	{
		//have all teams spawn a new unit
		foreach (TeamManager teamManager in TeamManager.AllTeamManagers) 
		{
			teamManager.Spawn();
		}
	}

    public float GetTeamMoral(TeamMember.Team _team)
    {
        float totalMoral = 0;
        for(int i = 0; i < TeamMember.AllTeamMembers.Count; i++)
        {
			if(TeamMember.AllTeamMembers[i].teamColor == _team)
            	totalMoral = TeamMember.AllTeamMembers[i].moral;
        }
        return totalMoral;
    }

	#endregion

	#region monobehaviour methods

	void Awake()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(gameObject);
	}

	void Start()
	{
		//have all teams spawn a new unit
		foreach (TeamManager teamManager in TeamManager.AllTeamManagers) 
		{
			teamManager.SpawnInitial();
		}
	}

	// Update is called once per frame
	void Update ()
    {
        
	}

	#endregion
}

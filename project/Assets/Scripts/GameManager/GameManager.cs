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

	public SpriteRenderer moralSlider = null;
	public float moralSliderMaxOffset = 3;
	public float startDelay = 1;
	public float RelativeMoral { get { return relativeMoral; } }

	public TextMesh ticketTextRed = null;
	public TextMesh ticketTextBlue = null;

	public bool dirtyScore = false;

    #endregion

	#region private variables

	bool spawnedInitial = false;
    float tick = 0;

	float relativeMoral = 0;
	Vector3 moralVelocity = Vector3.zero;

	#endregion


	#region public methods

	public void OnTeamMemberDeath(TeamMember _teamMember)
	{
		//have all teams spawn a new unit
		foreach (TeamManager teamManager in TeamManager.AllTeamManagers) 
		{
			teamManager.Spawn();
		}

		dirtyScore = true;
	}

    public float GetTeamMoral(TeamMember.Team _team)
    {
        float totalMoral = 0;
        for(int i = 0; i < TeamMember.AllTeamMembers.Count; i++)
        {
			if(TeamMember.AllTeamMembers[i].teamColor == _team)
            	totalMoral += TeamMember.AllTeamMembers[i].moral;
        }
        return totalMoral;
    }

	public float GetTeamRelativeMoral(TeamMember.Team _team)
	{
		if(_team == TeamMember.Team.red && relativeMoral > 0)
			return relativeMoral;
		else if(_team == TeamMember.Team.blue && relativeMoral < 0)
			return -relativeMoral;
		else
			return 0;
	}

	#endregion

	#region priovate methods

	void UpdateUI()
	{
		if(dirtyScore)
		{
			dirtyScore = false;

			//calculate relative moral
			float blueMoral = GetTeamMoral(TeamMember.Team.blue);
			float redMoral = GetTeamMoral(TeamMember.Team.red);
			
			float totalMoral = blueMoral + redMoral;
			relativeMoral = (redMoral - blueMoral) / totalMoral;
			
			//update ticket counts
			foreach(TeamManager teamManager in TeamManager.AllTeamManagers)
			{
				if(teamManager.teamColor == TeamMember.Team.blue)
					ticketTextBlue.text = teamManager.MembersToSpawn.Count.ToString();
				if(teamManager.teamColor == TeamMember.Team.red)
					ticketTextRed.text = teamManager.MembersToSpawn.Count.ToString();
			}
		}

		//animate slider
		Vector3 sliderPos = moralSlider.transform.position;
		sliderPos.x = relativeMoral * moralSliderMaxOffset;
		moralSlider.transform.position = Vector3.SmoothDamp(moralSlider.transform.position, sliderPos, ref moralVelocity, 0.5f);
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

	}

	// Update is called once per frame
	void Update ()
    {
		tick += Time.deltaTime;
		if(tick > startDelay)
		{
			UpdateUI();

			if(!spawnedInitial)
			{
				dirtyScore = true;
				spawnedInitial = true;
				//have all teams spawn a new unit
				foreach (TeamManager teamManager in TeamManager.AllTeamManagers) 
				{
					teamManager.SpawnInitial();
				}
			}

			if(Input.GetKeyDown(KeyCode.Escape))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		
		


	}

	#endregion
}

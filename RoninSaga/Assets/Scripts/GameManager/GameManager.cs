using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
	#region static variables

	static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }

	#endregion

	#region public types

	public enum State
	{
		NONE,
		SPLASH,
		FIGHT,
		FIGHT_END,
		FINAL_STAGE,
		WIN,
		LOOSE,
	}

	#endregion

    #region public variables

	public SpriteRenderer moralSlider = null;
	public float moralSliderMaxOffset = 3;
	public float startDelay = 1;
	public float RelativeMoral { get { return relativeMoral; } }

	public TextMesh ticketTextRed = null;
	public TextMesh ticketTextBlue = null;

	public bool dirtyScore = false;

	public GameObject splashPopup = null;
	public GameObject winPopup = null;
	public GameObject loosePopup = null;
	public GameObject finalStagePopup = null;

    #endregion

	#region private variables

	State state = State.NONE;

	bool spawnedInitial = false;
    float tick = 0;

	float relativeMoral = 0;
	Vector3 moralVelocity = Vector3.zero;

	GameObject currentPopup;
	GameObject lastPopup;

	#endregion


	#region public methods

	public void OnTeamMemberDeath(TeamMember _teamMember)
	{
		//have all teams spawn a new unit
		foreach (TeamManager teamManager in TeamManager.AllTeamManagers) 
		{
            if(_teamMember.teamColor == teamManager.teamColor)
			{
				if(teamManager.MembersToSpawn.Count > 0)
			    	teamManager.Spawn();
				else
				{
					//FIXME: this is such a fucking hack
					int remainingMembers = 0;
					foreach(TeamMember member in TeamMember.AllTeamMembers)
					{
						if(member.teamColor == teamManager.teamColor)
						{
							bool isPlayer = false;
							foreach(PlayerController player in PlayerController.currentPlayers)
							{
								if(player.activeEntity == member)
								{
									isPlayer = true;
									break;
								}
							}
							if(!isPlayer)
								remainingMembers++;
						}
					}

					if(remainingMembers == 0)
						SetState(State.FIGHT_END);
				}
			}
		}

		dirtyScore = true;
	}

    public float GetTeamMoral(TeamMember.Team _team)
    {
        float totalMoral = 0;
        float memsToSpawn = 0;
        for(int i = 0; i < TeamMember.AllTeamMembers.Count; i++)
        {
            if (TeamMember.AllTeamMembers[i].teamColor == _team)
                totalMoral += TeamMember.AllTeamMembers[i].moral;
        }
        for(int i = 0; i < TeamManager.AllTeamManagers.Count; i++)
        {
            if (TeamManager.AllTeamManagers[i].teamColor == _team)
                memsToSpawn = TeamManager.AllTeamManagers[i].MembersToSpawn.Count;
        }

        return totalMoral + (5*memsToSpawn);
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

	#region private methods

	void UpdateUI()
	{
		if(dirtyScore)
		{
			dirtyScore = false;

			//calculate relative moral
			float blueMoral = GetTeamMoral(TeamMember.Team.blue);
			float redMoral = GetTeamMoral(TeamMember.Team.red);
			
			float totalMoral = blueMoral + redMoral;

			if(totalMoral != 0)
				relativeMoral = (redMoral - blueMoral) / totalMoral;
			else
				relativeMoral = 0;
			

			foreach(TeamManager teamManager in TeamManager.AllTeamManagers)
			{
				//update ticket counts
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

	void SetState(State _newState)
	{
		if(state == _newState)
			return;

		tick = 0;

		lastPopup = currentPopup;

		switch(_newState)
		{
		case State.SPLASH:
		{
			currentPopup = splashPopup;
			break;
		}
		case State.FIGHT:
		{
			currentPopup = null;
			break;
		}
		case State.FIGHT_END:
		{
			foreach(PlayerController player in PlayerController.currentPlayers)
			{
				player.activeEntity.teamColor = TeamMember.Team.none;
			}
			currentPopup = finalStagePopup;
			break;
		}
		case State.FINAL_STAGE:
		{
			currentPopup = null;
			break;
		}
		case State.WIN:
		{
			currentPopup = winPopup;
			break;
		}
		case State.LOOSE:
		{
			currentPopup = loosePopup;
			break;
		}
		}

		if(currentPopup)
			currentPopup.SetActive(true);
		if(lastPopup)
			lastPopup.SetActive(false);

		state = _newState;

		Debug.Log("Game state: " + state.ToString());
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
		SetState(State.SPLASH);
	}

	// Update is called once per frame
	void Update ()
    {
		bool reset = false;
		tick += Time.deltaTime;

		switch(state)
		{
		case State.SPLASH:
		{
			if(Input.anyKeyDown && tick > 0.5f)
			{
				SetState(State.FIGHT);
			}
			break;
		}
		case State.FIGHT:
		{
			if(tick > startDelay)
			{
				//if player is dead
				if(PlayerController.currentPlayers.Count == 0)
				{
					SetState(State.LOOSE);
				}

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

				UpdateUI();
			}
			break;
		}
		case State.FIGHT_END:
		{
			if(tick > 2.0f)
			{
				SetState(State.FINAL_STAGE);
			}

			UpdateUI();
			
			//if player is dead
			if(PlayerController.currentPlayers.Count == 0)
			{
				SetState(State.LOOSE);
			}
			//if player is only member standing
			else if(TeamMember.AllTeamMembers.Count == 1)
			{
				SetState(State.WIN);
			}
			break;
		}
		case State.FINAL_STAGE:
		{
			UpdateUI();

			//if player is dead
			if(PlayerController.currentPlayers.Count == 0)
			{
				SetState(State.LOOSE);
			}
			//if player is only member standing
			else if(TeamMember.AllTeamMembers.Count == 1)
			{
				SetState(State.WIN);
			}
			break;
		}
		case State.WIN:
		{
			reset = Input.anyKeyDown && tick > 0.5f;
			break;
		}
		case State.LOOSE:
		{
			reset = Input.anyKeyDown && tick > 0.5f;
			break;
		}
		}

		if(reset || Input.GetKeyDown(KeyCode.Escape))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	#endregion
}

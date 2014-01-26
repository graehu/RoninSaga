using UnityEngine;
using System.Collections.Generic;

//the player controller controls an eneity via input from the user

public class PlayerController : MonoBehaviour 
{

	#region static variables

	public static List<PlayerController> currentPlayers = new List<PlayerController>();

	#endregion

    #region public variables

	public TeamMember activeEntity = null;
	public TeamMember whiteEntity = null;
	public TeamMember blackEntity = null;
	public List<GameObject> spawnOnTeamChange = new List<GameObject>();

	public bool inputLocked = false;

	public float meleeCooldown = 0.5f;
	public float magicCooldown = 0.3f;
	public float teamChangeCooldown = 0.0f;

	public float magicChargeTime = 1.0f;

    #endregion

    #region private variables

	float meleeCooldownRemaining = 0;
	float magicCooldownRemaining = 0;
	float teamChangeCooldownRemaining = 0;

    #endregion

    #region public methods

    #endregion

    #region private methods

    void TryMove()
    {
        //Handle input 
        Vector2 dir = Vector2.zero;
        if(Input.GetKey(KeyCode.W))
        {
            dir.y = 1;
        }
        else if (Input.GetKey (KeyCode.S)) 
        {
            dir.y = -1;
        }
        if (Input.GetKey (KeyCode.A))
        {
            dir.x = -1;
        }
        else if (Input.GetKey (KeyCode.D))
        {
            dir.x = 1;
        }
        
        activeEntity.TryMove(dir);
    }

    void TryLook()
    {

	        Vector3 botScreenPos = Camera.main.WorldToScreenPoint(activeEntity.transform.position);
	        activeEntity.TryLook(Input.mousePosition - botScreenPos);
	
    }

    void TryAction()
    {
		if (Input.GetMouseButtonDown(0) && meleeCooldownRemaining <= 0)
        {
			meleeCooldownRemaining += meleeCooldown;
			activeEntity.TryMeleeAttack();
        }
		else if (Input.GetMouseButtonDown(1) && magicCooldownRemaining <= 0)
		{
			magicCooldownRemaining += magicCooldown;
			activeEntity.TryCastMagic();
		}
		else if(Input.GetKeyDown(KeyCode.Space) && teamChangeCooldownRemaining <= 0 && activeEntity.teamColor != TeamMember.Team.none)
		{
			teamChangeCooldownRemaining += teamChangeCooldown;
			//TODO: better support for multiple teams
			if(activeEntity.teamColor == TeamMember.Team.white)
			{
				OnTeamChange(TeamMember.Team.black);
			}
			else if(activeEntity.teamColor == TeamMember.Team.black)
			{
				OnTeamChange(TeamMember.Team.white);
			}
		}
    }

	void OnTeamChange(TeamMember.Team _team)
	{
		TeamMember previousEntity = activeEntity;

		if(_team == TeamMember.Team.white)
		{
			blackEntity.gameObject.SetActive(false);
			activeEntity = whiteEntity;
		}
		else if(_team == TeamMember.Team.black)
		{
			whiteEntity.gameObject.SetActive(false);
			activeEntity = blackEntity;
		}

		activeEntity.gameObject.SetActive(true);
		activeEntity.transform.position = previousEntity.transform.position;

		foreach(GameObject gobj in spawnOnTeamChange)
		{
			GameObject spawned = Instantiate(gobj) as GameObject;
			spawned.transform.position = activeEntity.transform.position;
		}
	}

    #endregion

    #region monobehaviour methods

	// Use this for initialization
	void Awake () 
    {
		currentPlayers.Add(this);

		OnTeamChange(activeEntity.teamColor);
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (activeEntity)
        {
			if(!inputLocked)
			{
                TryMove();
                TryLook();
                TryAction();
			}
        }
		meleeCooldownRemaining = Mathf.Max(0, meleeCooldownRemaining - Time.deltaTime);
		magicCooldownRemaining = Mathf.Max(0, magicCooldownRemaining - Time.deltaTime);
		teamChangeCooldownRemaining = Mathf.Max(0, teamChangeCooldownRemaining - Time.deltaTime);

		//FIXME: DEBUG RESET
		if(Input.GetKeyDown(KeyCode.R))
		{
			Application.LoadLevel(Application.loadedLevel);
		}
	}

    void OnDisable()
    {
        activeEntity.TryMove(Vector2.zero);
    }

	void OnDestroy()
	{
		currentPlayers.Remove(this);
	}

    #endregion
}

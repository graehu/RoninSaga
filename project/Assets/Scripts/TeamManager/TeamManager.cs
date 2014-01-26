using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamManager : MonoBehaviour
{
	#region static variables

	public static List<TeamManager> AllTeamManagers = new List<TeamManager>();

	#endregion
	
    #region public variables

    public TeamMember.Team teamColor = TeamMember.Team.red;

    //
    public List<TeamMember> members = new List<TeamMember>();
    public List<Transform> spawnPoints = new List<Transform>();
    //
    public List<TeamMember> initialMembers = new List<TeamMember>();
    public List<Transform> initialPoints = new List<Transform>();

	public Stack<TeamMember> MembersToSpawn { get { return membersToSpawn; } }

	#endregion

	#region private variables
	
	private Stack<TeamMember> membersToSpawn = new Stack<TeamMember>();

	#endregion

	#region public methods

	//spawns the initial set of members randomly accross the initial spawn points
	public void SpawnInitial()
	{
		Stack<TeamMember> memberSet = new Stack<TeamMember>(initialMembers);

		SortHelper.ListShuffle(initialPoints);
		Stack<Transform> spawnPointSet = new Stack<Transform>(initialPoints);

		while(memberSet.Count > 0)
		{
			TeamMember memberToSpawn = memberSet.Pop();
			Transform spawnPoint = spawnPointSet.Pop();
			SpawnMember(memberToSpawn, spawnPoint);
		}   
	}

	//spawns one member at a random spawn point
    public void Spawn()
    {
		if(membersToSpawn.Count > 0)
		{
			TeamMember memberToSpawn = membersToSpawn.Pop();
			Transform spawnPoint = initialPoints[Random.Range(0, initialPoints.Count)];
			SpawnMember(memberToSpawn, spawnPoint);
		}
    }

    #endregion

	#region protected methods

	void SpawnMember(TeamMember _memberPrefab, Transform spawnPoint)
	{
		TeamMember inst = GameObject.Instantiate(_memberPrefab) as TeamMember;
		inst.manager = this;
		inst.teamColor = teamColor;
		inst.transform.parent = this.transform;
		inst.transform.position = spawnPoint.transform.position;
	}

	#endregion

    void Awake()
    {
		//shuffle members
		SortHelper.ListShuffle(initialMembers);
		SortHelper.ListShuffle(members);
		membersToSpawn = new Stack<TeamMember>(members);

		AllTeamManagers.Add(this);
    }

	void OnDestroy()
	{
		AllTeamManagers.Remove(this);
	}

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamMember : Killable
{
    #region public

    public enum Team
    {
        white,
        green,
        blue,
        red,
        yellow,
        black,
    }

    public Team teamColor = Team.white;
    public static List<TeamMember> TeamMembers = new List<TeamMember>();

    #endregion

    void OnEnable()
    {
        TeamMembers.Add(this);
    }

    void OnDisable()
    {
        TeamMembers.Remove(this);
    }

    // Update is called once per frame
	void Update () {
	
	}
}

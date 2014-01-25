using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamMember : CombatEntity
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

    [HideInInspector]
    public TeamManager manager = null;
    public Team teamColor = Team.white;
    public static List<TeamMember> TeamMembers = new List<TeamMember>();
    public float moral = 10.0f;
	
	public override bool GetClosestEnemy(out Killable _target, out float _distance)
	{
		_target = null;
		_distance = 0;

		float minDist = float.MaxValue;
		foreach (TeamMember teamMember in TeamMembers) 
		{
			if(teamMember.teamColor != this.teamColor)
			{
				float dist = Vector3.Distance(teamMember.transform.position, this.transform.position);
				if(dist < minDist)
				{
					_target = teamMember;
					minDist = dist;
				}
			}
		}
		_distance = minDist;

		return _target != null;
	}

	public override bool CanDamage (Killable _killable)
	{
		bool r = base.CanDamage (_killable);

		//if base check is valid and is not on the same team return true
		TeamMember teamMember = _killable as TeamMember;
		return r && teamMember && teamMember.teamColor != this.teamColor;
	}
	
    #endregion

    void OnEnable()
    {
		base.OnEnable ();
        TeamMembers.Add(this);
    }

    void OnDisable()
    {
		base.OnDisable ();
        TeamMembers.Remove(this);
    }
}

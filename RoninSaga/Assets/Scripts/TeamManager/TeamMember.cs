using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeamMember : CombatEntity
{
    #region public

    public enum Team
    {
        red,
        blue,
		none
    }

    [HideInInspector]
    public TeamManager manager = null;
    public Team teamColor = Team.red;
    public static List<TeamMember> AllTeamMembers = new List<TeamMember>();
    public float moral = 10.0f;
	public float MoralScale { get { return actualMoralScale; } }

	float actualMoralScale = 0;
	float moralScaleVelocity = 0;
	
	public override bool GetClosestEnemy(out Killable _target, out float _distance)
	{
		_target = null;
		_distance = 0;

		float minDist = float.MaxValue;
		foreach (TeamMember teamMember in AllTeamMembers) 
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
		return r && teamMember && (teamMember.teamColor != this.teamColor || teamMember.teamColor == Team.none || this.teamColor == Team.none);
	}

	public override void OnDeath ()
	{
		AllTeamMembers.Remove(this);
		GameManager.Instance.OnTeamMemberDeath (this);
		base.OnDeath ();
	}
	
    #endregion

	void Update()
	{
		base.Update();

		float moralScale = GameManager.Instance.GetTeamRelativeMoral(teamColor);
		actualMoralScale = Mathf.SmoothDamp(actualMoralScale, moralScale, ref moralScaleVelocity, 0.5f);

		/*float physicalScale = 1 + actualMoralScale;
		physicalScale = Mathf.Min(1.6f, physicalScale * physicalScale);
		transform.localScale = new Vector3(physicalScale,physicalScale,physicalScale);*/

		damageScale = 1.1f + actualMoralScale * 3f;
	}

    void OnEnable()
    {
		base.OnEnable ();
        AllTeamMembers.Add(this);
    }

    void OnDisable()
    {
		base.OnDisable ();
        AllTeamMembers.Remove(this);
    }
}

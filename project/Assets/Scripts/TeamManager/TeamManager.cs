using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamManager : MonoBehaviour
{
    #region public
    TeamMember.Team color = TeamMember.Team.white;

    //
    public List<TeamMember> members;
    public List<Transform> spawnPoints;
    //
    public List<TeamMember> initialMembers;
    public List<Transform> initialPoints;


    #endregion
    //Shuffled indexs used for member spawning;
    private int[] memIndices;

    void Awake()
    {
        //Spawn Points
        int[] memShuffle = SortHelper.IndexShuffle(0, initialMembers.Count);
        int[] pointShuffle = SortHelper.IndexShuffle(0, initialPoints.Count);
        memIndices = SortHelper.IndexShuffle(0, members.Count);

        for (int i = 0; i < memShuffle.Length; i++)
        {
            int mem = memShuffle[i];
            int point = pointShuffle[mem];
            TeamMember m = initialMembers[mem];
            Transform t = initialPoints[point];
            TeamMember inst = GameObject.Instantiate(m) as TeamMember;
            inst.transform.localPosition = t.localPosition;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

}

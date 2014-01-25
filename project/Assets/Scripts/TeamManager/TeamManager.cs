using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TeamManager : MonoBehaviour
{
    #region public
    public TeamMember.Team teamColor = TeamMember.Team.white;

    //
    public List<TeamMember> members;
    public List<Transform> spawnPoints;
    //
    public List<TeamMember> initialMembers;
    public List<Transform> initialPoints;

    public void Spawn()
    {
        if (currentSpawn >= spawnPoints.Count || currentMem >= members.Count)
            return;

        int member = memIndices[currentMem];
        int point = spawnIndices[currentSpawn];
        TeamMember inst = GameObject.Instantiate(members[member]) as TeamMember;
        inst.teamColor = teamColor;
        inst.transform.parent = this.transform;
        inst.transform.localPosition = spawnPoints[point].transform.localPosition;
        currentMem++;
        currentSpawn++;
    }

    #endregion
    //Shuffled indexs used for member spawning;
    private int[] memIndices;
    private int[] spawnIndices;
    private int currentMem = 0;
    private int currentSpawn = 0;

    void Awake()
    {
        //Spawn Points
        int[] memShuffle = SortHelper.IndexShuffle(0, initialMembers.Count);
        int[] pointShuffle = SortHelper.IndexShuffle(0, initialPoints.Count);
        memIndices = SortHelper.IndexShuffle(0, members.Count);
        spawnIndices = SortHelper.IndexShuffle(0, spawnPoints.Count);

        for (int i = 0; i < memShuffle.Length; i++)
        {
            int mem = memShuffle[i];
            int point = pointShuffle[mem];
            TeamMember m = initialMembers[mem];
            Transform t = initialPoints[point];
            TeamMember inst = GameObject.Instantiate(m) as TeamMember;
            inst.transform.parent = this.transform;
            inst.teamColor = teamColor;
            inst.transform.localPosition = t.localPosition;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}

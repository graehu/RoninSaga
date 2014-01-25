using UnityEngine;
using System.Collections;

public class RoomHelper : MonoBehaviour
{
	static Rect roomArea = new Rect(-3,-3,6,6);

	#region public static methods

	public static Vector2 RandomPosition()
	{
		//FIXME: Hardcoded area
		return new Vector2(Random.Range(roomArea.xMin,roomArea.xMax),Random.Range(roomArea.yMin,roomArea.yMax));
	}

	public static bool IsInRoom(Vector2 _pos)
	{
		//FIXME: Hardcoded area
		return roomArea.Contains(_pos);
	}

	public static Vector2 BoundToRoom(Vector2 _pos)
	{
		_pos.x = Mathf.Clamp(_pos.x, roomArea.xMin, roomArea.xMax);
		_pos.y = Mathf.Clamp(_pos.y, roomArea.yMin, roomArea.yMax);
		return _pos;
	}

	#endregion
}

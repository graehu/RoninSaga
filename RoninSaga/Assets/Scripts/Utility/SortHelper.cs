using UnityEngine;
using System.Collections;

public class SortHelper
{
    //Passes back a shuffled index array from min to max. E.g. 1-5 = 2,1,4,5,3
    static public int[] IndexShuffle(int min, int max)
    {
        if (min >= max)
        {
            Debug.LogError("Min should not be greater than or equal to max: Min: " + min + ", Max:" + max);
			return null;
        }
        int size = max - min;
        int[] shuffle = new int[size];
        for (int i = 0; i < size; i++)
            shuffle[i] = min + i;

        for (int i = size - 1; i >= 0; i--)
        {
            int j = Random.Range(0, i);
            int swap = shuffle[i];
            shuffle[i] = shuffle[j];
            shuffle[j] = swap;
        }
        return shuffle;
    }

	static public void ListShuffle(IList _list)
	{		
		for (int i = _list.Count - 1; i >= 0; i--)
		{
			int j = Random.Range(0, i);
			object swap = _list[i];
			_list[i] = _list[j];
			_list[j] = swap;
		}
	}
}

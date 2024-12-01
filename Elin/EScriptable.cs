using UnityEngine;

public class EScriptable : ScriptableObject
{
	public static int rnd(int a)
	{
		return Rand.rnd(a);
	}
}

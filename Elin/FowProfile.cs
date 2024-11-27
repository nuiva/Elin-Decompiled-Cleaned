using System;
using UnityEngine;

public class FowProfile : ScriptableObject
{
	public static FowProfile Load(string id)
	{
		return ResourceCache.Load<FowProfile>("Scene/Profile/Fow/FowProfile_" + id);
	}

	public FOWType type;

	public Color color;
}

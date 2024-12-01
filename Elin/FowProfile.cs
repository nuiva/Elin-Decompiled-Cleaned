using UnityEngine;

public class FowProfile : ScriptableObject
{
	public FOWType type;

	public Color color;

	public static FowProfile Load(string id)
	{
		return ResourceCache.Load<FowProfile>("Scene/Profile/Fow/FowProfile_" + id);
	}
}

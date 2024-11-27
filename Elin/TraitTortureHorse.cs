using System;
using UnityEngine;

public class TraitTortureHorse : TraitShackle
{
	public override Vector3 GetRestrainPos
	{
		get
		{
			return default(Vector3);
		}
	}

	public override AnimeID animeId
	{
		get
		{
			return AnimeID.JumpSmall;
		}
	}
}

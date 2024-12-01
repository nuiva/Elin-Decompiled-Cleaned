using UnityEngine;

public class TraitTortureHorse : TraitShackle
{
	public override Vector3 GetRestrainPos => default(Vector3);

	public override AnimeID animeId => AnimeID.JumpSmall;
}

using UnityEngine;

public class TraitGallows : TraitShackle
{
	public override Vector3 GetRestrainPos => EClass.setting.render.posGallows;
}

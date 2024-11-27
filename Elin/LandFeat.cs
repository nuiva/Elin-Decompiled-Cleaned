using System;
using UnityEngine;

public class LandFeat : Skill
{
	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_Feat");
	}
}

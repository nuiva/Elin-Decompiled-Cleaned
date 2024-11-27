using System;
using UnityEngine;

public class Domain : EClass
{
	public Sprite GetSprite()
	{
		string str = this.source.alias.Remove(0, 3).ToLower();
		return ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/" + str);
	}

	public SourceElement.Row source;
}

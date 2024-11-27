using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderDataThing : RenderDataCard
{
	public override string prefabName
	{
		get
		{
			return "ThingActor";
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public List<Sprite> sprites;

	public float frameTime;
}

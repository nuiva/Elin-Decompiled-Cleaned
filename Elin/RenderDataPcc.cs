using System;
using UnityEngine;

public class RenderDataPcc : RenderDataChara
{
	public override string prefabName
	{
		get
		{
			return "CharaActorPCC";
		}
	}

	private void OnValidate()
	{
		this._offset = this.offset;
	}

	public Vector3 _scale;
}

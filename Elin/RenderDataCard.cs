using System;
using UnityEngine;

public class RenderDataCard : RenderData
{
	public override CardActor CreateActor()
	{
		if (!this.mold && base.name == "pcc")
		{
			this.mold = Resources.Load<CharaActorPCC>("Scene/Render/Actor/CharaActorPCC");
		}
		if (this.mold == null)
		{
			return null;
		}
		return PoolManager.Spawn<CardActor>(this.mold, null);
	}

	public CardActor mold;
}

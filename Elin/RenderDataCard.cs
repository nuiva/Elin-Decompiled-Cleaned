using UnityEngine;

public class RenderDataCard : RenderData
{
	public CardActor mold;

	public override CardActor CreateActor()
	{
		if (!mold && base.name == "pcc")
		{
			mold = Resources.Load<CharaActorPCC>("Scene/Render/Actor/CharaActorPCC");
		}
		if (mold == null)
		{
			return null;
		}
		return PoolManager.Spawn(mold);
	}
}

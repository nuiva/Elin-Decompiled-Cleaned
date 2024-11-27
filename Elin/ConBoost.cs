using System;

public class ConBoost : BaseBuff
{
	public override void OnStart()
	{
		this.owner._CreateRenderer();
	}

	public override void OnRemoved()
	{
		this.owner._CreateRenderer();
	}

	public override RendererReplacer GetRendererReplacer()
	{
		string id = this.owner.id;
		if (id == "black_angel")
		{
			return RendererReplacer.CreateFrom("black_angel", -1);
		}
		if (!(id == "adv_verna"))
		{
			return null;
		}
		return RendererReplacer.CreateFrom("adv_verna", 2);
	}
}

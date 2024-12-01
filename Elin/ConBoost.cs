public class ConBoost : BaseBuff
{
	public override void OnStart()
	{
		owner._CreateRenderer();
	}

	public override void OnRemoved()
	{
		owner._CreateRenderer();
	}

	public override RendererReplacer GetRendererReplacer()
	{
		string text = owner.id;
		if (!(text == "black_angel"))
		{
			if (text == "adv_verna")
			{
				return RendererReplacer.CreateFrom("adv_verna", 2);
			}
			return null;
		}
		return RendererReplacer.CreateFrom("black_angel", -1);
	}
}

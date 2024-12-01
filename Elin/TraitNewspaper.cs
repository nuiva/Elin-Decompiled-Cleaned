public class TraitNewspaper : TraitScroll
{
	public override void OnRead(Chara c)
	{
		EClass.ui.AddLayer<LayerNewspaper>();
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!p.altAction && CanRead(EClass.pc))
		{
			p.TrySetAct("invRead", delegate
			{
				EClass.ui.AddLayer<LayerNewspaper>();
				return false;
			}, owner);
		}
	}
}

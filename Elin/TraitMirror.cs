public class TraitMirror : Trait
{
	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			TrySetAct(p);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actDress", delegate
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(EClass.pc, UIPCC.Mode.Body);
			return false;
		}, owner);
		p.TrySetAct("actDress2", delegate
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(EClass.pc, UIPCC.Mode.Extra);
			return false;
		}, owner);
	}
}

using System;

public class TraitMirror : Trait
{
	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			this.TrySetAct(p);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actDress", delegate()
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(EClass.pc, UIPCC.Mode.Body, null, null);
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actDress2", delegate()
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(EClass.pc, UIPCC.Mode.Extra, null, null);
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}

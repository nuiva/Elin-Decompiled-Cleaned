using System;

public class TraitQuestBoard : TraitBoard
{
	public override int GuidePriotiy
	{
		get
		{
			return 1;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actReadBoard", () => EClass.ui.AddLayer<LayerQuestBoard>(), this.owner, null, 1, false, true, false);
	}
}

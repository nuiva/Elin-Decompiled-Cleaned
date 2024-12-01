public class TraitResidentBoard : TraitBoard
{
	public override bool IsHomeItem => true;

	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone.IsPCFaction)
		{
			p.TrySetAct("LayerPeople", delegate
			{
				EClass.ui.ToggleLayer<LayerPeople>();
				Tutorial.Play("work");
				return false;
			}, owner);
		}
	}
}

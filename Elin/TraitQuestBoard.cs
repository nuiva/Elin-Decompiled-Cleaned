public class TraitQuestBoard : TraitBoard
{
	public override int GuidePriotiy => 1;

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actReadBoard", () => EClass.ui.AddLayer<LayerQuestBoard>(), owner);
	}
}

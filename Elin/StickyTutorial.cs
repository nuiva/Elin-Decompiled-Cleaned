public class StickyTutorial : BaseSticky
{
	public override int idIcon => 1;

	public override string idLang => "sticky_tutorial";

	public override void OnClick()
	{
		if (EClass.player.tutorialStep == 0)
		{
			LayerDrama.ActivateNerun("NerunFirst");
		}
		else
		{
			LayerHelp.Toggle("tutorial", "1");
		}
	}
}

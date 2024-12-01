public class StickyWelcome : BaseSticky
{
	public override int idIcon => 1;

	public override string idLang => "sticky_welcome";

	public override bool ShouldShow => !EClass.player.flags.welcome;

	public override bool RemoveOnClick => true;

	public override bool Removable => true;

	public override bool animate => false;

	public override void OnClick()
	{
		LayerHelp.Toggle("welcome");
		EClass.player.flags.welcome = true;
	}
}

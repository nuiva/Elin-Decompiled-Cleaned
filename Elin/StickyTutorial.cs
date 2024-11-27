using System;

public class StickyTutorial : BaseSticky
{
	public override int idIcon
	{
		get
		{
			return 1;
		}
	}

	public override string idLang
	{
		get
		{
			return "sticky_tutorial";
		}
	}

	public override void OnClick()
	{
		if (EClass.player.tutorialStep == 0)
		{
			LayerDrama.ActivateNerun("NerunFirst");
			return;
		}
		LayerHelp.Toggle("tutorial", "1");
	}
}

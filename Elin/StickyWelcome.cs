using System;

public class StickyWelcome : BaseSticky
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
			return "sticky_welcome";
		}
	}

	public override bool ShouldShow
	{
		get
		{
			return !EClass.player.flags.welcome;
		}
	}

	public override bool RemoveOnClick
	{
		get
		{
			return true;
		}
	}

	public override bool Removable
	{
		get
		{
			return true;
		}
	}

	public override bool animate
	{
		get
		{
			return false;
		}
	}

	public override void OnClick()
	{
		LayerHelp.Toggle("welcome", null);
		EClass.player.flags.welcome = true;
	}
}

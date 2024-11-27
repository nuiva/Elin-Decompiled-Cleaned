using System;

public class StickyQuest : BaseSticky
{
	public override string idLang
	{
		get
		{
			return "sticky_quest";
		}
	}

	public override int idIcon
	{
		get
		{
			return 3;
		}
	}

	public override bool Removable
	{
		get
		{
			return true;
		}
	}
}

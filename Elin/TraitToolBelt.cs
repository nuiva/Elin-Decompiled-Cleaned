using System;

public class TraitToolBelt : TraitContainer
{
	public override bool CanOpenContainer
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override void Prespawn(int lv)
	{
		this.owner.AddCard(ThingGen.Create("purse", -1, -1));
	}
}

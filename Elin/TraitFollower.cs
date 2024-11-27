using System;

public class TraitFollower : TraitChara
{
	public override void OnCreate(int lv)
	{
		base.owner.SetFaith(EClass.game.religions.dictAll.RandomItem<string, Religion>());
	}

	public override void SetName(ref string s)
	{
		s = "_of".lang(base.owner.faith.Name, s, null, null, null);
	}
}

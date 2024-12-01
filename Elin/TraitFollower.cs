public class TraitFollower : TraitChara
{
	public override void OnCreate(int lv)
	{
		base.owner.SetFaith(EClass.game.religions.dictAll.RandomItem());
	}

	public override void SetName(ref string s)
	{
		s = "_of".lang(base.owner.faith.Name, s);
	}
}

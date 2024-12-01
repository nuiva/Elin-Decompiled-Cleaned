public class TraitFirework : TraitEffect
{
	public override string Path => "Firework/" + GetID().IsEmpty("bees");

	public string GetID()
	{
		return EClass.core.refs.fireworks[base.id % EClass.core.refs.fireworks.Count].name;
	}

	public override void OnCreate(int lv)
	{
		base.id = EClass.rnd(EClass.core.refs.fireworks.Count);
	}

	public override bool OnUse(Chara c)
	{
		Proc();
		owner.ModNum(-1);
		return base.OnUse(c);
	}

	public override void SetName(ref string s)
	{
		s = "_firework".lang(s, (base.id + 1).ToString() ?? "");
	}
}

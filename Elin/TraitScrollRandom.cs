public class TraitScrollRandom : TraitScrollStatic
{
	public static ElementSelecter selector = new ElementSelecter
	{
		type = "S",
		lvMod = 20
	};

	public override EffectId idEffect => source.proc[0].ToEnum<EffectId>();

	public override int Power => 200;

	public override string AliasEle => source.aliasRef;

	public override string N1 => source.proc.TryGet(1, returnNull: true);

	public override SourceElement.Row source => EClass.sources.elements.map[owner.refVal];

	public override string GetName()
	{
		return Lang.TryGet("scroll_" + source.alias) ?? "scroll_".lang(source.GetName().ToLower());
	}

	public override void OnCreate(int lv)
	{
		owner.refVal = selector.Select(lv);
	}
}

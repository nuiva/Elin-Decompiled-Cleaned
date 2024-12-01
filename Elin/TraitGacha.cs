public class TraitGacha : Trait
{
	public enum GachaType
	{
		Junk,
		Plant,
		Furniture
	}

	public GachaType type => GetParam(1).ToEnum<GachaType>();

	public virtual string suffixCoin => "";

	public virtual int refVal => 0;

	public string GetIdCoin()
	{
		string text = "gacha_coin";
		switch (type)
		{
		case GachaType.Furniture:
			text += "_gold";
			break;
		case GachaType.Plant:
			text += "_silver";
			break;
		}
		return text;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (owner.isOn)
		{
			p.TrySetAct("gacha", delegate
			{
				LayerDragGrid.CreateGacha(this);
				return false;
			}, owner);
		}
	}

	public void PlayGacha(int num)
	{
		Thing thing = ThingGen.Create("gachaBall").SetNum(num);
		thing.refVal = (int)type;
		EClass.player.DropReward(thing, silent: true);
	}
}

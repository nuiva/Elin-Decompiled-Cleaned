using System;

public class TraitGacha : Trait
{
	public TraitGacha.GachaType type
	{
		get
		{
			return base.GetParam(1, null).ToEnum(true);
		}
	}

	public string GetIdCoin()
	{
		string text = "gacha_coin";
		TraitGacha.GachaType type = this.type;
		if (type != TraitGacha.GachaType.Plant)
		{
			if (type == TraitGacha.GachaType.Furniture)
			{
				text += "_gold";
			}
		}
		else
		{
			text += "_silver";
		}
		return text;
	}

	public virtual string suffixCoin
	{
		get
		{
			return "";
		}
	}

	public virtual int refVal
	{
		get
		{
			return 0;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!this.owner.isOn)
		{
			return;
		}
		p.TrySetAct("gacha", delegate()
		{
			LayerDragGrid.CreateGacha(this);
			return false;
		}, this.owner, null, 1, false, true, false);
	}

	public void PlayGacha(int num)
	{
		Thing thing = ThingGen.Create("gachaBall", -1, -1).SetNum(num);
		thing.refVal = (int)this.type;
		EClass.player.DropReward(thing, true);
	}

	public enum GachaType
	{
		Junk,
		Plant,
		Furniture
	}
}

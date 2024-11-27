using System;

public class TraitDye : Trait
{
	public override bool IsBlendBase
	{
		get
		{
			return true;
		}
	}

	public override bool CanUse(Chara c, Card tg)
	{
		return tg.isThing && c.Dist(tg) <= 1 && this.CanBlend(tg.Thing);
	}

	public override bool OnUse(Chara c, Card tg)
	{
		this.Dye(tg);
		return true;
	}

	public override void OnThrowGround(Chara c, Point p)
	{
		if (p.HasObj)
		{
			p.cell.objMat = (byte)this.owner.material.id;
			p.cell.isObjDyed = true;
			this.owner.Die(null, null, AttackSource.None);
		}
	}

	public override bool CanBlend(Thing t)
	{
		return t.id != "dye";
	}

	public override void OnBlend(Thing t, Chara c)
	{
		this.Dye(t);
	}

	public void Dye(Card tg)
	{
		tg.Dye(this.owner.material);
		Msg.Say("dye", tg, null, null, null);
		EClass.pc.PlaySound("water_farm", 1f, true);
		this.owner.ModNum(-1, true);
	}
}

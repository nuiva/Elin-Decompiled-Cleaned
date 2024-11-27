using System;

public class TraitMonsterBall : Trait
{
	public Chara chara
	{
		get
		{
			return this.owner.GetObj<Chara>(8);
		}
		set
		{
			this.owner.SetObj(8, value);
		}
	}

	public override bool IsThrowMainAction
	{
		get
		{
			return true;
		}
	}

	public override ThrowType ThrowType
	{
		get
		{
			return ThrowType.MonsterBall;
		}
	}

	public override EffectDead EffectDead
	{
		get
		{
			return EffectDead.None;
		}
	}

	public virtual bool IsLittleBall
	{
		get
		{
			return false;
		}
	}

	public override void OnCreate(int lv)
	{
		if (!this.IsLittleBall)
		{
			this.owner.SetLv(1 + EClass.rnd(lv + 10));
		}
	}

	public override bool CanStackTo(Thing to)
	{
		return to.GetObj<Chara>(8) == null && this.chara == null && to.LV == this.owner.LV;
	}

	public override void SetName(ref string s)
	{
		s = (this.IsLittleBall ? "_littleBall" : "_monsterball").lang(s, this.owner.LV.ToString() ?? "", (this.chara == null) ? "_monsterball_empty".lang() : this.chara.Name, null, null);
	}

	public override int GetValue()
	{
		if (this.IsLittleBall)
		{
			return base.GetValue();
		}
		return base.GetValue() * (100 + this.owner.LV * 15) / 100;
	}
}

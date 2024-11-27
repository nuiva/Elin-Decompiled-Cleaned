using System;

public class ConSuffocation : Condition
{
	public override bool PreventRegen
	{
		get
		{
			return true;
		}
	}

	public override int GetPhase()
	{
		if (base.value < 50)
		{
			return 0;
		}
		if (base.value >= 100)
		{
			return 2;
		}
		return 1;
	}

	public override void OnStart()
	{
		if (this.owner.IsPC)
		{
			Tutorial.Reserve("water", delegate
			{
				EClass.player.haltMove = true;
				EInput.Consume(true, 1);
			});
		}
	}

	public override void Tick()
	{
		if (this.owner.HasElement(429, 1))
		{
			base.Kill(false);
			return;
		}
		if (base.value >= 200)
		{
			base.value = 200;
		}
		if (base.value >= 100 && !EClass._zone.IsRegion)
		{
			this.owner.DamageHP(10 + this.owner.MaxHP / 20, AttackSource.Condition, null);
		}
		if (this.owner == null || !this.owner.IsAliveInCurrentZone)
		{
			return;
		}
		if (!this.owner.Cell.CanSuffocate())
		{
			base.Mod(-20, false);
			return;
		}
		if (!EClass._zone.IsRegion)
		{
			this.owner.ModExp(200, 4 + this.phase * 4);
		}
	}
}

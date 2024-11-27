using System;

public class TraitChestPractice : TraitContainer
{
	public override int ChanceLock
	{
		get
		{
			return 100;
		}
	}

	public override void Prespawn(int lv)
	{
	}

	public override void OnCreate(int lv)
	{
		base.OnCreate(lv);
		this.owner.refVal = 0;
		this.owner.LV = 1;
		this.owner.c_lockLv = this.owner.LV;
	}

	public override void OnSimulateHour(VirtualDate date)
	{
		this.owner.turn++;
		if (this.owner.turn >= 2)
		{
			this.owner.turn = 0;
			if (this.owner.c_lockLv == 0)
			{
				Card owner = this.owner;
				int lv = owner.LV;
				owner.LV = lv + 1;
			}
			else
			{
				Card owner2 = this.owner;
				int lv = owner2.LV;
				owner2.LV = lv - 1;
				if (this.owner.LV < 1)
				{
					this.owner.LV = 1;
				}
			}
			this.owner.c_lockLv = this.owner.LV;
		}
	}
}

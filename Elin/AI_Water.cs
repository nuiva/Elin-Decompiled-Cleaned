using System;

public class AI_Water : TaskPoint
{
	public override int LeftHand
	{
		get
		{
			return -1;
		}
	}

	public override int RightHand
	{
		get
		{
			return 1105;
		}
	}

	public override bool HasProgress
	{
		get
		{
			return true;
		}
	}

	public override void OnProgress()
	{
		this.owner.SetTempHand(1106, -1);
		this.owner.PlaySound("Material/mud", 1f, true);
	}

	public override void OnProgressComplete()
	{
		this.owner.PlaySound("water_farm", 1f, true);
		EClass._map.SetLiquid(this.pos.x, this.pos.z, 1, 2);
		this.pos.cell.isWatered = true;
	}

	public Thing well;
}

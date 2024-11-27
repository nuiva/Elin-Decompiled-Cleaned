using System;

public class AI_Farm : TaskPoint
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
			return 1006;
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
		this.owner.PlaySound("Material/mud", 1f, true);
	}

	public override void OnProgressComplete()
	{
	}
}

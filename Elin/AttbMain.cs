using System;

public class AttbMain : Element
{
	public override bool CanGainExp
	{
		get
		{
			return true;
		}
	}

	public override int DisplayValue
	{
		get
		{
			if (this.id != 79 || this.owner.Chara == null)
			{
				return base.DisplayValue;
			}
			return this.owner.Chara.Speed;
		}
	}

	public override int MinPotential
	{
		get
		{
			return 80;
		}
	}

	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override void OnChangeValue()
	{
		Chara chara = this.owner.Chara;
		int id = this.id;
		if (id - 70 > 1)
		{
			if (id != 79)
			{
				return;
			}
			if (chara != null)
			{
				chara.SetDirtySpeed();
			}
		}
		else if (chara != null)
		{
			chara.SetDirtyWeight();
			return;
		}
	}
}

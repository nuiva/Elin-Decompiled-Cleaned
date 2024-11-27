using System;

public class Skill : Element
{
	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override bool ShowEncNumber
	{
		get
		{
			return false;
		}
	}

	public override bool ShowRelativeAttribute
	{
		get
		{
			return true;
		}
	}

	public override int GetSourcePotential(int v)
	{
		if (v > 1)
		{
			return v * 10;
		}
		return 0;
	}

	public override void OnChangeValue()
	{
		Chara chara = this.owner.Chara;
		int id = this.id;
		if (id != 207)
		{
			if (id != 306)
			{
				return;
			}
			if (chara != null)
			{
				chara.RefreshFaithElement();
			}
		}
		else if (chara != null)
		{
			chara.SetDirtyWeight();
			return;
		}
	}
}

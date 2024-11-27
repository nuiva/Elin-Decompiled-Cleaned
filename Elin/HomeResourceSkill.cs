using System;

public class HomeResourceSkill : BaseHomeResource
{
	public override BaseHomeResource.ResourceGroup Group
	{
		get
		{
			return BaseHomeResource.ResourceGroup.Skill;
		}
	}

	public override float ExpRatio
	{
		get
		{
			return (float)this.exp / (float)this.next;
		}
	}

	public override void Refresh()
	{
		this.lastValue = this.value;
		this.value = 1;
		this.next = 100;
		while (this.exp >= this.next)
		{
			this.exp -= this.next;
			this.next *= 2;
			this.value++;
		}
		int value = this.value;
		int num = this.lastValue;
	}

	public int ApplyModifier(int a)
	{
		if (a == 0)
		{
			return a;
		}
		int num = 100 + this.value * 50;
		a = a * num / 100;
		return a;
	}

	public override void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(base.Name, null);
		n.AddText("vCurrent".lang() + this.value.ToString(), FontColor.DontChange);
		n.AddText("vExp".lang() + this.exp.ToString() + "/" + this.next.ToString(), FontColor.DontChange);
		n.Build();
	}

	public int exp;

	public int next;

	public new int lastValue;
}

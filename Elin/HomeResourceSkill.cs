public class HomeResourceSkill : BaseHomeResource
{
	public int exp;

	public int next;

	public new int lastValue;

	public override ResourceGroup Group => ResourceGroup.Skill;

	public override float ExpRatio => (float)exp / (float)next;

	public override void Refresh()
	{
		lastValue = value;
		value = 1;
		next = 100;
		while (exp >= next)
		{
			exp -= next;
			next *= 2;
			value++;
		}
		_ = value;
		_ = lastValue;
	}

	public int ApplyModifier(int a)
	{
		if (a == 0)
		{
			return a;
		}
		int num = 100 + value * 50;
		a = a * num / 100;
		return a;
	}

	public override void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader(base.Name);
		n.AddText("vCurrent".lang() + value);
		n.AddText("vExp".lang() + exp + "/" + next);
		n.Build();
	}
}

public class ThirstPersonInfo : EClass
{
	public string he;

	public string his;

	public string him;

	public string himself;

	public string your;

	public bool active;

	public bool thirdperson;

	public void Set(string n)
	{
		active = int.TryParse(n, out var _);
		if (active)
		{
			he = "it".lang();
			his = "its".lang();
			him = "it".lang();
			himself = "itself".lang();
			your = "somethings".lang();
			thirdperson = true;
		}
	}

	public void Set(Card c = null, bool forceThirdPerson = false)
	{
		if (c == null)
		{
			active = false;
			return;
		}
		active = true;
		if (c.IsPC && !forceThirdPerson)
		{
			he = "you".lang();
			his = "your".lang();
			him = "you".lang();
			himself = "yourself".lang();
			your = his;
			thirdperson = false;
		}
		else if (forceThirdPerson || Msg.alwaysVisible || (!EClass.pc.isBlind && EClass.pc.CanSee(c)))
		{
			he = ((!c.isChara) ? "it".lang() : (c.IsMale ? "he".lang() : "she".lang()));
			his = ((!c.isChara) ? "its".lang() : (c.IsMale ? "his".lang() : "hers".lang()));
			him = ((!c.isChara) ? "it".lang() : (c.IsMale ? "him".lang() : "her".lang()));
			himself = ((!c.isChara) ? "itself".lang() : (c.IsMale ? "himself".lang() : "herself".lang()));
			your = "ones".lang(c.Name);
			thirdperson = true;
		}
		else
		{
			he = "it".lang();
			his = "its".lang();
			him = "it".lang();
			himself = "itself".lang();
			your = (c.isChara ? "someones" : "somethings").lang();
			thirdperson = true;
		}
	}
}

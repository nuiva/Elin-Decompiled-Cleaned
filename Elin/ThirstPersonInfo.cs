using System;

public class ThirstPersonInfo : EClass
{
	public void Set(string n)
	{
		int num;
		this.active = int.TryParse(n, out num);
		if (!this.active)
		{
			return;
		}
		this.he = "it".lang();
		this.his = "its".lang();
		this.him = "it".lang();
		this.himself = "itself".lang();
		this.your = "somethings".lang();
		this.thirdperson = true;
	}

	public void Set(Card c = null, bool forceThirdPerson = false)
	{
		if (c == null)
		{
			this.active = false;
			return;
		}
		this.active = true;
		if (c.IsPC && !forceThirdPerson)
		{
			this.he = "you".lang();
			this.his = "your".lang();
			this.him = "you".lang();
			this.himself = "yourself".lang();
			this.your = this.his;
			this.thirdperson = false;
			return;
		}
		if (forceThirdPerson || Msg.alwaysVisible || (!EClass.pc.isBlind && EClass.pc.CanSee(c)))
		{
			this.he = (c.isChara ? (c.IsMale ? "he".lang() : "she".lang()) : "it".lang());
			this.his = (c.isChara ? (c.IsMale ? "his".lang() : "hers".lang()) : "its".lang());
			this.him = (c.isChara ? (c.IsMale ? "him".lang() : "her".lang()) : "it".lang());
			this.himself = (c.isChara ? (c.IsMale ? "himself".lang() : "herself".lang()) : "itself".lang());
			this.your = "ones".lang(c.Name, null, null, null, null);
			this.thirdperson = true;
			return;
		}
		this.he = "it".lang();
		this.his = "its".lang();
		this.him = "it".lang();
		this.himself = "itself".lang();
		this.your = (c.isChara ? "someones" : "somethings").lang();
		this.thirdperson = true;
	}

	public string he;

	public string his;

	public string him;

	public string himself;

	public string your;

	public bool active;

	public bool thirdperson;
}

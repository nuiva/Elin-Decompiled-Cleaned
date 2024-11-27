using System;

public class TraitFortuneCookie : TraitFood
{
	public override void OnEat(Chara c)
	{
		if (!c.IsPC)
		{
			return;
		}
		Msg.Say("read_fortune", c, null, null, null);
		bool flag = this.owner.blessedState < BlessedState.Blessed && (this.owner.blessedState <= BlessedState.Cursed || EClass.rnd(2) == 0);
		string text = Lang.GetDialog("rumor", "fortune" + (flag ? "_false" : "")).RandomItem<string>();
		Msg.SetColor(Msg.colors.Talk);
		Msg.SayRaw(text);
	}
}

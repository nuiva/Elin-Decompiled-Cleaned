public class TraitFortuneCookie : TraitFood
{
	public override void OnEat(Chara c)
	{
		if (c.IsPC)
		{
			Msg.Say("read_fortune", c);
			bool flag = owner.blessedState < BlessedState.Blessed && (owner.blessedState <= BlessedState.Cursed || EClass.rnd(2) == 0);
			string text = Lang.GetDialog("rumor", "fortune" + (flag ? "_false" : "")).RandomItem();
			Msg.SetColor(Msg.colors.Talk);
			Msg.SayRaw(text);
		}
	}
}

using UnityEngine;

public class TraitDaggerGrave : TraitItem
{
	public override string LangUse => "actMourn";

	public override bool CanUseFromInventory => false;

	public override bool CanBeHeld => !(EClass._zone is Zone_WindRest);

	public override bool UseExtra => owner.isOn;

	public override bool OnUse(Chara c)
	{
		if (EClass._zone is Zone_WindRest)
		{
			if (!owner.isOn)
			{
				if (EClass.world.date.hour >= 0 && EClass.world.date.hour < 19)
				{
					owner.PlaySound("curse3");
					owner.PlayEffect("curse");
					Msg.Say("daggerCursed");
					return true;
				}
				owner.isOn = true;
				Effect.Get("hit_light").Play(owner.pos);
				owner.PlaySound("crystal resonance");
				int num = RemainingSword();
				Debug.Log(num);
				string[] list = Lang.GetList("daggerTalk");
				owner.TalkRaw(list[num]);
				owner.RecalculateFOV();
				if (num == 0 || (EClass.debug.enable && EInput.isShiftDown))
				{
					SoundManager.ForceBGM();
					LayerDrama.Activate("_event", "event", "swordkeeper");
				}
			}
		}
		else
		{
			owner.isOn = !owner.isOn;
		}
		string text = owner.c_context;
		if (EClass.core.config.backer.FilterAll || text.IsEmpty())
		{
			text = "filteredBacker".lang();
		}
		Msg.Say("daggerGrave", text);
		return true;
	}

	public int RemainingSword()
	{
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if ((!(thing.id != "grave_dagger1") || !(thing.id != "grave_dagger2")) && thing.isOn)
			{
				num++;
			}
		}
		return 14 - num;
	}
}

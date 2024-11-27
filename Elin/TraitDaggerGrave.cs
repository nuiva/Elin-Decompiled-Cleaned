using System;
using UnityEngine;

public class TraitDaggerGrave : TraitItem
{
	public override string LangUse
	{
		get
		{
			return "actMourn";
		}
	}

	public override bool CanUseFromInventory
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return !(EClass._zone is Zone_WindRest);
		}
	}

	public override bool UseExtra
	{
		get
		{
			return this.owner.isOn;
		}
	}

	public override bool OnUse(Chara c)
	{
		if (EClass._zone is Zone_WindRest)
		{
			if (!this.owner.isOn)
			{
				if (EClass.world.date.hour >= 0 && EClass.world.date.hour < 19)
				{
					this.owner.PlaySound("curse3", 1f, true);
					this.owner.PlayEffect("curse", true, 0f, default(Vector3));
					Msg.Say("daggerCursed");
					return true;
				}
				this.owner.isOn = true;
				Effect.Get("hit_light").Play(this.owner.pos, 0f, null, null);
				this.owner.PlaySound("crystal resonance", 1f, true);
				int num = this.RemainingSword();
				Debug.Log(num);
				string[] list = Lang.GetList("daggerTalk");
				this.owner.TalkRaw(list[num], null, null, false);
				this.owner.RecalculateFOV();
				if (num == 0 || (EClass.debug.enable && EInput.isShiftDown))
				{
					SoundManager.ForceBGM();
					LayerDrama.Activate("_event", "event", "swordkeeper", null, null, "");
				}
			}
		}
		else
		{
			this.owner.isOn = !this.owner.isOn;
		}
		string text = this.owner.c_context;
		if (EClass.core.config.backer.FilterAll || text.IsEmpty())
		{
			text = "filteredBacker".lang();
		}
		Msg.Say("daggerGrave", text, null, null, null);
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

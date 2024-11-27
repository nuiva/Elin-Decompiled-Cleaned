using System;
using System.Collections.Generic;

public class TraitRadio : TraitItem
{
	public override string IDActorEx
	{
		get
		{
			return this.owner.GetStr(52, null);
		}
	}

	public override bool MaskOnBuild
	{
		get
		{
			return true;
		}
	}

	public override bool ShowContextOnPick
	{
		get
		{
			return true;
		}
	}

	public override bool OnUse(Chara c)
	{
		EClass.ui.AddLayer<LayerList>().SetStringList(() => this.ids, delegate(int a, string n)
		{
			EClass.scene.RemoveActorEx(this.owner);
			this.owner.SetStr(52, (a == 0) ? null : n);
			this.owner.isOn = (a != 0);
			if (a != 0)
			{
				EClass.scene.AddActorEx(this.owner, null);
			}
			SE.SwitchOn();
		}, true).SetSize(450f, -1f);
		return false;
	}

	public List<string> ids = new List<string>
	{
		"none",
		"amb_fire",
		"amb_bbq",
		"amb_crowd",
		"amb_seagull",
		"amb_horror",
		"amb_pub",
		"amb_smelter",
		"amb_clockwork",
		"amb_dead",
		"amb_magic",
		"amb_fountain",
		"amb_clock",
		"amb_boat",
		"amb_waterfall"
	};
}

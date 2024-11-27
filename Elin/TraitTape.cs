using System;

public class TraitTape : TraitItem
{
	public override void SetName(ref string s)
	{
		s = "_tape".lang(this.owner.refVal.ToString() ?? "", s, null, null, null);
	}

	public override void OnCreate(int lv)
	{
		if (EClass._map.plDay != null && EClass._map.plDay.list.Count > 0)
		{
			this.owner.refVal = EClass._map.plDay.list[0].data.id;
			return;
		}
		this.owner.refVal = EClass.core.refs.dictBGM.RandomItem<int, BGMData>().id;
	}

	public override bool OnUse(Chara c)
	{
		if (this.owner.refVal == 0 || EClass.player.knownBGMs.Contains(this.owner.refVal))
		{
			Msg.Say("songAlreayKnown");
		}
		else
		{
			Msg.Say("songAdded", EClass.core.refs.dictBGM[this.owner.refVal]._name, this.owner.refVal.ToString() ?? "", null, null);
			EClass.player.knownBGMs.Add(this.owner.refVal);
		}
		EClass.Sound.Play("tape");
		this.owner.ModNum(-1, true);
		return false;
	}
}

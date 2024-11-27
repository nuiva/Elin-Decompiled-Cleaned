using System;
using System.Collections.Generic;
using UnityEngine;

public class ZonePreEnterBout : ZonePreEnterEvent
{
	public override void Execute()
	{
		Debug.Log(EClass._zone);
		Debug.Log(EClass._map);
		Debug.Log(EClass._map.GetCenterPos());
		Point point = EClass._map.GetCenterPos().GetNearestPoint(false, false, true, false) ?? EClass._map.GetCenterPos();
		Point point2 = point;
		string str = (point2 != null) ? point2.ToString() : null;
		string str2 = "/";
		Chara chara = this.target;
		Debug.Log(str + str2 + ((chara != null) ? chara.ToString() : null));
		if (this.target == null)
		{
			Debug.LogError("exception: Target is Null");
			return;
		}
		this.target.MoveZone(EClass._zone, new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = point.x,
			z = point.z
		});
		EClass._zone.SetBGM(102, true);
		Debug.Log(this.target.pos);
		List<Chara> list = new List<Chara>
		{
			this.target
		};
		for (int i = 0; i < EClass.pc.party.members.Count - 1; i++)
		{
			Chara chara2 = CharaGen.CreateFromFilter("c_neutral", this.target.LV + 10, -1);
			Debug.Log(chara2);
			chara2.ChangeRarity(Rarity.Superior);
			if (chara2.LV < this.target.LV)
			{
				chara2.SetLv(this.target.LV);
			}
			EClass._zone.AddCard(chara2, this.target.pos.GetNearestPoint(false, false, true, false) ?? this.target.pos);
			list.Add(chara2);
		}
		Debug.Log(list.Count);
		foreach (Chara chara3 in list)
		{
			chara3.c_originalHostility = (chara3.hostility = Hostility.Enemy);
			chara3.SetEnemy(EClass.pc);
			chara3.HealAll();
		}
	}

	public Chara target;
}

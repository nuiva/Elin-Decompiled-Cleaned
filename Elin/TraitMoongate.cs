using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using UnityEngine;

public class TraitMoongate : Trait
{
	public override bool CanUse(Chara c)
	{
		return !EClass._zone.IsInstance && EClass._zone.dateExpire == 0 && !EClass._zone.IsRegion && this.owner.IsInstalled;
	}

	public override bool OnUse(Chara c)
	{
		LayerProgress.StartAsync("Loading", this.UseMoongate(), null);
		return false;
	}

	public UniTask<bool> UseMoongate()
	{
		TraitMoongate.<UseMoongate>d__3 <UseMoongate>d__;
		<UseMoongate>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
		<UseMoongate>d__.<>4__this = this;
		<UseMoongate>d__.<>1__state = -1;
		<UseMoongate>d__.<>t__builder.Start<TraitMoongate.<UseMoongate>d__3>(ref <UseMoongate>d__);
		return <UseMoongate>d__.<>t__builder.Task;
	}

	public void LoadMap(MapMetaData m)
	{
		if (EClass.pc.burden.GetPhase() == 4)
		{
			Msg.Say("returnOverweight");
			return;
		}
		Debug.Log("loading:" + m.name + "/" + m.path);
		Zone_User zone_User = EClass.game.spatials.Find<Zone_User>((Zone_User z) => z.idUser == m.id);
		if (zone_User == null)
		{
			zone_User = (SpatialGen.Create("user", EClass.world.region, true, -99999, -99999, 0) as Zone_User);
			zone_User.path = m.path;
			zone_User.idUser = m.id;
			zone_User.dateExpire = EClass.world.date.GetRaw(1);
			zone_User.name = m.name;
		}
		Debug.Log(zone_User);
		this.MoveZone(zone_User);
	}

	public void MoveZone(Zone zone)
	{
		zone.instance = new ZoneInsstanceMoongate
		{
			uidZone = EClass._zone.uid,
			x = EClass.pc.pos.x,
			z = EClass.pc.pos.z
		};
		EClass.pc.MoveZone(zone, ZoneTransition.EnterState.Moongate);
	}

	public UniTask<bool> test;
}

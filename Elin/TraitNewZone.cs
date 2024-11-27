using System;
using UnityEngine;

public class TraitNewZone : Trait
{
	public Zone zone
	{
		get
		{
			return RefZone.Get(this.owner.c_uidZone);
		}
		set
		{
			this.owner.c_uidZone = ((value != null) ? value.uid : 0);
		}
	}

	public virtual bool CanUseInTempDungeon
	{
		get
		{
			return false;
		}
	}

	public virtual string langOnUse
	{
		get
		{
			return "";
		}
	}

	public virtual bool IsUpstairs
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsDownstairs
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsTeleport
	{
		get
		{
			return false;
		}
	}

	public virtual bool OnlyInTheSameTopZone
	{
		get
		{
			return false;
		}
	}

	public virtual bool AutoEnter
	{
		get
		{
			return true;
		}
	}

	public virtual bool ForceEnter
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanToggleAutoEnter
	{
		get
		{
			return false;
		}
	}

	public virtual bool CreateExternalZone
	{
		get
		{
			return !this.IsTeleport && (this.owner.GetStr(30, null) != null || base.GetParam(1, null) != null);
		}
	}

	public virtual ZoneTransition.EnterState enterState
	{
		get
		{
			return ZoneTransition.EnterState.Region;
		}
	}

	public virtual bool IsEntrace
	{
		get
		{
			return !this.IsUpstairs && !this.IsDownstairs;
		}
	}

	public override bool CanBeHeld
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool CanBeStolen
	{
		get
		{
			return false;
		}
	}

	public virtual int UseDist
	{
		get
		{
			return 0;
		}
	}

	public override void OnImportMap()
	{
		this.owner.c_uidZone = 0;
	}

	public virtual Point GetExitPos()
	{
		Point point = new Point(this.owner.pos);
		if (this.owner.dir % 2 == 0)
		{
			point.x -= this.owner.sourceCard.W / 2;
			point.z--;
		}
		else
		{
			point.x++;
			point.z += this.owner.sourceCard.H / 2;
		}
		return point;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.AllowNewZone)
		{
			return;
		}
		bool flag = this.IsEntrace || p.IsSelf;
		if (this.owner.sourceRenderCard.multisize)
		{
			int x = p.pos.x;
			int z = p.pos.z;
			int w = this.owner.W;
			int h = this.owner.H;
			flag = ((this.owner.dir % 2 == 0) ? (z == this.owner.pos.z && x > this.owner.pos.x - w + 1 && x <= this.owner.pos.x - 1) : (p.pos.x == this.owner.pos.x && p.pos.z >= this.owner.pos.z + 1 && p.pos.z < this.owner.pos.z + h - 1));
		}
		if (!flag)
		{
			return;
		}
		if (EClass.pc.held != null && EClass.pc.held.trait.CanOnlyCarry)
		{
			return;
		}
		if (p.dist <= this.UseDist)
		{
			p.TrySetAct("actNewZone", delegate()
			{
				if ((EClass._zone.RegenerateOnEnter || EClass._zone.IsInstance) && !this.CanUseInTempDungeon)
				{
					Msg.Say("badidea");
					return false;
				}
				return this.MoveZone(false);
			}, this.owner, CursorSystem.MoveZone, 1, false, true, false);
		}
	}

	public bool CanAutoEnter()
	{
		return EClass._zone.AllowNewZone && !this.owner.sourceRenderCard.multisize && (EClass.pc.held == null || !EClass.pc.held.trait.CanOnlyCarry);
	}

	public bool MoveZone(bool confirmed = false)
	{
		if (this.Electricity != 0 && !this.owner.isOn)
		{
			this.owner.PlaySound("electricity_insufficient", 1f, true);
			return false;
		}
		if (this.TryTeleport())
		{
			return true;
		}
		if (!confirmed && EClass._zone.IsNefia && EClass._zone.Boss != null)
		{
			EClass.pc.ai.Cancel();
			EInput.Consume(true, 1);
			Dialog.YesNo("ExitZoneBoss".lang(EClass._zone.Name, null, null, null, null), delegate
			{
				this.MoveZone(true);
			}, null, "yes", "no");
			return false;
		}
		ZoneTransition transition = new ZoneTransition
		{
			state = this.enterState,
			idTele = base.GetParam(3, null)
		};
		Zone zone = null;
		string param = base.GetParam(2, null);
		int num = (param != null) ? param.ToInt() : 0;
		if (this.OnlyInTheSameTopZone && this.zone != null && this.zone.GetTopZone() != EClass._zone.GetTopZone())
		{
			this.zone = null;
		}
		if (this.zone == null)
		{
			if (this.CreateExternalZone)
			{
				zone = EClass._zone;
			}
			else if (this.IsTeleport)
			{
				zone = EClass.world.region.FindZone(base.GetParam(1, null));
				if (zone != null)
				{
					zone = zone.GetTopZone();
					this.zone = zone.FindZone(num);
				}
			}
			else if (this.IsDownstairs || this.IsUpstairs)
			{
				zone = EClass._zone.GetTopZone();
				num = EClass._zone.lv + (this.IsUpstairs ? 1 : (this.IsDownstairs ? -1 : 0));
				this.zone = EClass._zone.GetTopZone().FindZone(num);
				if (this.zone == null && EClass._zone.parent.IsRegion && ((this.IsUpstairs && EClass._zone.lv == -1) || (this.IsDownstairs && EClass._zone.lv == 1)))
				{
					EClass.pc.MoveZone(EClass._zone.parent as Zone, ZoneTransition.EnterState.Auto);
					return false;
				}
			}
			if (this.OnlyInTheSameTopZone && this.zone != null && this.zone.GetTopZone() != EClass._zone.GetTopZone())
			{
				this.zone = null;
			}
			if (this.zone == null)
			{
				if (this.OnlyInTheSameTopZone && zone != null && zone.GetTopZone() != EClass._zone.GetTopZone())
				{
					zone = null;
				}
				if (zone == null || EClass._zone.isExternalZone)
				{
					Msg.SayNothingHappen();
					return false;
				}
				this.CreateZone(zone, num);
			}
		}
		if ((this.IsDownstairs || this.IsUpstairs) && this.zone.IDGenerator == null && EClass._zone.IDGenerator == null)
		{
			this.zone.events.AddPreEnter(new ZonePreEnterDigStairs
			{
				pos = this.owner.pos.Copy(),
				fromAbove = this.IsDownstairs,
				uidZone = EClass._zone.uid
			}, true);
			transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.UndergroundOrSky,
				x = this.owner.pos.x,
				z = this.owner.pos.z
			};
		}
		Debug.Log(this.zone);
		EClass.pc.MoveZone(this.zone, transition);
		return true;
	}

	public Zone CreateZone(Zone dest, int destLv)
	{
		string text = dest.GetNewZoneID(destLv);
		if (this.CreateExternalZone)
		{
			text = (this.owner.GetStr(30, null) ?? base.GetParam(1, null));
		}
		Debug.Log(string.Concat(new string[]
		{
			"Creating:",
			text,
			"/",
			destLv.ToString(),
			"/",
			EClass.sources.zones.map.ContainsKey(text).ToString(),
			"/",
			(dest != null) ? dest.ToString() : null
		}));
		Zone zone = SpatialGen.Create(text, dest, true, this.owner.pos.x, this.owner.pos.z, 0) as Zone;
		zone.lv = destLv;
		zone.x = dest.x;
		zone.y = dest.y;
		this.owner.c_uidZone = zone.uid;
		if (this.CreateExternalZone)
		{
			zone.isExternalZone = true;
		}
		Debug.Log("Created:" + zone.Name + "/" + zone.id);
		return zone;
	}

	public override void OnStepped(Chara c)
	{
		if (this.AutoEnter && c.IsPC && this.owner.IsInstalled && c.IsAliveInCurrentZone && (this.ForceEnter || !EClass.core.config.game.disableAutoStairs))
		{
			string str = "OnStepped:";
			AIAct ai = EClass.pc.ai;
			Debug.Log(str + ((ai != null) ? ai.ToString() : null));
			AI_Goto ai_Goto = EClass.pc.ai.Current as AI_Goto;
			if (ai_Goto == null && !(EClass.pc.ai is GoalManualMove))
			{
				return;
			}
			if (ai_Goto != null && !ai_Goto.dest.Equals(this.owner.pos))
			{
				return;
			}
			this.MoveZone(false);
		}
	}

	public virtual bool TryTeleport()
	{
		return false;
	}

	public virtual bool IsFor(Zone z)
	{
		if (z == this.zone)
		{
			return true;
		}
		string param = base.GetParam(1, null);
		if (!param.IsEmpty())
		{
			int num = (base.GetParam(2, null) != null) ? base.GetParam(2, null).ToInt() : 0;
			if (z.id == param && z.lv == num)
			{
				return true;
			}
		}
		return false;
	}
}

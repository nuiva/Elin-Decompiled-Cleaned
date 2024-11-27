using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Fish : AIAct
{
	public override int MaxRestart
	{
		get
		{
			return 9999;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override TargetType TargetType
	{
		get
		{
			return TargetType.Ground;
		}
	}

	public override bool CanPerform()
	{
		return Act.TP.cell.IsTopWaterAndNoSnow;
	}

	public override AIProgress CreateProgress()
	{
		return new AI_Fish.ProgressFish
		{
			posWater = this.pos
		};
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (!this.owner.IsPC)
		{
			this.owner.TryPickGroundItem();
		}
		if (this.pos != null)
		{
			if (!this.pos.cell.IsTopWaterAndNoSnow)
			{
				yield return this.Cancel();
			}
			yield return base.DoGoto(this.pos, 1, false, null);
			this.owner.LookAt(this.pos);
			if (this.owner.IsPC)
			{
				EClass.player.TryEquipBait();
				if (EClass.player.eqBait == null)
				{
					Msg.Say("noBait");
					yield return this.Cancel();
				}
			}
			if (!this.pos.cell.IsTopWaterAndNoSnow || this.owner.Dist(this.pos) > 1)
			{
				yield return this.Cancel();
			}
			AIAct.Status status = base.DoProgress();
			if (AI_Fish.shouldCancel)
			{
				AI_Fish.shouldCancel = false;
				yield return this.Cancel();
			}
			if (status == AIAct.Status.Running)
			{
				yield return AIAct.Status.Running;
			}
			if (this.owner == EClass.pc)
			{
				yield return base.Restart();
			}
			yield return base.DoWait(2);
			if (this.owner != null)
			{
				if (!this.owner.IsPC)
				{
					this.owner.TryPickGroundItem();
				}
				if (this.owner.IsPCFaction && !this.owner.IsPCParty)
				{
					this.owner.ClearInventory(ClearInventoryType.Purge);
				}
			}
			yield return base.Success(null);
		}
		if (this.owner.fov == null)
		{
			yield return this.Cancel();
		}
		List<Point> list = this.owner.fov.ListPoints();
		foreach (Point p in list)
		{
			if (!p.cell.IsTopWaterAndNoSnow)
			{
				int num;
				for (int _x = p.x - 1; _x <= p.x + 1; _x = num + 1)
				{
					for (int _z = p.z - 1; _z <= p.z + 1; _z = num + 1)
					{
						Point.shared.Set(_x, _z);
						if (Point.shared.IsValid && Point.shared.cell.IsTopWaterAndNoSnow)
						{
							Point dest = Point.shared.Copy();
							yield return base.DoGoto(dest, 0, false, null);
							this.owner.LookAt(dest);
							yield return base.DoProgress();
							yield return AIAct.Status.Success;
							dest = null;
						}
						num = _z;
					}
					num = _x;
				}
				p = null;
			}
		}
		List<Point>.Enumerator enumerator = default(List<Point>.Enumerator);
		yield break;
		yield break;
	}

	public static Point GetFishingPoint(Point p)
	{
		Point point = new Point();
		if (p.cell.IsTopWaterAndNoSnow)
		{
			return Point.Invalid;
		}
		for (int i = p.x - 1; i <= p.x + 1; i++)
		{
			for (int j = p.z - 1; j <= p.z + 1; j++)
			{
				point.Set(i, j);
				if (point.IsValid && point.cell.IsTopWaterAndNoSnow)
				{
					return point;
				}
			}
		}
		return Point.Invalid;
	}

	public static Thing Makefish(Chara c)
	{
		int num = c.Evalue(245);
		if (EClass.rnd(3 + num) == 0)
		{
			return null;
		}
		int[] source = new int[]
		{
			233,
			235,
			236,
			236,
			236,
			1170,
			1143,
			1144,
			727,
			728,
			237,
			869,
			1178,
			1179,
			1180
		};
		int num2 = 1;
		string text = "";
		if (c.IsPC || EClass.rnd(20) == 0)
		{
			if (EClass.rnd(30) == 0)
			{
				text = "book_ancient";
			}
			if (EClass.rnd(35) == 0 || EClass.debug.enable)
			{
				text = "plat";
				if (EClass.rnd(2) == 0)
				{
					text = "scratchcard";
				}
				if (EClass.rnd(3) == 0)
				{
					text = "casino_coin";
				}
				if (EClass.rnd(3) == 0)
				{
					text = "gacha_coin";
				}
				if (EClass.rnd(50) == 0 || EClass.debug.enable)
				{
					text = new string[]
					{
						"659",
						"758",
						"759",
						"806",
						"828",
						"1190",
						"1191"
					}.RandomItem<string>();
				}
			}
			if (EClass.rnd(40) == 0 && EClass.rnd(40) < num / 3 + 10)
			{
				text = "medal";
			}
		}
		Thing thing;
		if (text != "")
		{
			thing = ThingGen.Create(text, -1, -1);
		}
		else if (EClass.rnd(5 + num / 3) == 0)
		{
			thing = ThingGen.Create(source.RandomItem<int>().ToString() ?? "", -1, -1);
		}
		else
		{
			int num3 = EClass.rnd(num * 2) + 1;
			thing = ThingGen.Create("fish", -1, num3);
			num2 = EClass.rnd(num / (num3 + 10)) + 1;
			int num4 = 5;
			if (EClass.Branch != null)
			{
				num4 += EClass.Branch.Evalue(3604) * 20 + EClass.Branch.Evalue(3605) * 20 + EClass.Branch.Evalue(3706) * 25;
			}
			if (num4 >= EClass.rnd(100))
			{
				num2++;
			}
		}
		if (thing != null)
		{
			thing.SetNum(num2);
			thing.SetBlessedState(BlessedState.Normal);
		}
		return thing;
	}

	public Point pos;

	public static bool shouldCancel;

	public class ProgressFish : AIProgress
	{
		public override bool ShowProgress
		{
			get
			{
				return false;
			}
		}

		public override int MaxProgress
		{
			get
			{
				return 100;
			}
		}

		public override int LeftHand
		{
			get
			{
				return -1;
			}
		}

		public override int RightHand
		{
			get
			{
				return 1107;
			}
		}

		public override void OnStart()
		{
			if (AI_Fish.shouldCancel)
			{
				return;
			}
			this.owner.PlaySound("fish_cast", 1f, true);
			if (this.owner.Tool != null)
			{
				this.owner.Say("fish_start", this.owner, this.owner.Tool, null, null);
				return;
			}
			this.owner.Say("fish_start2", this.owner, null, null);
		}

		public override void OnProgress()
		{
			if (this.owner.IsPC && (this.owner.Tool == null || !this.owner.Tool.HasElement(245, 1)))
			{
				this.Cancel();
				return;
			}
			if (this.hit >= 0)
			{
				this.owner.renderer.PlayAnime(AnimeID.Fishing, default(Vector3), false);
				this.owner.PlaySound("fish_fight", 1f, true);
				this.Ripple();
				int a = Mathf.Clamp(10 - EClass.rnd(this.owner.Evalue(245) + 1) / 10, 5, 10);
				if (this.hit > EClass.rnd(a))
				{
					this.hit = 100;
					this.progress = this.MaxProgress;
				}
				this.hit++;
				return;
			}
			if (EClass.rnd(Mathf.Clamp(10 - EClass.rnd(this.owner.Evalue(245) + 1) / 5, 2, 10)) == 0 && this.progress >= 10)
			{
				this.hit = 0;
			}
			if (this.progress == 2 || (this.progress >= 8 && this.progress % 6 == 0 && EClass.rnd(3) == 0))
			{
				this.owner.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
				this.Ripple();
			}
		}

		public void Ripple()
		{
			if (this.posWater != null)
			{
				Effect.Get("ripple").Play(this.posWater, -0.04f, null, null);
				this.posWater.PlaySound("fish_splash", true, 1f, true);
			}
		}

		public override void OnProgressComplete()
		{
			this.owner.renderer.PlayAnime(AnimeID.Fishing, default(Vector3), false);
			if (this.hit < 100)
			{
				this.Fail();
				return;
			}
			if (this.owner.IsPC && !EClass.debug.enable)
			{
				if (EClass.player.eqBait == null || EClass.player.eqBait.isDestroyed)
				{
					Msg.Say("noBait");
					return;
				}
				EClass.player.eqBait.ModNum(-1, true);
			}
			Thing thing = AI_Fish.Makefish(this.owner);
			if (thing == null)
			{
				this.Fail();
				return;
			}
			int num = thing.Num;
			EClass._zone.AddCard(thing, this.owner.pos);
			thing.renderer.PlayAnime(AnimeID.Jump, default(Vector3), false);
			this.owner.Say("fish_get", this.owner, thing, null, null);
			this.owner.PlaySound("fish_get", 1f, true);
			this.owner.elements.ModExp(245, 100, false);
			if (thing.id == "medal")
			{
				thing.isHidden = false;
			}
			if (this.owner.IsPC)
			{
				if (EClass.game.config.preference.pickFish)
				{
					if (StatsBurden.GetPhase((EClass.pc.ChildrenWeight + thing.ChildrenAndSelfWeight) * 100 / EClass.pc.WeightLimit) >= 3)
					{
						EClass.pc.Say("tooHeavy", thing, null, null);
						AI_Fish.shouldCancel = true;
					}
					else
					{
						this.owner.Pick(thing, true, true);
					}
				}
			}
			else
			{
				foreach (Thing thing2 in this.owner.things.List((Thing t) => t.source._origin == "fish", false))
				{
					thing2.Destroy();
				}
			}
			if (EClass.rnd(2) == 0 || num > 1)
			{
				this.owner.stamina.Mod(-1 * num);
			}
		}

		public void Fail()
		{
			if (this.owner.IsPC)
			{
				this.owner.Say("fish_miss", this.owner, null, null);
			}
			this.owner.stamina.Mod(-1);
		}

		public int hit = -1;

		public Point posWater;
	}
}

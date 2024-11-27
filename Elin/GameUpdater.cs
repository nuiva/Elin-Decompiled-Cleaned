using System;
using System.Collections.Generic;
using Algorithms;
using UnityEngine;

public class GameUpdater : EClass
{
	public void Reset()
	{
		this.area = new GameUpdater.AreaUpdater();
		this.surface = new GameUpdater.SurfaceUpdater();
		this.surfaceFast = new GameUpdater.FastSurfaceUpdater();
		this.fire = new GameUpdater.FireUpdater();
		this.chara = new GameUpdater.CharaUpdater();
		this.condition = new GameUpdater.ConditionUpdater();
		this.thing = new GameUpdater.ThingUpdater();
		this.sensor = new GameUpdater.SensorUpdater();
	}

	public unsafe void Update100ms()
	{
		if (EClass.game.activeZone.IsPCFaction && EClass.Branch.resources.isDirty)
		{
			EClass.Branch.resources.Refresh();
		}
		bool flag = EClass.world.weather.CurrentCondition == Weather.Condition.RainHeavy;
		bool flag2;
		if (!EClass._map.config.indoor)
		{
			if (EClass.pc.IsInActiveZone)
			{
				Room room = EClass.pc.pos.cell.room;
				flag2 = (room != null && room.HasRoof);
			}
			else
			{
				flag2 = false;
			}
		}
		else
		{
			flag2 = true;
		}
		bool flag3 = flag2;
		Room room2 = EClass.pc.pos.cell.room;
		bool flag4 = room2 != null && room2.data.atrium;
		float mtpVolume = (EClass._zone.lv <= -2) ? 0f : ((flag3 && !flag4) ? 0.5f : 1f);
		if (EClass.world.weather.IsRaining && !EClass._map.IsIndoor)
		{
			for (int i = 0; i < EClass.setting.weather.splashCount * (flag ? 2 : 1); i++)
			{
				Point randomPoint = EClass.screen.GetRandomPoint();
				if (randomPoint.IsValid)
				{
					Cell cell = randomPoint.cell;
					if (!cell.HasRoof && cell._block == 0 && !randomPoint.IsSky)
					{
						if (cell.IsTopWaterAndNoSnow)
						{
							EClass.scene.psRainSplash.Emit(*randomPoint.PositionCenter(), Vector3.zero, Rand.Range(0.6f, 1.1f), 0.2f, Color.white);
						}
						else
						{
							EClass.scene.psRainSplashWater.Emit(*randomPoint.PositionCenter(), Vector3.zero, Rand.Range(0.6f, 1.1f), 0.2f, Color.white);
						}
					}
				}
			}
		}
		if (flag)
		{
			this.timerThunder += 0.1f;
			if (this.timerThunder > EClass.setting.weather.thunerInterval)
			{
				if (!EClass._map.IsIndoor)
				{
					ScreenFlash.Play("storm" + (EClass.rnd(2) + 1).ToString(), (flag3 && !flag4) ? 0.3f : 1f);
				}
				EClass.Sound.Play("Ambience/Random/storm", mtpVolume);
				this.timerThunder = 0f - Rand.Range(0f, EClass.setting.weather.thunerInterval);
			}
		}
	}

	public void FixedUpdate()
	{
		this.recipe.FixedUpdate();
		if (EClass.scene.paused)
		{
			return;
		}
		if (this.all == null)
		{
			this.all = new GameUpdater.Updater[]
			{
				this.area,
				this.surface,
				this.surfaceFast,
				this.fire,
				this.chara,
				this.condition,
				this.sensor,
				this.thing
			};
		}
		GameUpdater.delta = Core.delta;
		this.area.FixedUpdate();
		this.surface.FixedUpdate();
		this.surfaceFast.FixedUpdate();
		this.fire.FixedUpdate();
		this.chara.FixedUpdate();
		this.condition.FixedUpdate();
		this.sensor.FixedUpdate();
		this.thing.FixedUpdate();
	}

	public void Update()
	{
		if (EClass.game.activeZone == null)
		{
			return;
		}
		EClass._map.RefreshSunMap();
		if (EClass.debug.testLOS)
		{
			if (EClass._zone.IsRegion)
			{
				Zone zone = EClass.game.spatials.Find("palmia");
				PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(new Point(zone.x - EClass.scene.elomap.minX, zone.y - EClass.scene.elomap.minY), EClass.pc.pos, EClass.pc, PathManager.MoveType.Default, -1, 0);
				if (pathProgress.HasPath)
				{
					Debug.Log(pathProgress.nodes.Count);
					foreach (PathFinderNode pathFinderNode in pathProgress.nodes)
					{
						Point point2 = new Point(pathFinderNode.X, pathFinderNode.Z);
						EClass.screen.tileMap.passGuideBlock.Add(point2.PositionTopdown(), 20f, 0f);
					}
				}
			}
			Point hit = Scene.HitPoint;
			if (hit.IsValid)
			{
				for (int i = 0; i < EClass._map.charas.Count; i++)
				{
					Chara tg = EClass._map.charas[i];
					if (Los.IsVisible(hit, tg.pos, null))
					{
						Los.IsVisible(hit, tg.pos, delegate(Point point, bool blocked)
						{
							EClass.screen.tileMap.passGuideFloor.Add(point, 20f, 0f);
							if (Input.GetKey(KeyCode.LeftControl))
							{
								Debug.Log(tg.Name + ": Distance:" + hit.Distance(tg.pos).ToString());
							}
						});
					}
				}
			}
		}
		if (EClass.debug.testLOS2)
		{
			Point hit = Scene.HitPoint;
			if (hit.IsValid)
			{
				EClass._map.ForeachSphere(hit.x, hit.z, 6f, delegate(Point p)
				{
					if (p.IsValid && Los.IsVisible(hit, p, null))
					{
						EClass.screen.tileMap.passGuideFloor.Add(p, 20f, 0f);
					}
				});
			}
		}
		if (EClass.scene.paused)
		{
			return;
		}
		GameUpdater.delta = Core.gameDelta;
		EClass._zone.events.Tick(GameUpdater.delta);
		this.dateTimer += GameUpdater.delta;
		if (this.dateTimer > EClass.setting.secsPerHour / 60f)
		{
			this.dateTimer = 0f;
			EClass.world.date.AdvanceMin(1);
		}
		this.counterTimer += GameUpdater.delta;
		if (this.counterTimer > 0.1f)
		{
			this.counterTimer = 0f;
			this.counter++;
			if (this.counter == 10)
			{
				this.counter = 0;
			}
			int num = this.counter;
			if (num == 1)
			{
				UIResourceTrack.Refresh();
				return;
			}
			if (num != 5)
			{
				return;
			}
			if (EClass.Sound.currentAmbience)
			{
				EClass.Sound.currentAmbience.TryEmit(EClass.world.date.IsDay, Point.shared.Set(EClass.pc.pos.x + EClass.rnd(5) - EClass.rnd(5), EClass.pc.pos.z).Clamp(false).Position(), 1f);
			}
		}
	}

	public string GetText()
	{
		if (this.all == null)
		{
			return "";
		}
		string text = "";
		foreach (GameUpdater.Updater updater in this.all)
		{
			text = string.Concat(new string[]
			{
				text,
				updater.GetType().Name,
				": ",
				updater.updatesPerFrame.ToString(),
				" / frame     ",
				updater.maxCount.ToString(),
				" in ",
				updater.duration.ToString(),
				"sec \n"
			});
		}
		return text;
	}

	public static float delta;

	public GameUpdater.AreaUpdater area;

	public GameUpdater.SurfaceUpdater surface;

	public GameUpdater.FastSurfaceUpdater surfaceFast;

	public GameUpdater.FireUpdater fire;

	public GameUpdater.CharaUpdater chara;

	public GameUpdater.ConditionUpdater condition;

	public GameUpdater.ThingUpdater thing;

	public GameUpdater.SensorUpdater sensor;

	public RecipeUpdater recipe = new RecipeUpdater();

	public GameUpdater.Updater[] all;

	private float dateTimer;

	private float counterTimer;

	private int counter;

	private float timerThunder;

	public class Updater : EClass
	{
		public virtual void FixedUpdate()
		{
		}

		public void SetUpdatesPerFrame(int _maxcount, float _duration)
		{
			this.duration = _duration;
			this.maxCount = _maxcount;
			if (this.maxCount <= 0)
			{
				this.updatesPerFrame = 0;
				return;
			}
			this._updatesPerFrame += EClass.scene.actionMode.gameSpeed * (float)this.maxCount / (1f / GameUpdater.delta) / this.duration;
			this.updatesPerFrame = (int)this._updatesPerFrame;
			this._updatesPerFrame -= (float)this.updatesPerFrame;
		}

		public int index;

		public int updatesPerFrame;

		public int maxCount;

		public float duration = 1f;

		public float _updatesPerFrame;

		public Point pos = new Point();
	}

	public class AreaUpdater : GameUpdater.Updater
	{
		public override void FixedUpdate()
		{
			List<Area> listArea = EClass._map.rooms.listArea;
			if (listArea.Count == 0)
			{
				return;
			}
			base.SetUpdatesPerFrame(listArea.Count, 1f);
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.index++;
				if (this.index >= listArea.Count)
				{
					this.index = 0;
				}
				listArea[this.index].Update();
			}
		}
	}

	public class SurfaceUpdater : GameUpdater.Updater
	{
		public int SizeXZ
		{
			get
			{
				return EClass._map.SizeXZ;
			}
		}

		public int Size
		{
			get
			{
				return EClass._map.Size;
			}
		}

		public override void FixedUpdate()
		{
			if (!this.pos.Set(this.x, this.z).IsValid)
			{
				this.x = (this.z = 0);
			}
			base.SetUpdatesPerFrame(this.SizeXZ, 20f);
			PropSet propSet = EClass._map.Roaming.raceMap.TryGetValue("fish", null);
			if (propSet != null)
			{
				int num = propSet.num;
			}
			bool isRaining = EClass.world.weather.IsRaining;
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.pos.Set(this.x, this.z);
				SourceObj.Row sourceObj = this.pos.sourceObj;
				bool flag = this.pos.sourceBlock.id != 0;
				Cell cell = this.pos.cell;
				bool flag2 = EClass._map.IsIndoor || cell.HasRoof;
				if (cell.effect != null)
				{
					if (cell.HasFire)
					{
						if (isRaining)
						{
							this.pos.ModFire(-5);
						}
						else if (EClass.rnd(2) == 0)
						{
							this.pos.ModFire(-2);
						}
					}
					else if (cell.HasLiquid && (!isRaining || cell.IsTopWater || cell.HasRoof))
					{
						EClass._map.ModLiquid(this.pos.x, this.pos.z, -1);
					}
				}
				if (isRaining)
				{
					if (!flag2)
					{
						if (cell.IsFarmField)
						{
							cell.isWatered = true;
						}
						if (EClass.rnd(500) == 0 && !flag && !cell.blocked && !cell.HasObj && !cell.IsTopWaterAndNoSnow && !EClass._zone.IsRegion)
						{
							EClass._map.SetLiquid(this.pos.x, this.pos.z, 1, EClass.rnd(3) + 1);
						}
					}
					if (EClass.rnd(50) == 0 && cell.decal != 0)
					{
						cell.decal = 0;
					}
				}
				this.x++;
				if (this.x >= this.Size)
				{
					this.x = 0;
					this.z++;
					if (this.z >= this.Size)
					{
						this.z = 0;
					}
				}
			}
		}

		public int x;

		public int z;
	}

	public class FastSurfaceUpdater : GameUpdater.SurfaceUpdater
	{
		public override void FixedUpdate()
		{
			if (!this.pos.Set(this.x, this.z).IsValid)
			{
				this.x = (this.z = 0);
			}
			base.SetUpdatesPerFrame(base.SizeXZ, 1f);
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.pos.Set(this.x, this.z);
				Cell cell = this.pos.cell;
				if (cell.effect != null && cell.effect.WillFade)
				{
					CellEffect effect = cell.effect;
					int amount = effect.amount;
					effect.amount = amount - 1;
					if (cell.effect.amount <= 0)
					{
						EClass._map.SetEffect(this.pos.x, this.pos.z, null);
					}
				}
				this.x++;
				if (this.x >= base.Size)
				{
					this.x = 0;
					this.z++;
					if (this.z >= base.Size)
					{
						this.z = 0;
					}
				}
			}
		}
	}

	public class LogicUpdater : GameUpdater.Updater
	{
		public virtual LogicalPointManager manager
		{
			get
			{
				return null;
			}
		}

		public virtual float UpdateDuration
		{
			get
			{
				return 1f;
			}
		}

		public virtual bool RefreshTile
		{
			get
			{
				return true;
			}
		}

		public override void FixedUpdate()
		{
			List<LogicalPoint> list = this.manager.list;
			if (list.Count == 0)
			{
				return;
			}
			base.SetUpdatesPerFrame(list.Count, this.UpdateDuration);
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.index++;
				if (this.index >= list.Count)
				{
					this.index = 0;
				}
				list[this.index].Update();
			}
			if (this.RefreshTile)
			{
				foreach (Point point in this.manager.refreshList)
				{
					EClass._map.RefreshSingleTile(point.x, point.z);
				}
			}
			this.manager.refreshList.Clear();
		}
	}

	public class FireUpdater : GameUpdater.LogicUpdater
	{
		public override LogicalPointManager manager
		{
			get
			{
				return EClass._map.effectManager;
			}
		}

		public override float UpdateDuration
		{
			get
			{
				return 5f;
			}
		}
	}

	public class SensorUpdater : GameUpdater.Updater
	{
		public override void FixedUpdate()
		{
			List<Chara> charas = EClass._map.charas;
			base.SetUpdatesPerFrame(charas.Count, 1f);
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.index++;
				if (this.index >= charas.Count)
				{
					this.index = 0;
				}
				Chara chara = charas[this.index];
				if (chara.IsAliveInCurrentZone && !chara.IsPC)
				{
					chara.FindNewEnemy();
				}
			}
		}
	}

	public class CharaUpdater : GameUpdater.Updater
	{
		public override void FixedUpdate()
		{
			if (Game.isPaused)
			{
				return;
			}
			List<Chara> charas = EClass._map.charas;
			base.SetUpdatesPerFrame(charas.Count, 0.05f);
			float gameDelta = Core.gameDelta;
			for (int i = 0; i < charas.Count; i++)
			{
				Chara chara = charas[i];
				if (Game.isPaused && !chara.IsPC)
				{
					break;
				}
				chara.roundTimer += gameDelta;
				float actTime = chara.actTime;
				if (chara.roundTimer > actTime)
				{
					chara.Tick();
					chara.roundTimer -= actTime;
					if (chara.ai is GoalEndTurn)
					{
						chara.SetNoGoal();
						return;
					}
					if (i > charas.Count)
					{
						break;
					}
				}
			}
		}
	}

	public class ConditionUpdater : GameUpdater.CharaUpdater
	{
		public override void FixedUpdate()
		{
			List<Chara> charas = EClass._map.charas;
			base.SetUpdatesPerFrame(charas.Count, 0.01f * (float)EClass.pc.Speed * (EClass._zone.IsRegion ? 0.1f : 1f));
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.index++;
				if (this.index >= charas.Count)
				{
					this.index = 0;
				}
				Chara chara = charas[this.index];
				for (int j = chara.conditions.Count - 1; j >= 0; j--)
				{
					Condition condition = chara.conditions[j];
					if (condition.TimeBased)
					{
						condition.Tick();
					}
					if (chara.isDead)
					{
						break;
					}
				}
			}
		}
	}

	public class ThingUpdater : GameUpdater.Updater
	{
		public override void FixedUpdate()
		{
			List<Thing> things = EClass._map.things;
			base.SetUpdatesPerFrame(things.Count, 1f);
			for (int i = 0; i < this.updatesPerFrame; i++)
			{
				this.index++;
				if (this.index >= things.Count)
				{
					this.index = 0;
				}
				Thing thing = things[this.index];
				if (thing.fov != null)
				{
					thing.CalculateFOV();
				}
				if (thing.trait.HaveUpdate)
				{
					thing.trait.Update();
				}
				if ((thing.id == "snow" || thing.id == "flyer") && EClass.rnd(30) == 0 && !thing.IsInstalled && !LayerDragGrid.Instance && !LayerCraft.Instance)
				{
					thing.Destroy();
				}
			}
		}
	}
}

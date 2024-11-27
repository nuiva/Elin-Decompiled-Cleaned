using System;
using UnityEngine;
using UnityEngine.UI;

public class WidgetDebug : Widget
{
	public override void OnActivate()
	{
		this.UpdateText();
	}

	private void OnEnable()
	{
		if (Timer.current)
		{
			this.timer = Timer.Start(0.1f, new Action(this.UpdateText), true);
		}
	}

	private void OnDisable()
	{
		if (this.timer != null)
		{
			this.timer.Cancel();
		}
	}

	public void Refresh()
	{
	}

	public void ToggleProps()
	{
		this.showProps = !this.showProps;
	}

	public void ToggleUpdates()
	{
		this.showUpdates = !this.showUpdates;
	}

	public void ToggleOther()
	{
		this.showOther = !this.showOther;
	}

	private void UpdateText()
	{
		this.deltaTime += (Time.unscaledDeltaTime - this.deltaTime) * 0.1f;
		float num = this.deltaTime * 1000f;
		float num2 = 1f / this.deltaTime;
		string text = string.Format("{0:0.0} ms ({1:0.} fps)", num, num2);
		this.textFPS.text = text + " avg delta: " + Core.avgDelta.ToString();
		if (EMono.game == null || EMono.game.activeZone == null)
		{
			this.debugText.text = "";
			return;
		}
		IPathfinder pathfinder = EMono.core.pathManager.pathfinder;
		BaseTileMap tileMap = EMono.screen.tileMap;
		PropsManager props = EMono._map.props;
		int num3 = PathManager.requestCount - this.lastPathCount;
		this.lastPathCount = PathManager.requestCount;
		Point hitPoint = Scene.HitPoint;
		text = "";
		if (this.showOther)
		{
			text += WidgetDebug.output;
			string[] array = new string[5];
			array[0] = text;
			int num4 = 1;
			AIAct ai = EMono.pc.ai;
			array[num4] = ((ai != null) ? ai.ToString() : null);
			array[2] = "/";
			array[3] = EMono.pc.ai.status.ToString();
			array[4] = "\n";
			text = string.Concat(array);
			text = string.Concat(new string[]
			{
				text,
				"time: ",
				EMono.scene.timeRatio.ToString(),
				" turn:",
				EMono.pc.turn.ToString(),
				" "
			});
			Vector3 mposWorld = EInput.mposWorld;
			if (tileMap != null)
			{
				string[] array2 = new string[8];
				array2[0] = text;
				array2[1] = "mouse: ";
				array2[2] = tileMap.mx.ToString();
				array2[3] = "/";
				array2[4] = tileMap.mz.ToString();
				array2[5] = "   hit: ";
				int num5 = 6;
				Point point = hitPoint;
				array2[num5] = ((point != null) ? point.ToString() : null);
				array2[7] = "\n";
				text = string.Concat(array2);
				if (EMono._zone.IsRegion)
				{
					Point point2 = hitPoint.Copy();
					point2.x += EMono.scene.elomapActor.elomap.minX;
					point2.z += EMono.scene.elomapActor.elomap.minY;
					string[] array3 = new string[8];
					array3[0] = text;
					array3[1] = "mouse: ";
					array3[2] = (tileMap.mx + EMono.scene.elomapActor.elomap.minX).ToString();
					array3[3] = "/";
					array3[4] = (tileMap.mz + EMono.scene.elomapActor.elomap.minY).ToString();
					array3[5] = "   hit: ";
					int num6 = 6;
					Point point3 = point2;
					array3[num6] = ((point3 != null) ? point3.ToString() : null);
					array3[7] = "\n";
					text = string.Concat(array3);
				}
			}
			CharaActorPCC charaActorPCC = EMono.pc.renderer.actor as CharaActorPCC;
			float num7 = (charaActorPCC != null) ? charaActorPCC.provider.angle : 0f;
			text = string.Concat(new string[]
			{
				text,
				"path:",
				num3.ToString(),
				"/",
				PathManager.requestCount.ToString(),
				"/",
				PathManager.Instance._pathfinder.total.ToString(),
				" syncList:",
				EMono.scene.syncList.Count.ToString(),
				"\n"
			});
			text = string.Concat(new string[]
			{
				text,
				"events:",
				EMono._zone.events.list.Count.ToString(),
				"  roundTimer:",
				EMono.pc.roundTimer.ToString(),
				"  seed:",
				EMono._map.seed.ToString(),
				"\n"
			});
			text = string.Concat(new string[]
			{
				text,
				"details:",
				CellDetail.count.ToString(),
				"(",
				CellDetail.cache.Count.ToString(),
				"  pointAnimes:",
				EMono._map.pointAnimes.Count.ToString(),
				") pccCache:",
				PCCManager.current.pccCache.Count.ToString(),
				"\n"
			});
			if (hitPoint.IsValid)
			{
				string[] array4 = new string[12];
				array4[0] = text;
				array4[1] = "GatAngle: ";
				array4[2] = Util.GetAngle((float)(hitPoint.x - EMono.pc.pos.x), (float)(hitPoint.z - EMono.pc.pos.z)).ToString();
				array4[3] = "  dir: ";
				array4[4] = EMono.pc.dir.ToString();
				array4[5] = "  actor dir: ";
				int num8 = 6;
				CardActor actor = EMono.pc.renderer.actor;
				array4[num8] = ((actor != null) ? new int?(actor.currentDir) : null).ToString();
				array4[7] = "  angle:";
				array4[8] = EMono.pc.angle.ToString();
				array4[9] = "/";
				array4[10] = num7.ToString();
				array4[11] = "\n";
				text = string.Concat(array4);
				string[] array5 = new string[7];
				array5[0] = text;
				array5[1] = "room: ";
				int num9 = 2;
				Room room = hitPoint.cell.room;
				array5[num9] = ((room != null) ? room.ToString() : null);
				array5[3] = " objVal:";
				array5[4] = hitPoint.cell.objVal.ToString();
				array5[5] = " ";
				array5[6] = hitPoint.cell.CanGrow(hitPoint.cell.sourceObj, new VirtualDate(1)).ToString();
				text = string.Concat(array5);
			}
		}
		if (this.showUpdates)
		{
			text += EMono.game.updater.GetText();
			text += "\n";
		}
		if (this.showProps)
		{
			text = text + "roaming: " + props.roaming.Count.ToString() + "\n";
			text = text + "installed: " + props.installed.Count.ToString() + "\n";
			text = text + "charas: " + EMono._map.charas.Count.ToString() + "\n";
			text = text + "global charas: " + EMono.game.cards.globalCharas.Count.ToString() + "\n";
			text += "\n";
		}
		if (hitPoint.IsValid && hitPoint.FirstChara != null)
		{
			text += "\n";
			Chara firstChara = hitPoint.FirstChara;
			text = string.Concat(new string[]
			{
				text,
				firstChara.id,
				" uid:",
				firstChara.uid.ToString(),
				" skin:",
				firstChara.idSkin.ToString(),
				" dir:",
				firstChara.dir.ToString()
			});
		}
		this.debugText.text = text;
	}

	public static string output;

	public Text debugText;

	public Text textFPS;

	public bool showUpdates;

	public bool showProps;

	public bool showOther;

	private Timer.TimerItem timer;

	private float deltaTime;

	private int lastPathCount;
}

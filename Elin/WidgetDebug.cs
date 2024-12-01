using UnityEngine;
using UnityEngine.UI;

public class WidgetDebug : Widget
{
	public static string output;

	public Text debugText;

	public Text textFPS;

	public bool showUpdates;

	public bool showProps;

	public bool showOther;

	private Timer.TimerItem timer;

	private float deltaTime;

	private int lastPathCount;

	public override void OnActivate()
	{
		UpdateText();
	}

	private void OnEnable()
	{
		if ((bool)Timer.current)
		{
			timer = Timer.Start(0.1f, UpdateText, repeat: true);
		}
	}

	private void OnDisable()
	{
		if (timer != null)
		{
			timer.Cancel();
		}
	}

	public void Refresh()
	{
	}

	public void ToggleProps()
	{
		showProps = !showProps;
	}

	public void ToggleUpdates()
	{
		showUpdates = !showUpdates;
	}

	public void ToggleOther()
	{
		showOther = !showOther;
	}

	private void UpdateText()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		float num = deltaTime * 1000f;
		float num2 = 1f / deltaTime;
		string text = $"{num:0.0} ms ({num2:0.} fps)";
		textFPS.text = text + " avg delta: " + Core.avgDelta;
		if (EMono.game == null || EMono.game.activeZone == null)
		{
			debugText.text = "";
			return;
		}
		_ = EMono.core.pathManager.pathfinder;
		BaseTileMap tileMap = EMono.screen.tileMap;
		PropsManager props = EMono._map.props;
		int num3 = PathManager.requestCount - lastPathCount;
		lastPathCount = PathManager.requestCount;
		Point hitPoint = Scene.HitPoint;
		text = "";
		if (showOther)
		{
			text += output;
			text = text + EMono.pc.ai?.ToString() + "/" + EMono.pc.ai.status.ToString() + "\n";
			text = text + "time: " + EMono.scene.timeRatio + " turn:" + EMono.pc.turn + " ";
			_ = EInput.mposWorld;
			if (tileMap != null)
			{
				text = text + "mouse: " + tileMap.mx + "/" + tileMap.mz + "   hit: " + hitPoint?.ToString() + "\n";
				if (EMono._zone.IsRegion)
				{
					Point point = hitPoint.Copy();
					point.x += EMono.scene.elomapActor.elomap.minX;
					point.z += EMono.scene.elomapActor.elomap.minY;
					text = text + "mouse: " + (tileMap.mx + EMono.scene.elomapActor.elomap.minX) + "/" + (tileMap.mz + EMono.scene.elomapActor.elomap.minY) + "   hit: " + point?.ToString() + "\n";
				}
			}
			float num4 = (EMono.pc.renderer.actor as CharaActorPCC)?.provider.angle ?? 0f;
			text = text + "path:" + num3 + "/" + PathManager.requestCount + "/" + PathManager.Instance._pathfinder.total + " syncList:" + EMono.scene.syncList.Count + "\n";
			text = text + "events:" + EMono._zone.events.list.Count + "  roundTimer:" + EMono.pc.roundTimer + "  seed:" + EMono._map.seed + "\n";
			text = text + "details:" + CellDetail.count + "(" + CellDetail.cache.Count + "  pointAnimes:" + EMono._map.pointAnimes.Count + ") pccCache:" + PCCManager.current.pccCache.Count + "\n";
			if (hitPoint.IsValid)
			{
				text = text + "GatAngle: " + Util.GetAngle(hitPoint.x - EMono.pc.pos.x, hitPoint.z - EMono.pc.pos.z) + "  dir: " + EMono.pc.dir + "  actor dir: " + EMono.pc.renderer.actor?.currentDir + "  angle:" + EMono.pc.angle + "/" + num4 + "\n";
				text = text + "room: " + hitPoint.cell.room?.ToString() + " objVal:" + hitPoint.cell.objVal + " " + hitPoint.cell.CanGrow(hitPoint.cell.sourceObj, new VirtualDate(1));
			}
		}
		if (showUpdates)
		{
			text += EMono.game.updater.GetText();
			text += "\n";
		}
		if (showProps)
		{
			text = text + "roaming: " + props.roaming.Count + "\n";
			text = text + "installed: " + props.installed.Count + "\n";
			text = text + "charas: " + EMono._map.charas.Count + "\n";
			text = text + "global charas: " + EMono.game.cards.globalCharas.Count + "\n";
			text += "\n";
		}
		if (hitPoint.IsValid && hitPoint.FirstChara != null)
		{
			text += "\n";
			Chara firstChara = hitPoint.FirstChara;
			text = text + firstChara.id + " uid:" + firstChara.uid + " skin:" + firstChara.idSkin + " dir:" + firstChara.dir;
		}
		debugText.text = text;
	}
}

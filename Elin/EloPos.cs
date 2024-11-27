using System;
using UnityEngine;

public class EloPos : EClass, IInspect
{
	public EloMap elomap
	{
		get
		{
			return EClass.scene.elomapActor.elomap;
		}
	}

	public EloMapActor actor
	{
		get
		{
			return EClass.scene.elomapActor;
		}
	}

	bool IInspect.CanInspect
	{
		get
		{
			return true;
		}
	}

	string IInspect.InspectName
	{
		get
		{
			return this.gx.ToString() + "/" + this.gy.ToString();
		}
	}

	Point IInspect.InspectPoint
	{
		get
		{
			return Point.Invalid;
		}
	}

	Vector3 IInspect.InspectPosition
	{
		get
		{
			return Vector3.zero;
		}
	}

	void IInspect.OnInspect()
	{
	}

	void IInspect.WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uiitem = n.AddHeaderCard(this.zone.Name, null);
		uiitem.image2.sprite = this.tile.sprite;
		uiitem.image2.SetNativeSize();
		n.AddHeaderTopic("mainFaction".lang(), null);
		n.AddText(this.zone.mainFaction.name, FontColor.DontChange);
		n.Space(0, 1);
		n.AddHeaderTopic("listRoamers".lang(), null);
		int num = 0;
		foreach (Chara chara in EClass.game.cards.globalCharas.Values)
		{
			if (chara.currentZone == this.zone)
			{
				n.AddText(chara.Name, FontColor.DontChange);
				num++;
				if (num > 5)
				{
					break;
				}
			}
		}
		if (num == 0)
		{
			n.AddText("????????", FontColor.DontChange);
		}
		n.Build();
	}

	public Zone zone;

	public EloMap.Cell cell;

	public EloMap.TileInfo tile;

	public int gx;

	public int gy;
}

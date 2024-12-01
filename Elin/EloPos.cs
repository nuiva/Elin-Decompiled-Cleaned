using System;
using UnityEngine;

public class EloPos : EClass, IInspect
{
	public Zone zone;

	public EloMap.Cell cell;

	public EloMap.TileInfo tile;

	public int gx;

	public int gy;

	public EloMap elomap => EClass.scene.elomapActor.elomap;

	public EloMapActor actor => EClass.scene.elomapActor;

	bool IInspect.CanInspect => true;

	string IInspect.InspectName => gx + "/" + gy;

	Point IInspect.InspectPoint => Point.Invalid;

	Vector3 IInspect.InspectPosition => Vector3.zero;

	void IInspect.OnInspect()
	{
	}

	void IInspect.WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uIItem = n.AddHeaderCard(zone.Name);
		uIItem.image2.sprite = tile.sprite;
		uIItem.image2.SetNativeSize();
		n.AddHeaderTopic("mainFaction".lang());
		n.AddText(zone.mainFaction.name);
		n.Space();
		n.AddHeaderTopic("listRoamers".lang());
		int num = 0;
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.currentZone == zone)
			{
				n.AddText(value.Name);
				num++;
				if (num > 5)
				{
					break;
				}
			}
		}
		if (num == 0)
		{
			n.AddText("????????");
		}
		n.Build();
	}
}

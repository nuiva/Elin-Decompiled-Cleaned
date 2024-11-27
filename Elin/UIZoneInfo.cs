using System;

public class UIZoneInfo : EMono
{
	public EloMap elomap
	{
		get
		{
			return EMono.scene.elomapActor.elomap;
		}
	}

	public EloMapActor actor
	{
		get
		{
			return EMono.scene.elomapActor;
		}
	}

	public void SetZone(Zone _zone)
	{
		this.zone = _zone;
		this.gx = this.zone.x;
		this.gy = this.zone.y;
		this.elomap.GetCell(this.gx, this.gy);
		this.note.Clear();
		base.GetComponentInParent<Window>().SetCaption(this.zone.Name);
		this.note.AddHeaderTopic("mainFaction".lang(), null);
		this.note.AddText(this.zone.mainFaction.name, FontColor.DontChange);
		this.note.Space(0, 1);
		this.note.AddHeaderTopic("listRoamers".lang(), null);
		int num = 0;
		foreach (Chara chara in EMono.game.cards.globalCharas.Values)
		{
			if (chara.currentZone == this.zone)
			{
				this.note.AddText(chara.Name, FontColor.DontChange);
				num++;
				if (num > 5)
				{
					break;
				}
			}
		}
		if (num == 0)
		{
			this.note.AddText("????????", FontColor.DontChange);
		}
		this.note.Build();
	}

	public UINote note;

	public UIButton buttonBuy;

	public UIButton buttonVisit;

	public UIButton buttonExplore;

	public int gx;

	public int gy;

	public Zone zone;
}

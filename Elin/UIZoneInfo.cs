public class UIZoneInfo : EMono
{
	public UINote note;

	public UIButton buttonBuy;

	public UIButton buttonVisit;

	public UIButton buttonExplore;

	public int gx;

	public int gy;

	public Zone zone;

	public EloMap elomap => EMono.scene.elomapActor.elomap;

	public EloMapActor actor => EMono.scene.elomapActor;

	public void SetZone(Zone _zone)
	{
		zone = _zone;
		gx = zone.x;
		gy = zone.y;
		elomap.GetCell(gx, gy);
		note.Clear();
		GetComponentInParent<Window>().SetCaption(zone.Name);
		note.AddHeaderTopic("mainFaction".lang());
		note.AddText(zone.mainFaction.name);
		note.Space();
		note.AddHeaderTopic("listRoamers".lang());
		int num = 0;
		foreach (Chara value in EMono.game.cards.globalCharas.Values)
		{
			if (value.currentZone == zone)
			{
				note.AddText(value.Name);
				num++;
				if (num > 5)
				{
					break;
				}
			}
		}
		if (num == 0)
		{
			note.AddText("????????");
		}
		note.Build();
	}
}

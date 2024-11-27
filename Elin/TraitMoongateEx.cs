using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TraitMoongateEx : TraitMoongate
{
	public override bool OnUse(Chara c)
	{
		List<MapMetaData> list = new List<MapMetaData>();
		foreach (FileInfo fileInfo in new DirectoryInfo(CorePath.ZoneSaveUser).GetFiles().Concat(MOD.listMaps))
		{
			if (!(fileInfo.Extension != ".z"))
			{
				MapMetaData metaData = Map.GetMetaData(fileInfo.FullName);
				if (metaData != null && metaData.IsValidVersion())
				{
					list.Add(metaData);
				}
			}
		}
		if (list.Count == 0)
		{
			EClass.pc.SayNothingHappans();
			return false;
		}
		EClass.ui.AddLayer<LayerList>().SetList2<MapMetaData>(list, (MapMetaData a) => a.name, delegate(MapMetaData a, ItemGeneral b)
		{
			base.LoadMap(a);
		}, delegate(MapMetaData a, ItemGeneral b)
		{
		}, true).SetSize(500f, -1f).SetTitles("wMoongate", null);
		return false;
	}
}

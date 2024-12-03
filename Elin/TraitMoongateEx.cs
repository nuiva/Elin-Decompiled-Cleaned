using System.Collections.Generic;
using System.IO;
using System.Linq;

public class TraitMoongateEx : TraitMoongate
{
	public override bool OnUse(Chara c)
	{
		if (EClass.core.version.demo)
		{
			Msg.SayNothingHappen();
			return false;
		}
		List<MapMetaData> list = new List<MapMetaData>();
		foreach (FileInfo item in new DirectoryInfo(CorePath.ZoneSaveUser).GetFiles().Concat(MOD.listMaps))
		{
			if (!(item.Extension != ".z"))
			{
				MapMetaData metaData = Map.GetMetaData(item.FullName);
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
		EClass.ui.AddLayer<LayerList>().SetList2(list, (MapMetaData a) => a.name, delegate(MapMetaData a, ItemGeneral b)
		{
			LoadMap(a);
		}, delegate
		{
		}).SetSize(500f)
			.SetTitles("wMoongate");
		return false;
	}
}

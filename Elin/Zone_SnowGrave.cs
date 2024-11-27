using System;
using System.Collections.Generic;

public class Zone_SnowGrave : Zone
{
	public override float PrespawnRate
	{
		get
		{
			return 1f;
		}
	}

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		Zone_SnowGrave.GenerateGrave();
	}

	public static void GenerateGrave()
	{
		List<Dictionary<string, string>> list = new ExcelData("Data/Raw/monument", 1).BuildList("_default");
		list.RemoveAt(0);
		list.Shuffle<Dictionary<string, string>>();
		int num = 200;
		string[] source = new string[]
		{
			"1035",
			"1036",
			"1046",
			"1047",
			"1048",
			"1049",
			"1050"
		};
		MapBounds mapBounds = new MapBounds
		{
			x = 50,
			z = 50,
			maxX = num - 1,
			maxZ = num - 1,
			Size = num
		};
		Point point = null;
		Point point2 = new Point();
		Point p = new Point(125, 125);
		for (int i = 0; i < list.Count; i++)
		{
			Dictionary<string, string> dictionary = list[i];
			Thing thing = ThingGen.Create(source.RandomItem<string>(), -1, -1);
			for (int j = 0; j < 1000; j++)
			{
				bool flag = true;
				point = mapBounds.GetRandomSurface(false, true, false);
				if (!point.HasBlock && !point.HasObj && point.Distance(p) >= 5)
				{
					for (int k = point.z - 1; k < point.z + 2; k++)
					{
						for (int l = point.x - 1; l < point.x + 2; l++)
						{
							point2.Set(l, k);
							if (point2.Installed != null)
							{
								flag = false;
								break;
							}
						}
						if (!flag)
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
			EClass._zone.AddCard(thing, point).Install();
			thing.c_note = BackerContent.ConvertName(dictionary["Name"]);
			thing.c_idBacker = dictionary["id"].ToInt();
			thing.isModified = true;
		}
		string[] source2 = new string[]
		{
			"1058",
			"1059",
			"1064"
		};
		for (int m = 0; m < 500; m++)
		{
			Card card = EClass._zone.AddThing(source2.RandomItem<string>(), EClass._zone.bounds.GetRandomSurface(false, true, false));
			card.dir = EClass.rnd(2);
			card.Install();
		}
	}
}

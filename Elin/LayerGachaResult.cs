using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class LayerGachaResult : ELayer
{
	public void PlayGacha(int n, string id)
	{
		this.layout.DestroyChildren(false, true);
		this.items.Clear();
		int i;
		for (i = 0; i < n; i++)
		{
			Chara c = LayerGachaResult.Draw(id.Split('_', StringSplitOptions.None).LastItem<string>());
			ItemGachaResult itemGachaResult = Util.Instantiate<ItemGachaResult>(this.moldItem, this.layout);
			itemGachaResult.SetChara(c, this);
			this.items.Add(itemGachaResult);
		}
		ELayer.pc.things.AddCurrency(ELayer.pc, id, -n, null);
		Msg.Say("playedGacha", n.ToString() ?? "", null, null, null);
		this.buttonGetAll.onClick.AddListener(delegate()
		{
			ELayer.Sound.Play("good");
			this.items.ForeachReverse(delegate(ItemGachaResult i)
			{
				i.Confirm(true);
			});
		});
		this.buttonDumpAll.onClick.AddListener(delegate()
		{
			ELayer.Sound.Play("pay");
			this.items.ForeachReverse(delegate(ItemGachaResult i)
			{
				i.Confirm(false);
			});
		});
		this.Refresh();
	}

	public static Chara Draw(string id = "citizen")
	{
		List<LayerGachaResult.Data> list = new List<LayerGachaResult.Data>();
		int num = 0;
		foreach (SourceChara.Row row in ELayer.sources.charas.rows)
		{
			if (id == "citizen")
			{
				if (!row.gachaFilter.Contains("resident"))
				{
					continue;
				}
			}
			else if (id == "livestock")
			{
				if (!row.gachaFilter.Contains("livestock"))
				{
					continue;
				}
			}
			else if (id == "unique")
			{
				if (!row.gachaFilter.Contains("resident"))
				{
					continue;
				}
				if (row.quality != 4)
				{
					continue;
				}
			}
			else if (!row.gachaFilter.Contains("resident") && !row.gachaFilter.Contains("livestock"))
			{
				continue;
			}
			int num2 = (row.chance > 0) ? row.chance : 1;
			if (row.LV < 10)
			{
				num2 = num2 * 300 / 100;
			}
			else if (row.LV < 20)
			{
				num2 = num2 * 250 / 100;
			}
			else if (row.LV < 30)
			{
				num2 = num2 * 200 / 100;
			}
			else if (row.LV < 50)
			{
				num2 = num2 * 150 / 100;
			}
			list.Add(new LayerGachaResult.Data
			{
				row = row,
				weight = num2
			});
			num += num2;
		}
		int num3 = ELayer.rnd(num);
		int num4 = 0;
		LayerGachaResult.Data data = list[0];
		foreach (LayerGachaResult.Data data2 in list)
		{
			num4 += data2.weight;
			if (num3 < num4)
			{
				data = data2;
				break;
			}
		}
		return CharaGen.Create(data.row.id, -1);
	}

	public void Refresh()
	{
		int num = 0;
		foreach (ItemGachaResult itemGachaResult in this.items)
		{
			num += itemGachaResult.GetMedal();
		}
		this.buttonDumpAll.mainText.text = "dumpAll".lang(num.ToString() ?? "", null, null, null, null);
	}

	public ItemGachaResult moldItem;

	public LayoutGroup layout;

	public UIButton buttonGetAll;

	public UIButton buttonDumpAll;

	public LayerGachaResult.Mode mode;

	public List<ItemGachaResult> items = new List<ItemGachaResult>();

	public enum Mode
	{
		Chara
	}

	public class Data
	{
		public SourceChara.Row row;

		public int weight;
	}
}

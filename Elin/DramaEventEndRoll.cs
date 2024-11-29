using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DramaEventEndRoll : DramaEvent
{
	public UIDynamicList list
	{
		get
		{
			return this.sequence.manager.listCredit;
		}
	}

	public override bool Play()
	{
		if (this.progress == 0)
		{
			this.sequence.manager.endroll.SetActive(true);
			this.Init();
			this.progress++;
		}
		else
		{
			this.timer += Time.deltaTime;
			if (this.timer < 0.08f)
			{
				return false;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				return true;
			}
			if (this.list.dsv.contentAnchoredPosition <= -this.list.dsv.contentSize + this.list.dsv.viewportSize + 1f)
			{
				if ((!EInput.rightMouse.pressedLong || !Application.isEditor) && EInput.IsAnyKeyDown(true, true))
				{
					return true;
				}
			}
			else
			{
				float num = 1f;
				if (EInput.leftMouse.pressing)
				{
					num = 10f;
				}
				if (EInput.rightMouse.pressing)
				{
					num = (Application.isEditor ? 200f : 30f);
				}
				this.list.dsv.contentAnchoredPosition += Time.deltaTime * this.sequence.manager.creditSpeed * num;
			}
		}
		return false;
	}

	public void Init()
	{
		ExcelData excelData = new ExcelData("Data/Raw/credit", 1);
		List<Dictionary<string, string>> items = excelData.BuildList("_default");
		int index = 0;
		this.list.Clear();
		BaseList list = this.list;
		UIList.Callback<string, UIItem> callback = new UIList.Callback<string, UIItem>();
		callback.onRedraw = delegate(string a, UIItem b, int i)
		{
			b.text1.SetText(BackerContent.ConvertName(a));
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			List<string> list2 = IO.LoadTextArray(CorePath.CorePackage.TextCommon + "endroll").ToList<string>();
			for (int i = 0; i < Screen.height / 32 + 1; i++)
			{
				base.<Init>g__Space|0(true);
			}
			foreach (string text in list2)
			{
				if (text.StartsWith("#"))
				{
					string[] array = text.Replace("#", "").Split(',', StringSplitOptions.None);
					string text2 = array[0];
					int num = array[1].ToInt();
					using (List<Dictionary<string, string>>.Enumerator enumerator2 = items.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							Dictionary<string, string> dictionary = enumerator2.Current;
							int index;
							if (index % 5 < num)
							{
								for (int j = 0; j < num; j++)
								{
									this.list.Add("");
									index = index;
									index++;
								}
							}
							if ((text2 == "Abandon" && dictionary["abandon"] == "Yes") || dictionary["Pledge"] == text2)
							{
								base.<Init>g__AddBacker|1(dictionary["Name"]);
							}
							if (index % 5 >= 5 - num)
							{
								for (int k = 0; k < num; k++)
								{
									this.list.Add("");
									index = index;
									index++;
								}
							}
						}
						continue;
					}
				}
				base.<Init>g__Add|2(text);
			}
			for (int l = 0; l < Screen.height / 32 / 2 - 1; l++)
			{
				base.<Init>g__Space|0(true);
			}
		};
		list.callbacks = callback;
		this.list.List();
	}

	private float timer;
}

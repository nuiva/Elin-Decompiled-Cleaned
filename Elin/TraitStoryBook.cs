using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TraitStoryBook : TraitScroll
{
	public virtual string IdBook
	{
		get
		{
			return "_main";
		}
	}

	public override void OnRead(Chara c)
	{
		if (this is TraitTutorialBook)
		{
			Tutorial.debugSkip = false;
		}
		ExcelData excelData = new ExcelData();
		excelData.path = CorePath.DramaData + this.IdBook + ".xlsx";
		if (!Lang.isBuiltin)
		{
			excelData.path = CorePath.DramaDataLocal + this.IdBook + ".xlsx";
		}
		excelData.BuildList("index");
		ExcelData.Sheet sheet = excelData.sheets["index"];
		List<Dictionary<string, string>> rows = sheet.list;
		if (!EClass.debug.allStory)
		{
			rows.ForeachReverse(delegate(Dictionary<string, string> a)
			{
				int num = a["id"].ToInt();
				if (!EClass.player.flags.IsStoryPlayed(num) || num >= 950)
				{
					rows.Remove(a);
				}
			});
		}
		EClass.ui.AddLayer<LayerList>().SetSize(450f, -1f).SetList2<Dictionary<string, string>>(rows, (Dictionary<string, string> a) => TraitStoryBook.<OnRead>g__GetText|2_1(a, "text"), delegate(Dictionary<string, string> a, ItemGeneral b)
		{
			EClass.player.flags.PlayStory(this.IdBook, a["id"].ToInt(), true);
		}, delegate(Dictionary<string, string> a, ItemGeneral b)
		{
		}, false);
	}

	[CompilerGenerated]
	internal static string <OnRead>g__GetText|2_1(Dictionary<string, string> dict, string id)
	{
		if (!Lang.isBuiltin)
		{
			return dict[id];
		}
		if (!dict.ContainsKey(id + "_" + Lang.langCode))
		{
			return dict[id + "_JP"];
		}
		return dict[id + "_" + Lang.langCode];
	}
}

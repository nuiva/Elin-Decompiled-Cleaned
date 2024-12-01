using System.Collections.Generic;

public class TraitStoryBook : TraitScroll
{
	public virtual string IdBook => "_main";

	public override void OnRead(Chara c)
	{
		if (this is TraitTutorialBook)
		{
			Tutorial.debugSkip = false;
		}
		ExcelData excelData = new ExcelData();
		excelData.path = CorePath.DramaData + IdBook + ".xlsx";
		if (!Lang.isBuiltin)
		{
			excelData.path = CorePath.DramaDataLocal + IdBook + ".xlsx";
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
		EClass.ui.AddLayer<LayerList>().SetSize().SetList2(rows, (Dictionary<string, string> a) => GetText(a, "text"), delegate(Dictionary<string, string> a, ItemGeneral b)
		{
			EClass.player.flags.PlayStory(IdBook, a["id"].ToInt(), fromBook: true);
		}, delegate
		{
		}, autoClose: false);
		static string GetText(Dictionary<string, string> dict, string id)
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
}

using System;

public class UICardInfo : EMono
{
	public void SetRenderer(IRenderer _renderer, RenderParam p)
	{
		this.renderer = _renderer;
		this.param = p;
	}

	public void UpdateImage()
	{
		if (!EMono.core.IsGameStarted)
		{
			return;
		}
		if (this.renderer != null)
		{
			this.renderer.RenderToRenderCam(this.param);
		}
	}

	public void Build()
	{
		this.note.Build();
		this.UpdateImage();
	}

	private void Update()
	{
		this.UpdateImage();
	}

	public void SetElement(Element e)
	{
		e.WriteNote(this.note, null, null);
	}

	public void SetThing(Thing t)
	{
		t.WriteNote(this.note, null, IInspect.NoteMode.Info, null);
		this.Build();
	}

	public void UpdateRecipe(Recipe r)
	{
		this.SetRenderer(r.GetRenderer(), r.renderRow.GetRenderParam(r.GetColorMaterial(), 0, null, -1));
	}

	public void SetRecipe(Recipe r)
	{
		this.UpdateRecipe(r);
		this.note.Clear();
		this.note.AddHeaderCard(r.Name, null);
		this.note.AddText(r.source.GetDetail(), FontColor.DontChange);
		this.note.Space(0, 1);
		this.note.AddHeaderTopic("buildType".lang() + ": " + r.tileType.LangPlaceType.lang(), null);
		this.note.AddText((r.tileType.LangPlaceType + "_hint").lang(), FontColor.DontChange);
		this.note.Space(0, 1);
		this.note.AddHeaderTopic("reqMat".lang(), null);
		this.Build();
	}

	public void SetBlock(Cell cell)
	{
		string blockName = cell.GetBlockName();
		RenderParam renderParam = cell.sourceBlock.GetRenderParam(cell.matBlock, 0, null, -1);
		this.SetRenderer(cell.sourceBlock.renderData, renderParam);
		this.note.AddHeaderCard(blockName, null).image2.SetActive(false);
		this.note.AddText("isMadeOf".lang(cell.matBlock.GetText("name", false), null, null, null, null), FontColor.DontChange);
		this.Build();
	}

	public void SetFloor(Cell cell)
	{
		string floorName = cell.GetFloorName();
		RenderParam renderParam = cell.sourceFloor.GetRenderParam(cell.matFloor, 0, null, -1);
		this.SetRenderer(cell.sourceFloor.renderData, renderParam);
		this.note.AddHeaderCard(floorName, null).image2.SetActive(false);
		this.note.AddText("isMadeOf".lang(cell.matFloor.GetText("name", false), null, null, null, null), FontColor.DontChange);
		this.Build();
	}

	public void SetLiquid(Cell cell)
	{
		string liquidName = cell.GetLiquidName();
		RenderParam renderParam = cell.sourceEffect.GetRenderParam(cell.matFloor, 0, null, -1);
		this.SetRenderer(cell.sourceEffect.renderData, renderParam);
		this.note.AddHeaderCard(liquidName, null).image2.SetActive(false);
		this.note.AddText("isMadeOf".lang(cell.sourceEffect.GetText("name", false), null, null, null, null), FontColor.DontChange);
		this.Build();
	}

	public void SetObj(Cell cell)
	{
		SourceObj.Row sourceObj = cell.sourceObj;
		RenderParam renderParam = cell.sourceObj.GetRenderParam(cell.matBlock, 0, null, -1);
		this.SetRenderer(sourceObj.renderData, renderParam);
		this.note.AddHeaderCard(base.name, null).image2.SetActive(false);
		this.note.AddText("isMadeOf".lang(sourceObj.DefaultMaterial.GetText("name", false), null, null, null, null), FontColor.DontChange);
		this.Build();
	}

	public UINote note;

	public IRenderer renderer;

	public RenderParam param;
}

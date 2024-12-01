public class UICardInfo : EMono
{
	public UINote note;

	public IRenderer renderer;

	public RenderParam param;

	public void SetRenderer(IRenderer _renderer, RenderParam p)
	{
		renderer = _renderer;
		param = p;
	}

	public void UpdateImage()
	{
		if (EMono.core.IsGameStarted && renderer != null)
		{
			renderer.RenderToRenderCam(param);
		}
	}

	public void Build()
	{
		note.Build();
		UpdateImage();
	}

	private void Update()
	{
		UpdateImage();
	}

	public void SetElement(Element e)
	{
		e.WriteNote(note);
	}

	public void SetThing(Thing t)
	{
		t.WriteNote(note, null, IInspect.NoteMode.Info);
		Build();
	}

	public void UpdateRecipe(Recipe r)
	{
		SetRenderer(r.GetRenderer(), r.renderRow.GetRenderParam(r.GetColorMaterial(), 0));
	}

	public void SetRecipe(Recipe r)
	{
		UpdateRecipe(r);
		note.Clear();
		note.AddHeaderCard(r.Name);
		note.AddText(r.source.GetDetail());
		note.Space();
		note.AddHeaderTopic("buildType".lang() + ": " + r.tileType.LangPlaceType.lang());
		note.AddText((r.tileType.LangPlaceType + "_hint").lang());
		note.Space();
		note.AddHeaderTopic("reqMat".lang());
		Build();
	}

	public void SetBlock(Cell cell)
	{
		string blockName = cell.GetBlockName();
		RenderParam renderParam = cell.sourceBlock.GetRenderParam(cell.matBlock, 0);
		SetRenderer(cell.sourceBlock.renderData, renderParam);
		note.AddHeaderCard(blockName).image2.SetActive(enable: false);
		note.AddText("isMadeOf".lang(cell.matBlock.GetText()));
		Build();
	}

	public void SetFloor(Cell cell)
	{
		string floorName = cell.GetFloorName();
		RenderParam renderParam = cell.sourceFloor.GetRenderParam(cell.matFloor, 0);
		SetRenderer(cell.sourceFloor.renderData, renderParam);
		note.AddHeaderCard(floorName).image2.SetActive(enable: false);
		note.AddText("isMadeOf".lang(cell.matFloor.GetText()));
		Build();
	}

	public void SetLiquid(Cell cell)
	{
		string liquidName = cell.GetLiquidName();
		RenderParam renderParam = cell.sourceEffect.GetRenderParam(cell.matFloor, 0);
		SetRenderer(cell.sourceEffect.renderData, renderParam);
		note.AddHeaderCard(liquidName).image2.SetActive(enable: false);
		note.AddText("isMadeOf".lang(cell.sourceEffect.GetText()));
		Build();
	}

	public void SetObj(Cell cell)
	{
		SourceObj.Row sourceObj = cell.sourceObj;
		RenderParam renderParam = cell.sourceObj.GetRenderParam(cell.matBlock, 0);
		SetRenderer(sourceObj.renderData, renderParam);
		note.AddHeaderCard(base.name).image2.SetActive(enable: false);
		note.AddText("isMadeOf".lang(sourceObj.DefaultMaterial.GetText()));
		Build();
	}
}

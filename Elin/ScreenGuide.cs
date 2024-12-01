using UnityEngine;

public class ScreenGuide : EMono
{
	public MeshPass passGuideBlock;

	public MeshPass passGuideFloor;

	public MeshPass passArea;

	public MeshPass passBlockMarker;

	public LineRenderer lr;

	public bool isActive = true;

	public void DrawLine(Vector3 from, Vector3 to)
	{
		lr.positionCount = 2;
		from.z = (to.z = -300f);
		lr.SetPosition(0, from);
		lr.SetPosition(1, to);
	}

	public void DrawFloor(Point pos, int tile)
	{
	}

	public void DrawBlock(Point pos, int tile)
	{
	}

	public void OnDrawPass()
	{
	}

	public void OnEndOfFrame()
	{
		lr.positionCount = 0;
	}

	public void DrawWall(Point point, int color, bool useMarkerPass = false, float offsetZ = 0f)
	{
		int num = -1;
		if (num != -1)
		{
			Vector3 vector = point.Position();
			EMono.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z - 0.01f);
		}
		SourceBlock.Row sourceBlock = point.sourceBlock;
		RenderParam renderParam = sourceBlock.GetRenderParam(point.matBlock, point.cell.blockDir, point, num);
		renderParam.matColor = color;
		renderParam.z -= 0.01f;
		if (useMarkerPass)
		{
			renderParam.x += sourceBlock.renderData.offset.x;
			renderParam.y += sourceBlock.renderData.offset.y;
			renderParam.z += sourceBlock.renderData.offset.z + offsetZ;
			passBlockMarker.Add(renderParam);
		}
		else
		{
			sourceBlock.renderData.Draw(renderParam);
		}
		if (point.cell.blockDir == 2)
		{
			renderParam.tile *= -1f;
			if (useMarkerPass)
			{
				passBlockMarker.Add(renderParam);
			}
			else
			{
				sourceBlock.renderData.Draw(renderParam);
			}
		}
	}
}

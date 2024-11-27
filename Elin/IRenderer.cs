using System;

public interface IRenderer
{
	void RenderToRenderCam(RenderParam p);

	void Draw(RenderParam p);
}

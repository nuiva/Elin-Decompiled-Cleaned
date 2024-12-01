using UnityEngine;

public class ScreenOverlay : EMono
{
	public Vector2 speed;

	public Vector2 cameraSpeed;

	public Vector2 heightSpeed;

	public Vector3 cameraAngle;

	public MeshRenderer _renderer;

	private Vector2 offsetAnime;

	private void Update()
	{
		offsetAnime += speed * Core.gameDelta;
		Vector3 vector = EMono.screen.position * -1f;
		vector.x += vector.z * heightSpeed.x;
		vector.y += vector.z * heightSpeed.y;
		vector.z = 0f;
		Vector3 vector2 = Quaternion.Euler(cameraAngle) * vector;
		_renderer.sharedMaterial.mainTextureOffset = offsetAnime + new Vector2(vector2.x * cameraSpeed.x, vector2.y * cameraSpeed.y);
	}
}

using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenshot : EMono
{
	public static UIScreenshot Instance;

	public Image imageFrame;

	public InputField inputName;

	public UIButton buttonSave;

	public UIButton buttonCancel;

	public bool isDeactivating;

	public static UIScreenshot Create()
	{
		return Util.Instantiate<UIScreenshot>("UI/Util/UIScreenshot", EMono.ui);
	}

	private void Awake()
	{
		Instance = this;
		EMono.ui.Hide(0f);
	}

	public void Activate(PartialMap partial, DirectoryInfo dir, Action<PartialMap> onSave = null, bool isUpdate = false)
	{
		inputName.text = (MapPiece.IsEditor ? GetUniqueName("") : EMono._zone.Name);
		if (isUpdate)
		{
			inputName.text = (MapPiece.IsEditor ? partial.ID : partial.name);
		}
		buttonSave.SetOnClick(delegate
		{
			isDeactivating = true;
			SE.Play("camera");
			GetComponent<CanvasGroup>().alpha = 0f;
			string text = dir.FullName + "/" + (MapPiece.IsEditor ? inputName.text.IsEmpty("new") : GetUniqueName("user")) + ".mp";
			if (isUpdate)
			{
				if (MapPiece.IsEditor)
				{
					PartialMap.Delete(partial.path);
				}
				else
				{
					text = partial.path;
				}
			}
			partial.name = inputName.text.IsEmpty(EMono._zone.Name);
			partial.path = text;
			File.Copy(PartialMap.PathTemp, text, overwrite: true);
			SavePreview(text, partial.name, delegate
			{
				if ((bool)PartialMapMenu.Instance)
				{
					PartialMapMenu.Instance.Refresh();
				}
				Deactivate();
				if (onSave != null)
				{
					onSave(partial);
				}
			});
		});
		string GetUniqueName(string id)
		{
			for (int i = 1; i < 999999; i++)
			{
				if (!File.Exists(dir.FullName + "/" + id + i + ".mp"))
				{
					return id + i;
				}
			}
			return id;
		}
	}

	private void Update()
	{
		if (isDeactivating)
		{
			EInput.Consume();
		}
		else if (EInput.rightMouse.clicked || Input.GetKeyDown(KeyCode.Escape))
		{
			Deactivate();
		}
	}

	public static void SavePreview(string path, string name, Action onSave)
	{
		EMono.core.actionsNextFrame.Add(delegate
		{
			EMono.core.actionsNextFrame.Add(delegate
			{
				Texture2D texture2D = ScreenCapture.CaptureScreenshotAsTexture();
				texture2D.filterMode = FilterMode.Point;
				int num = 300;
				int num2 = 200;
				float num3 = (float)num / (float)Screen.width;
				float num4 = (float)num2 / (float)Screen.height;
				Vector2 scale = new Vector2(num3, num4);
				Vector2 offset = new Vector2((1f - num3) * 0.5f, (1f - num4) * 0.5f);
				RenderTexture renderTexture = new RenderTexture(num, num2, 0);
				renderTexture.filterMode = FilterMode.Point;
				renderTexture.Create();
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = renderTexture;
				Graphics.Blit(texture2D, renderTexture, scale, offset);
				Texture2D texture2D2 = new Texture2D(num, num2, texture2D.format, mipChain: false);
				texture2D2.filterMode = FilterMode.Point;
				texture2D2.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
				texture2D2.Apply();
				RenderTexture.active = active;
				renderTexture.Release();
				File.WriteAllBytes(path.GetFullFileNameWithoutExtension() + ".jpg", texture2D2.EncodeToJPG());
				File.WriteAllText(path.GetFullFileNameWithoutExtension() + ".txt", name);
				UnityEngine.Object.Destroy(texture2D2);
				UnityEngine.Object.Destroy(texture2D);
				UnityEngine.Object.Destroy(renderTexture);
				onSave?.Invoke();
			});
		});
	}

	public void Deactivate()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		EMono.ui.Show(0f);
		EInput.Consume();
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIScreenshot : EMono
{
	public static UIScreenshot Create()
	{
		return Util.Instantiate<UIScreenshot>("UI/Util/UIScreenshot", EMono.ui);
	}

	private void Awake()
	{
		UIScreenshot.Instance = this;
		EMono.ui.Hide(0f);
	}

	public void Activate(PartialMap partial, DirectoryInfo dir, Action<PartialMap> onSave = null, bool isUpdate = false)
	{
		UIScreenshot.<>c__DisplayClass8_0 CS$<>8__locals1 = new UIScreenshot.<>c__DisplayClass8_0();
		CS$<>8__locals1.dir = dir;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.isUpdate = isUpdate;
		CS$<>8__locals1.partial = partial;
		CS$<>8__locals1.onSave = onSave;
		this.inputName.text = (MapPiece.IsEditor ? CS$<>8__locals1.<Activate>g__GetUniqueName|0("") : EMono._zone.Name);
		if (CS$<>8__locals1.isUpdate)
		{
			this.inputName.text = (MapPiece.IsEditor ? CS$<>8__locals1.partial.ID : CS$<>8__locals1.partial.name);
		}
		this.buttonSave.SetOnClick(delegate
		{
			CS$<>8__locals1.<>4__this.isDeactivating = true;
			SE.Play("camera");
			CS$<>8__locals1.<>4__this.GetComponent<CanvasGroup>().alpha = 0f;
			string text = CS$<>8__locals1.dir.FullName + "/" + (MapPiece.IsEditor ? CS$<>8__locals1.<>4__this.inputName.text.IsEmpty("new") : base.<Activate>g__GetUniqueName|0("user")) + ".mp";
			if (CS$<>8__locals1.isUpdate)
			{
				if (MapPiece.IsEditor)
				{
					PartialMap.Delete(CS$<>8__locals1.partial.path);
				}
				else
				{
					text = CS$<>8__locals1.partial.path;
				}
			}
			CS$<>8__locals1.partial.name = CS$<>8__locals1.<>4__this.inputName.text.IsEmpty(EMono._zone.Name);
			CS$<>8__locals1.partial.path = text;
			File.Copy(PartialMap.PathTemp, text, true);
			string path = text;
			string name = CS$<>8__locals1.partial.name;
			Action onSave2;
			if ((onSave2 = CS$<>8__locals1.<>9__2) == null)
			{
				onSave2 = (CS$<>8__locals1.<>9__2 = delegate()
				{
					if (PartialMapMenu.Instance)
					{
						PartialMapMenu.Instance.Refresh();
					}
					CS$<>8__locals1.<>4__this.Deactivate();
					if (CS$<>8__locals1.onSave != null)
					{
						CS$<>8__locals1.onSave(CS$<>8__locals1.partial);
					}
				});
			}
			UIScreenshot.SavePreview(path, name, onSave2);
		});
	}

	private void Update()
	{
		if (this.isDeactivating)
		{
			EInput.Consume(false, 1);
			return;
		}
		if (EInput.rightMouse.clicked || Input.GetKeyDown(KeyCode.Escape))
		{
			this.Deactivate();
		}
	}

	public static void SavePreview(string path, string name, Action onSave)
	{
		Action <>9__1;
		EMono.core.actionsNextFrame.Add(delegate
		{
			List<Action> actionsNextFrame = EMono.core.actionsNextFrame;
			Action item;
			if ((item = <>9__1) == null)
			{
				item = (<>9__1 = delegate()
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
					Texture2D texture2D2 = new Texture2D(num, num2, texture2D.format, false);
					texture2D2.filterMode = FilterMode.Point;
					texture2D2.ReadPixels(new Rect(0f, 0f, (float)num, (float)num2), 0, 0);
					texture2D2.Apply();
					RenderTexture.active = active;
					renderTexture.Release();
					File.WriteAllBytes(path.GetFullFileNameWithoutExtension() + ".jpg", texture2D2.EncodeToJPG());
					File.WriteAllText(path.GetFullFileNameWithoutExtension() + ".txt", name);
					UnityEngine.Object.Destroy(texture2D2);
					UnityEngine.Object.Destroy(texture2D);
					UnityEngine.Object.Destroy(renderTexture);
					Action onSave2 = onSave;
					if (onSave2 == null)
					{
						return;
					}
					onSave2();
				});
			}
			actionsNextFrame.Add(item);
		});
	}

	public void Deactivate()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		EMono.ui.Show(0f);
		EInput.Consume(false, 1);
	}

	public static UIScreenshot Instance;

	public Image imageFrame;

	public InputField inputName;

	public UIButton buttonSave;

	public UIButton buttonCancel;

	public bool isDeactivating;
}

//Written with ♥ by Ankit Priyarup
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class NodeEditor : EditorWindow
{
	public static GUISkin skin { get; private set; }
	public static int selectedIndex = 0;
	public static NodeData data;
	public static Rect lastTRect = Rect.zero;
	public static Color[] connectionColors = new Color[] {Color.white, Color.red, Color.blue};
	public static UIFlowEditor parent;
	public static string path = "Assets/UI Flow/data/";
	
	Vector2 mousePos = Vector2.zero;
	BaseNode selectedNode;
	bool makeTransitionMode = false;

	const int connectionTypes = 3;

	public static void ShowEditor(UIFlowEditor source, List<BaseNode> pow)
	{
		EditorWindow.GetWindow<NodeEditor>();
		parent = source;
		data = AssetDatabase.LoadAssetAtPath(path + parent.ID + ".asset", typeof(NodeData)) as NodeData;
		if (data == null)
		{
			data = ScriptableObject.CreateInstance<NodeData>();
			AssetDatabase.CreateAsset(data, path + parent.ID + ".asset");
			AssetDatabase.SaveAssets();
		}
	}

	void OnDisable()
	{
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		parent.SetData(data.n);
	}
	
	Vector2 initMousePos = Vector2.zero;
	void OnGUI()
	{
		Event e = Event.current;
		mousePos = e.mousePosition;

		DrawBackground();

		if (data.n == null)
		{
			data.n = new List<BaseNode>();
			Vector2 pos = new Vector2(10, position.height/2 - 30);
			MessageNode o = ScriptableObject.CreateInstance<MessageNode>();
			o.init(pos, 180, "START");
			AssetDatabase.AddObjectToAsset(o, data);
			//AssetDatabase.CreateAsset(o, path + o.id + ".asset");
		}

		switch (e.button)
		{
			case 0:
			{
				if (e.type == EventType.MouseDown && makeTransitionMode)
				{
					if (GetWinClicked())
					{
						if (!data.n[selectedIndex].Equals(selectedIndex))
						{
							BaseNode n = data.n[selectedIndex];
							n.SetInput((BaseNode)selectedNode, mousePos);
							if (GetWinClicked() && n.rootNode != null)
								n.rootNode.SetAttributeNode(lastTRect, (BaseNode)n);
							makeTransitionMode = false;
							selectedNode = null;
						}
					}
					else
					{
						makeTransitionMode = false;
						selectedNode = null;
					}

					e.Use();
				}
			}
			break;
			case 1:
			{
				if (e.type == EventType.MouseDown && !makeTransitionMode)
				{
					GenericMenu menu = new GenericMenu();
					if (!GetWinClicked())
					{
						menu.AddItem(new GUIContent("Main Menu"), false, ContextCallback, "mainmenuNode");
						menu.AddItem(new GUIContent("Single Player"), false, ContextCallback, "singleplayerNode");
						menu.AddItem(new GUIContent("Quit Confirm"), false, ContextCallback, "quitconfirmNode");
					}
					else
					{
						selectedNode = data.n[selectedIndex];
						lastTRect = selectedNode.ClickedOnRect(new Vector2(mousePos.x, mousePos.y));

						if (lastTRect != selectedNode.winRect || selectedNode.type == BaseNode.nodeType.Message)
							menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
						if (selectedIndex != 0)
							menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");
					}

					menu.ShowAsContext();
					e.Use();
				}
				
			}
			break;
			case 2:
			{
				if (e.type == EventType.mouseDown) initMousePos = e.mousePosition;
				else if (e.type == EventType.mouseDrag)
				{
					Vector2 deltaMousePos = e.mousePosition - initMousePos;
					for (int i=0; i<data.n.Count; i++)
					{
						if (data.n[i] != null)
						{
							data.n[i].winRect.x += deltaMousePos.x;
							data.n[i].winRect.y += deltaMousePos.y;
						}
					}
					initMousePos = e.mousePosition;
					Repaint();
				}
			}
			break;
		}

		if (selectedNode != null && makeTransitionMode)
		{
			Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10,10);
			DrawNodeCurve(lastTRect, mouseRect, connectionColors[0]);
		}

		BeginWindows();
		for (int i=0; i<data.n.Count; i++)
		{
			BaseNode w = data.n[i];
			if (w != null)
			{
				string title = (w.type == BaseNode.nodeType.Message) ? "" : w.winTitle;
				data.n[i].winRect = GUI.Window(i, w.winRect, DrawNodeWindow, title);
			}
		}
		EndWindows();

		foreach(BaseNode n in data.n) if (n!= null) n.DrawCurves();
		Repaint();
	}

	void DrawNodeWindow(int id)
	{		
		data.n[id].DrawWindow();
		GUI.DragWindow();

		Event e = Event.current;
		if (e.keyCode == KeyCode.Space && e.type == EventType.keyDown)
		{
			BaseNode sNode = data.n[id];
			sNode.connectionType++;
			if ((int)sNode.connectionType >= connectionTypes) sNode.connectionType = 0;
		}
	}

	void ContextCallback(object obj)
	{
		string callback = obj.ToString();

		switch (callback)
		{
			case "mainmenuNode":
				NodeMainMenu(mousePos);
			break;
			case "quitconfirmNode":
				NodeQuitConfirm(mousePos);
			break;
			case "singleplayerNode":
				NodeSinglePlayer(mousePos);
			break;
			case "makeTransition":
				if (GetWinClicked()) makeTransitionMode = true;
			break;
			case "deleteNode":
			{
				if (GetWinClicked())
				{
					BaseNode selNode = data.n[selectedIndex];
					AssetDatabase.DeleteAsset(path + selNode.id + ".asset");
					AssetDatabase.SaveAssets();
					data.n.RemoveAt(selectedIndex);					
					foreach(BaseNode n in data.n) n.NodeDeleted(selNode);
				}
			}
			break;
		}
	}

	public static BaseNode NodeMainMenu(Vector2 pos)
	{
		string title = "MAIN MENU";
		string[] options = {"SINGLEPLAYER", "MULTIPLAYER", "OPTIONS", "QUIT"};
		float width = 220;
		MenuNode n = ScriptableObject.CreateInstance<MenuNode>();
		n.init(pos, width, title, options);
		n.type = BaseNode.nodeType.MainMenu;
		AssetDatabase.AddObjectToAsset(n, data);
		//AssetDatabase.CreateAsset(n, path + n.id + ".asset");
		return n;
	}
	public static BaseNode NodeQuitConfirm(Vector2 pos)
	{
		string title = "QUIT CONFIRM";
		string[] options = {"CANCEL", "QUIT"};
		float width = 180;
		MenuNode n = ScriptableObject.CreateInstance<MenuNode>();
		n.init(pos, width, title, options);
		n.type = BaseNode.nodeType.QuitConfirm;
		AssetDatabase.AddObjectToAsset(n, data);
		//AssetDatabase.CreateAsset(n, path + n.id + ".asset");
		return n;
	}
	public static BaseNode NodeSinglePlayer(Vector2 pos)
	{
		string title = "SINGLE PLAYER";
		string[] options = {"CANCEL", "START GAME"};
		float width = 180;
		MenuNode n = ScriptableObject.CreateInstance<MenuNode>();
		n.init(pos, width, title, options);
		n.type = BaseNode.nodeType.SinglePlayer;
		AssetDatabase.AddObjectToAsset(n, data);
		//AssetDatabase.CreateAsset(n, path + n.id + ".asset");
		return n;
	}

	public bool GetWinClicked()
	{
		for (int i=0; i<data.n.Count; i++)
		{
			if (data.n[i] != null)
			{
				if (data.n[i].winRect.Contains(mousePos))
				{
					selectedIndex = i;
					return true;
				}
			}
		}
		return false;
	}

	public void DrawBackground()
	{
		if (skin == null) skin = EditorGUIUtility.Load("skin.guiskin") as GUISkin;
		GUI.skin = skin;

		float w = position.width;
		float h = position.width;

		Texture2D bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		bg.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f));
		bg.Apply();
		GUI.DrawTexture(new Rect(0, 0, w, h), bg);
		
		Handles.BeginGUI();
		Handles.color = new Color(0.7f, 0.7f, 0.7f, 0.1f);
		for (int i=0; i*60<=w; i++) Handles.DrawLine(new Vector3(60*i, 0), new Vector3(60*i, h));
		for (int i=0; i*60<=h; i++) Handles.DrawLine(new Vector3(0, 60*i), new Vector3(w, 60*i));
		Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
		for (int i=0; i*20<=w; i++) if (i%3 != 0) Handles.DrawLine(new Vector3(20*i, 0), new Vector3(20*i, h));
		for (int i=0; i*20<=h; i++) if (i%3 != 0) Handles.DrawLine(new Vector3(0, 20*i), new Vector3(w, 20*i));
		Handles.EndGUI();
	}

	public static void DrawNodeCurve(Rect start, Rect end, Color col)
	{
		Vector3 startPos = new Vector3(start.x + start.width - 10, start.y + start.height/2, 0);
		Vector3 endPos = new Vector3(end.x + end.width, end.y + end.height/2, 0);
		Vector3 startTan = startPos + (Vector3.right * 50);
		Vector3 endTan = endPos + (Vector3.left * 50);
		Color shadowCol = new Color(0, 0, 0, 0.06f);

		for (int i=0; i<3; i++) Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i+1)*5);
		Handles.DrawBezier(startPos, endPos, startTan, endTan, col, null, 2);
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeEditor : EditorWindow
{
	public static GUISkin skin;
	public static int selectedIndex = -1;
	public static List<BaseNode> windows = new List<BaseNode>();
	public static Rect lastTRect = Rect.zero;

	Vector2 mousePos;
	BaseNode selectedNode;
	bool makeTransitionMode = false;

	[MenuItem("Window/Node Editor")]
	static void ShowEditor()
	{
		EditorWindow.GetWindow<NodeEditor>();		
	}

	Vector2 initMousePos = Vector2.zero;
	void OnGUI()
	{
		Event e = Event.current;
		mousePos = e.mousePosition;

		if (skin == null) skin = EditorGUIUtility.Load("skin.guiskin") as GUISkin;
		GUI.skin = skin;
		DrawBackground();
		if (windows.Count == 0)
		{
			StartNode startNode = (StartNode)ScriptableObject.CreateInstance("StartNode");
			startNode.winRect = new Rect(10, position.height/2 - 30, 180, 60);
			windows.Add(startNode);
		}

		if (e.button == 2 && !makeTransitionMode)
		{
			if (e.type == EventType.mouseDown) initMousePos = e.mousePosition;
			else if (e.type == EventType.mouseDrag)
			{
				Vector2 deltaMousePos = e.mousePosition - initMousePos;
				for (int i=0; i<windows.Count; i++)
					if (windows[i] != null) windows[i].winRect = new Rect(windows[i].winRect.x + deltaMousePos.x, windows[i].winRect.y + deltaMousePos.y, windows[i].winRect.width, windows[i].winRect.height);
				initMousePos = e.mousePosition;
				Repaint();
			}
		}
		else if (e.button == 1 && !makeTransitionMode && e.type == EventType.MouseDown)
		{
			GenericMenu menu = new GenericMenu();
			if (!GetWinClicked())
			{
				menu.AddItem(new GUIContent("Main Menu"), false, ContextCallback, "mainmenuNode");
			}
			else
			{
				selectedNode = windows[selectedIndex];
				lastTRect = selectedNode.ClickedOnRect(new Vector2(mousePos.x, mousePos.y));
				if (lastTRect != selectedNode.winRect || selectedNode.ifMsgNode) menu.AddItem(new GUIContent("Make Transition"), false, ContextCallback, "makeTransition");
				if (selectedIndex != 0) menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "deleteNode");
			}
			menu.ShowAsContext();
			e.Use();
		}
		else if (e.button == 0 && e.type == EventType.MouseDown && makeTransitionMode)
		{			
			if (GetWinClicked() && !windows[selectedIndex].Equals(selectedIndex))
			{
				BaseNode n = windows[selectedIndex];
				n.SetInput((BaseNode)selectedNode, mousePos);
				if (GetWinClicked() && n.rootNode != null) n.rootNode.SetAttributeNode(lastTRect, (BaseNode)n);
				makeTransitionMode = false;
				selectedNode = null;
			}

			if (!GetWinClicked())
			{
				makeTransitionMode = false;
				selectedNode = null;
			}

			e.Use();
		}

		if (makeTransitionMode && selectedNode != null)
		{			
			Rect mouseRect = new Rect(e.mousePosition.x, e.mousePosition.y, 10,10);
			DrawNodeCurve(lastTRect, mouseRect);
			Repaint();
		}
		
		BeginWindows();
		for (int i=0; i<windows.Count; i++)
			if (windows[i] != null) windows[i].winRect = GUI.Window(i, windows[i].winRect, DrawNodeWindow, windows[i].winTitle);		
		EndWindows();

		foreach(BaseNode n in windows)
			if (n!= null) n.DrawCurves();
	}

	void DrawNodeWindow(int id)
	{
		windows[id].DrawWindow();		
		GUI.DragWindow();
	}

	void ContextCallback(object obj)
	{
		string callback = obj.ToString();
		if (callback.Equals("mainmenuNode"))
		{
			MainMenuNode mainmenuNode = (MainMenuNode)ScriptableObject.CreateInstance("MainMenuNode");
			mainmenuNode.setWidth(mousePos.x, mousePos.y, 200);
			windows.Add(mainmenuNode);
		}
		else if (callback.Equals("makeTransition"))
		{
			if (GetWinClicked()) makeTransitionMode = true;
		}
		else if (callback.Equals("deleteNode"))
		{
			if (GetWinClicked())
			{
				BaseNode selNode = windows[selectedIndex];
				windows.RemoveAt(selectedIndex);
				foreach(BaseNode n in windows) n.NodeDeleted(selNode);
			}
		}
	}

	public bool GetWinClicked()
	{
		for (int i=0; i<windows.Count; i++)
		{
			if (windows[i] != null)
			{
				if (windows[i].winRect.Contains(mousePos))
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
		Texture2D bg = new Texture2D(1, 1, TextureFormat.RGBA32, false);
		bg.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f));
		bg.Apply();
		GUI.DrawTexture(new Rect(0, 0, position.width, position.height), bg);
		Handles.BeginGUI();
		Handles.color = new Color(0.7f, 0.7f, 0.7f, 0.1f);
		for (int i=0; i*60<=position.width; i++) Handles.DrawLine(new Vector3(60*i, 0), new Vector3(60*i, position.height));
		for (int i=0; i*60<=position.height; i++) Handles.DrawLine(new Vector3(0, 60*i), new Vector3(position.width, 60*i));
		Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.1f);
		for (int i=0; i*20<=position.width; i++)
			if (i%3 != 0) Handles.DrawLine(new Vector3(20*i, 0), new Vector3(20*i, position.height));
		for (int i=0; i*20<=position.height; i++)
			if (i%3 != 0) Handles.DrawLine(new Vector3(0, 20*i), new Vector3(position.width, 20*i));
		Handles.EndGUI();
	}

	public static void DrawNodeCurve(Rect start, Rect end)
	{
		Vector3 startPos = new Vector3(start.x + start.width - 10, start.y + start.height/2, 0);
		Vector3 endPos = new Vector3(end.x + end.width, end.y + end.height/2, 0);
		Vector3 startTan = startPos + (Vector3.right * 50);
		Vector3 endTan = endPos + (Vector3.left * 50);
		Color shadowCol = new Color(0, 0, 0, 0.06f);

		for (int i=0; i<3; i++) Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i+1)*5);
		Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 2);
	}
}
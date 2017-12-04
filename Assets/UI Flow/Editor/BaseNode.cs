using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseNode : ScriptableObject
{
	public Rect winRect;
	public string winTitle = "";
	public Texture2D winIcon;
	public Texture2D curveJoinR;
	public Texture2D curveJoinL;
	public bool ifMsgNode = false;
	public bool ifConnected = false;
	public BaseNode rootNode;
	public List<BaseComponents> attributes = new List<BaseComponents>();
    
	public class BaseComponents
    {
        public BaseNode node;
        public Rect rect;
        public string txt;

        public BaseComponents(string _txt)
        {
            txt = _txt;
            rect = new Rect();
        }
    }

	public virtual void DrawWindow()
	{
		GUI.skin = NodeEditor.skin;
		winIcon = EditorGUIUtility.Load("markPanel.png") as Texture2D;
		curveJoinR = EditorGUIUtility.Load("joinR.png") as Texture2D;
		curveJoinL = EditorGUIUtility.Load("joinL.png") as Texture2D;

		if (!ifMsgNode)
		{
			GUI.DrawTexture(new Rect(winRect.width - 34, 14, 20, 20), winIcon, ScaleMode.ScaleToFit, true);
			Handles.BeginGUI();
			Handles.color = new Color(0.4f, 0.4f, 0.4f, 1);
			Handles.DrawLine(new Vector3(8, 38), new Vector2(winRect.width - 8, 38));
			Handles.EndGUI();
			GUILayout.Space(5);
		}
	}

	public virtual Rect ClickedOnRect(Vector2 pos) { return winRect; }

	public virtual void DrawCurves() { }

	public virtual void SetAttributeNode(Rect rect, BaseNode node)
	{
		rect.x -= winRect.x;
		rect.y -= winRect.y;
		for (int i=0; i<attributes.Count; i++)
			if (attributes[i].rect.Equals(rect)) attributes[i].node = node;
	}

	public virtual void SetInput(BaseNode input, Vector2 clickPos)
	{
		clickPos.x -= winRect.x;
		clickPos.y -= winRect.y;

        Rect titleRect = new Rect(0, 0, winRect.width, 60);
		if (titleRect.Contains(clickPos) && !input.ifConnected)
		{
			if (input.ifMsgNode) input.ifConnected = true;
			rootNode = input;
		}
	}

	public virtual void NodeDeleted(BaseNode node)
	{
		if (node.rootNode != null)
		{
			BaseNode sNode = node.rootNode;
			if (sNode.ifMsgNode) sNode.ifConnected = false;
			for (int i=0; i<sNode.attributes.Count; i++)
				if(sNode.attributes[i].node == node) sNode.attributes[i].node = null;
		}
	}

	public virtual void setWidth(float x, float y, float width)
	{
		winRect.x = x;
		winRect.y = y;
		winRect.width = width;
		winRect.height += 60;
	}
}

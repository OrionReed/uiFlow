using System.Collections;
using UnityEngine;
using UnityEditor;

public class StartNode : BaseNode
{	
	public StartNode ()
    {
        ifMsgNode = true;
    }
	public override void DrawWindow()
	{
		base.DrawWindow();		
		var style = GUI.skin.GetStyle("Label");
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = 18;
		EditorGUI.LabelField(new Rect(0, 0, winRect.width, winRect.height), "START", style);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MainMenuNode : BaseNode
{    
	public MainMenuNode ()
    {
        winTitle = "MENU" + NodeEditor.windows.Count;
        
        attributes.Add(new BaseComponents("OPTION1"));
        attributes.Add(new BaseComponents("OPTION2"));
        attributes.Add(new BaseComponents("OPTION3"));
        attributes.Add(new BaseComponents("OPTION4"));
        attributes.Add(new BaseComponents("OPTION5"));
        attributes.Add(new BaseComponents("OPTION6"));
        winRect.height += 18*attributes.Count;
    }

    public override void DrawWindow()
	{
        base.DrawWindow();
        Event e = Event.current;

        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.alignment = TextAnchor.UpperRight;
        style.richText = true;
        style.normal.textColor = new Color(1, 1, 1, 0.5f);

        for (int i=0; i<attributes.Count; i++)
        {
            string txt = attributes[i].txt;
            string disTxt = (attributes[i].node) ? txt : "<color=#5E5E5EFF>" + txt + "</color>";
            GUILayout.Label(disTxt, style);
            if (e.type == EventType.Repaint) attributes[i].rect = GUILayoutUtility.GetLastRect();
        }
    }

    public override Rect ClickedOnRect(Vector2 pos)
    {
        for (int i=0; i<attributes.Count; i++)
        {
            Rect trect = attributes[i].rect;
            trect.x += winRect.x;
            trect.y += winRect.y;

            if (trect.Contains(pos)) return trect;
        }

        return winRect;
    }

    public override void SetInput(BaseNode input, Vector2 clickPos)
	{
        base.SetInput(input, clickPos);
    }

    public override void DrawCurves()
    {
        if (rootNode && rootNode.ifMsgNode)
        {
            Rect titleRct = new Rect(winRect.x + 9, winRect.y + 18, 12, 12);
            Rect src = rootNode.winRect;
            NodeEditor.DrawNodeCurve(src, new Rect(titleRct.x - 10, titleRct.y, titleRct.width, titleRct.height));
            GUI.DrawTexture(new Rect(src.x + src.width - 21, src.y + src.height/2 - 6, 12, 12), curveJoinR);
            GUI.DrawTexture(titleRct, curveJoinL);
        }
        for (int i=0; i<attributes.Count; i++)
        {
            if (attributes[i].node)
            {
                Rect tmpRct = attributes[i].rect;
                tmpRct.x += winRect.x + 26;
                tmpRct.y += winRect.y;
                Rect destRct = attributes[i].node.winRect;
                destRct.x -= destRct.width - 9;
                destRct.y -= destRct.height/2 - 25;
                GUI.DrawTexture(new Rect(tmpRct.x + tmpRct.width - 22, tmpRct.y + 3, 12, 12), curveJoinR);
                GUI.DrawTexture(new Rect(destRct.x + destRct.width, destRct.y + destRct.height/2 - 6, 12, 12), curveJoinL);
                NodeEditor.DrawNodeCurve(tmpRct, destRct);
            }
        }
    }
}

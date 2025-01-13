using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class IntersectionDrawer 
    {
        public delegate void IntersectionClicked(GenericIntersectionSettings clickedIntersection);
        public static event IntersectionClicked onIntersectionClicked;
        static void TriggetIntersectionClickedEvent(GenericIntersectionSettings clickedIntersection)
        {
            SettingsWindow.SetSelectedIntersection(clickedIntersection);
            if (onIntersectionClicked != null)
            {
                onIntersectionClicked(clickedIntersection);
            }
        }

        private static GUIStyle style = new GUIStyle();


        internal static void DrawIntersection(GenericIntersectionSettings intersection, Color color, List<IntersectionStopWaypointsSettings> stopWaypoints, Color stopWaypointsColor, Color textColor, List<WaypointSettings> exitWaypoints = null, Color exitWaypointsColor = new Color())
        {
            if (GleyUtilities.IsPointInViewWithValidation(intersection.transform.position))
            {
                Handles.color = color;
                if (Handles.Button(intersection.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 1f, 1f, Handles.DotHandleCap))
                {
                    TriggetIntersectionClickedEvent(intersection);
                }
                style.normal.textColor = color;
                Handles.Label(intersection.transform.position, "\n" + intersection.name, style);
                for (int i = 0; i < stopWaypoints.Count; i++)
                {
                    DrawStopWaypoints(stopWaypoints[i].roadWaypoints, stopWaypointsColor, i + 1, textColor);
                }

                if (exitWaypoints != null)
                {
                    Handles.color = exitWaypointsColor;
                    for (int i = 0; i < exitWaypoints.Count; i++)
                    {
                        if (exitWaypoints[i] != null)
                        {
                            Handles.DrawSolidDisc(exitWaypoints[i].transform.position, Vector3.up, 1);
                        }
                        else
                        {
                            exitWaypoints.RemoveAt(i);
                        }
                    }
                }
            }
        }


        static void DrawStopWaypoints(List<WaypointSettings> stopWaypoints, Color stopWaypointsColor, int road, Color textColor)
        {
            Handles.color = stopWaypointsColor;
            GUIStyle centeredStyle = new GUIStyle();
            centeredStyle.alignment = TextAnchor.UpperRight;
            centeredStyle.normal.textColor = textColor;
            centeredStyle.fontStyle = FontStyle.Bold;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                if (stopWaypoints[i] != null)
                {
                    Handles.DrawSolidDisc(stopWaypoints[i].transform.position, Vector3.up, 1);
                    Handles.Label(stopWaypoints[i].transform.position, road.ToString(), centeredStyle);
                }
                else
                {
                    stopWaypoints.RemoveAt(i);
                }
            }
        }


        internal static void DrawIntersectionWaypoint(WaypointSettingsBase waypoint, Color drawColor, int road, Color textColor, float size)
        {
            if (waypoint != null)
            {
                Handles.color = drawColor;
                Handles.DrawSolidDisc(waypoint.transform.position, Vector3.up, size);
                if (road != -1)
                {
                    GUIStyle centeredStyle = new GUIStyle();
                    centeredStyle.alignment = TextAnchor.UpperRight;
                    centeredStyle.normal.textColor = textColor;
                    centeredStyle.fontStyle = FontStyle.Bold;
                    Handles.Label(waypoint.transform.position, road.ToString(), centeredStyle);
                }
            }
        }


        internal static void DrawListWaypoints<T>(List<T> waypointList, Color drawColor, int road, Color textColor, float size) where T : WaypointSettingsBase
        {
            for (int i = 0; i < waypointList.Count; i++)
            {
                if (waypointList[i] != null)
                {
                    if (waypointList[i].draw)
                    {
                        DrawIntersectionWaypoint(waypointList[i], drawColor, road, textColor, size);
                    }
                }
            }
        }
    }
}

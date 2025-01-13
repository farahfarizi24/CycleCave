#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Internal;
#endif
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Constants = Gley.UrbanAssets.Internal.Constants;

namespace Gley.UrbanAssets.Editor
{
    public class LaneDrawer
    {
        static GUIStyle style = new GUIStyle();


        public static void DrawAllLanes(RoadBase road, bool drawWaypoints, bool drawLaneChange, Color laneColor, Color waypointColor, Color disconnectedColor, Color laneChangeColor, Color textColor)
        {
            Transform lanesHolder = road.transform.Find(Constants.lanesHolderName);
            if (lanesHolder)
            {
                for (int i = 0; i < lanesHolder.childCount; i++)
                {
                    if (!drawWaypoints)
                    {
                        DrawSimplifiedLane(lanesHolder.GetChild(i), laneColor, textColor, true, false);
                    }
                    else
                    {
                        DrawLane(lanesHolder.GetChild(i), drawLaneChange, waypointColor, disconnectedColor, laneChangeColor, false);
                    }
                }
            }
        }


        public static void DrawPedestrianLanes(RoadBase road, bool drawWaypoints, Color laneColor, Color waypointColor, Color disconnectedColor, Color textColor)
        {
            Transform lanesHolder = road.transform.Find(Constants.lanesHolderName);
            if (lanesHolder)
            {
                for (int i = 0; i < lanesHolder.childCount; i++)
                {
                    if (!drawWaypoints)
                    {
                        DrawSimplifiedLane(lanesHolder.GetChild(i), laneColor, textColor, false, true);
                    }
                    else
                    {
                        DrawLane(lanesHolder.GetChild(i), false, waypointColor, disconnectedColor, default, true);
                    }
                }
            }
        }


        public static void DrawLane(Transform holder, bool drawLaneChange, Color waypointColor, Color disconnectedColor, Color laneChangeColor, bool square)
        {
            if (holder != null)
            {
                for (int i = 0; i < holder.childCount; i++)
                {
                    WaypointSettingsBase waypointScript = holder.GetChild(i).GetComponent<WaypointSettingsBase>();
                    if (waypointScript != null)
                    {
                        if (waypointScript.neighbors.Count == 0 || waypointScript.prev.Count == 0)
                        {
                            DrawUnconnectedWaypoint(waypointScript.transform.position, disconnectedColor);
                        }

                        if (drawLaneChange)
                        {
                            for (int j = 0; j < waypointScript.otherLanes.Count; j++)
                            {
                                if (waypointScript.otherLanes[j] != null)
                                {
                                    DrawTriangle(waypointScript.transform.position, waypointScript.otherLanes[j].transform.position, laneChangeColor, true, square);
                                }
                                else
                                {
                                    for (int k = waypointScript.otherLanes.Count - 1; k >= 0; k--)
                                    {
                                        if (waypointScript.otherLanes[k] == null)
                                        {
                                            waypointScript.otherLanes.RemoveAt(k);
                                        }
                                    }
                                }
                            }
                        }

                        for (int j = 0; j < waypointScript.neighbors.Count; j++)
                        {

                            if (waypointScript.neighbors[j] != null)
                            {
                                DrawTriangle(waypointScript.transform.position, waypointScript.neighbors[j].transform.position, waypointColor, true, square);
                            }
                            else
                            {
                                for (int k = waypointScript.neighbors.Count - 1; k >= 0; k--)
                                {
                                    if (waypointScript.neighbors[k] == null)
                                    {
                                        waypointScript.neighbors.RemoveAt(k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        private static void DrawSimplifiedLane(Transform laneHolder, Color laneColor, Color textColor, bool drawLabels, bool square)
        {
            WaypointSettingsBase waypointScript;
            for (int i = 0; i < laneHolder.childCount; i++)
            {
                waypointScript = laneHolder.GetChild(i).GetComponent<WaypointSettingsBase>();
                for (int j = 0; j < waypointScript.neighbors.Count; j++)
                {
                    if (waypointScript.neighbors[j] != null)
                    {
                        DrawWaypointLine(waypointScript.transform.position, waypointScript.neighbors[j].transform.position, laneColor);
                    }
                }
                if (drawLabels)
                {
                    if (i == 0 || i == laneHolder.childCount - 1)
                    {
                        DrawLabel(waypointScript.transform.position, waypointScript.transform.parent.name, textColor);

                        if (waypointScript.neighbors.Count == 0)
                        {
                            if (waypointScript.prev.Count > 0)
                            {
                                DrawTriangle(waypointScript.prev[0].transform.position, waypointScript.transform.position, laneColor, false, square);
                            }
                        }

                        for (int j = 0; j < waypointScript.neighbors.Count; j++)
                        {
                            if (waypointScript.neighbors[j] == null)
                            {
                                waypointScript.neighbors.RemoveAt(j);
                            }
                            else
                            {
                                DrawTriangle(waypointScript.transform.position, waypointScript.neighbors[j].transform.position, laneColor, false, square);
                            }
                        }
                    }
                }
            }
        }


        private static void DrawLabel(Vector3 position, string text, Color color)
        {
            style.normal.textColor = color;
            Handles.Label(position, text, style);
        }


        private static void DrawUnconnectedWaypoint(Vector3 position, Color disconnectedColor)
        {
            Handles.color = disconnectedColor;
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(Vector3.up), 10, EventType.Repaint);
        }


        private static void DrawTriangle(Vector3 start, Vector3 end, Color waypointColor, bool drawLane, bool square)
        {
            Handles.color = waypointColor;

            if (square)
            {
                Handles.DrawWireCube(start, new Vector3(0.5f, 0, 0.5f));
            }
            else
            {
                Vector3 direction = (start - end).normalized;

                Vector3 point2 = end + Quaternion.Euler(0, -25, 0) * direction;

                Vector3 point3 = end + Quaternion.Euler(0, 25, 0) * direction;

                Handles.DrawPolyLine(end, point2, point3, end);
            }

            if (drawLane)
            {
                Handles.DrawLine(start, end);
            }
        }


        private static void DrawWaypointLine(Vector3 start, Vector3 end, Color color)
        {
            Handles.color = color;
            Handles.DrawLine(start, end);
        }
#if GLEY_PEDESTRIAN_SYSTEM
        internal static void DrawPathWidth(Transform laneHolder, Color roadColor)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector3> left = new List<Vector3>();
            List<Vector3> right = new List<Vector3>();
            float laneWidth = 0;
            Handles.color = roadColor;
            if (laneHolder)
            {
                for (int i = 0; i < laneHolder.childCount; i++)
                {
                    points.Add(laneHolder.GetChild(i).transform.position);

                    var waypoint = laneHolder.GetChild(i).GetComponent<PedestrianWaypointSettings>();
                    laneWidth = waypoint.laneWidth;
                    Handles.DrawLine(waypoint.transform.position + waypoint.left * laneWidth / 2, waypoint.transform.position - waypoint.left * laneWidth / 2);

                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                Vector3 forward = Vector3.zero;
                if (i < points.Count - 1)
                {
                    forward += points[i + 1] - points[i];
                }
                if (i > 0)
                {
                    forward += points[i] - points[i - 1];
                }
                forward.Normalize();

                Vector3 leftVector = Vector3.Cross(Vector3.up, forward).normalized;

                left.Add(points[i] + leftVector * laneWidth * 0.5f);
                right.Add(points[i] - leftVector * laneWidth * 0.5f);
            }

           
            for (int i = 0; i < left.Count - 1; i++)
            {
                Handles.DrawDottedLine(left[i], left[i + 1], 10);
            }
            for (int i = 0; i < right.Count - 1; i++)
            {
                Handles.DrawDottedLine(right[i], right[i + 1], 10);
            }
        }
#endif
    }
}

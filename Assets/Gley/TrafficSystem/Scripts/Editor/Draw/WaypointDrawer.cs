using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class WaypointDrawer : UnityEditor.Editor
    {
        private WaypointSettings[] allWaypoints;
        private List<WaypointSettings> speedEditedWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> penaltyEditedWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> priorityEditedWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> giveWayWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> complexGiveWayWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> eventWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> zipperGiveWayWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> pathProblems = new List<WaypointSettings>();
        private List<WaypointSettings> vehicleEditedWaypoints = new List<WaypointSettings>();
        private List<WaypointSettings> disconnectedWaypoints = new List<WaypointSettings>();
        private Dictionary<VehicleTypes, string> vehicleTypesToString = new Dictionary<VehicleTypes, string>();
        private Dictionary<int, string> priorityToString = new Dictionary<int, string>();
        private Dictionary<int, string> speedToString = new Dictionary<int, string>();
        private GUIStyle speedStyle;
        private GUIStyle carsStyle;
        private GUIStyle priorityStyle;
        private Quaternion towardsCamera;
        private Quaternion arrowSide1 = Quaternion.Euler(0, -25, 0);
        private Quaternion arrowSide2 = Quaternion.Euler(0, 25, 0);
        private Vector3 direction;
        private bool colorChanged;

        
        internal delegate void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick);
        internal event WaypointClicked onWaypointClicked;
        void TriggerWaypointClickedEvent(WaypointSettings clickedWaypoint, bool leftClick)
        {
            SettingsWindow.SetSelectedWaypoint(clickedWaypoint);
            if (onWaypointClicked != null)
            {
                onWaypointClicked(clickedWaypoint, leftClick);
            }
        }


        internal WaypointDrawer Initialize()
        {
            speedStyle = new GUIStyle();
            carsStyle = new GUIStyle();
            priorityStyle = new GUIStyle();
            vehicleTypesToString = new Dictionary<VehicleTypes, string>();
            var allTypes = Enum.GetValues(typeof(VehicleTypes)).Cast<VehicleTypes>();
            GleyUtilities.ForceRefresh();
            foreach (var vehicleType in allTypes)
            {
                vehicleTypesToString.Add(vehicleType, vehicleType.ToString());
            }
            LoadWaypoints();
            return this;
        }


        #region Draw Waypoints 
        internal void DrawWaypointsForLink(WaypointSettings currentWaypoint, List<WaypointSettingsBase> neighborsList, List<WaypointSettingsBase> otherLinesList, Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i] != currentWaypoint && !neighborsList.Contains(allWaypoints[i]) && !otherLinesList.Contains(allWaypoints[i]))
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }
        }

        internal void ShowIntersectionWaypoints(Color waypointColor)
        {
            ShowAllWaypoints(waypointColor, true, false, default, false, default, false, default, false, default);
        }


        internal void ShowAllWaypoints(Color waypointColor, bool showConnections, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool showPriority, Color priorityColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            if (showSpeed)
            {
                speedStyle.normal.textColor = speedColor;
            }
            if (showCars)
            {
                carsStyle.normal.textColor = carsColor;
            }
            if (showPriority)
            {
                priorityStyle.normal.textColor = priorityColor;
            }

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    DrawCompleteWaypoint(allWaypoints[i], showConnections, waypointColor, showSpeed, speedColor, showCars, carsColor, drawOtherLanes, otherLanesColor, showPriority, priorityColor, false, default, true);
                }
            }
        }


        internal void ShowWaypointsWithPenalty(int penalty, Color color)
        {
            colorChanged = true;
            UpdateInViewProperty();

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].penalty == penalty)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowWaypointsWithPriority(int priority, Color color)
        {
            colorChanged = true;
            UpdateInViewProperty();

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].priority == priority)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowWaypointsWithVehicle(int vehicle, Color color)
        {
            colorChanged = true;
            UpdateInViewProperty();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].allowedCars.Contains((VehicleTypes)vehicle))
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal void ShowWaypointsWithSpeed(int speed, Color color)
        {
            colorChanged = true;
            UpdateInViewProperty();

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    if (allWaypoints[i].maxSpeed == speed)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], true, color, false, default, false, default, false, default, false, default, false, default, false);
                    }
                }
            }
        }


        internal List<WaypointSettings> ShowDisconnectedWaypoints(Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].neighbors.Count == 0)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }

            if (nr != disconnectedWaypoints.Count)
            {
                UpdateDisconnectedWaypoints();
            }
            return disconnectedWaypoints;
        }


        internal List<WaypointSettings> ShowVehicleEditedWaypoints(Color waypointColor, Color carsColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            carsStyle.normal.textColor = carsColor;
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].carsLocked)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, true, default, false, default, false, default, false, default, true);
                    }
                }
            }

            if (nr != vehicleEditedWaypoints.Count)
            {
                UpdateVehicleEditedWaypoints();
            }

            return vehicleEditedWaypoints;
        }


        internal List<WaypointSettings> ShowSpeedEditedWaypoints(Color waypointColor, Color speedColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            speedStyle.normal.textColor = speedColor;
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].speedLocked)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, true, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }
            if (nr != speedEditedWaypoints.Count)
            {
                UpdateSpeedEditedWaypoints();
            }

            return speedEditedWaypoints;
        }


        internal List<WaypointSettings> ShowPriorityEditedWaypoints(Color waypointColor, Color priorityColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            priorityStyle.normal.textColor = priorityColor;
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].priorityLocked)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, true, default, false, default, true);
                    }
                }
            }
            if (nr != priorityEditedWaypoints.Count)
            {
                UpdatePriorityEditedWaypoints();
            }

            return priorityEditedWaypoints;
        }


        internal List<WaypointSettings> ShowGiveWayWaypoints(Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].giveWay)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }

            if (nr != giveWayWaypoints.Count)
            {
                UpdateGiveWayWaypoints();
            }

            return giveWayWaypoints;
        }


        internal List<WaypointSettings> ShowComplexGiveWayWaypoints(Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].complexGiveWay)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }

            if (nr != complexGiveWayWaypoints.Count)
            {
                UpdateComplexGiveWayWaypoints();
            }

            return complexGiveWayWaypoints;
        }


        internal List<WaypointSettings> ShowZipperGiveWayWaypoints(Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].zipperGiveWay)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }

            if (nr != zipperGiveWayWaypoints.Count)
            {
                UpdateZipperGiveWayWaypoints();
            }

            return zipperGiveWayWaypoints;
        }


        internal List<WaypointSettings> ShowEventWaypoints(Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].triggerEvent)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                        Handles.Label(allWaypoints[i].position, (allWaypoints[i]).eventData);
                    }
                }
            }

            if (nr != eventWaypoints.Count)
            {
                UpdateEventWaypoints();
            }

            return eventWaypoints;
        }


        internal List<WaypointSettings> ShowVehiclePathProblems(Color waypointColor, Color carsColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            carsStyle.normal.textColor = carsColor;
            pathProblems = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].inView)
                {
                    int nr = allWaypoints[i].allowedCars.Count;
                    for (int j = 0; j < allWaypoints[i].allowedCars.Count; j++)
                    {
                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            if (((WaypointSettings)allWaypoints[i].neighbors[k]).allowedCars.Contains(allWaypoints[i].allowedCars[j]))
                            {
                                nr--;
                                break;
                            }
                        }
                    }
                    if (nr != 0)
                    {
                        pathProblems.Add(allWaypoints[i]);
                        DrawCompleteWaypoint(allWaypoints[i], true, waypointColor, false, default, true, default, false, default, false, default, false, default, true);

                        for (int k = 0; k < allWaypoints[i].neighbors.Count; k++)
                        {
                            for (int j = 0; j < ((WaypointSettings)allWaypoints[i].neighbors[k]).allowedCars.Count; j++)
                            {
                                DrawCompleteWaypoint((WaypointSettings)allWaypoints[i].neighbors[k], false, waypointColor, false, default, true, default, false, default, false, default, false, default, true);
                            }
                        }
                    }
                }

            }
            return pathProblems;
        }


        internal List<WaypointSettings> ShowPenaltyEditedWaypoints(Color waypointColor)
        {
            colorChanged = true;
            UpdateInViewProperty();
            int nr = 0;
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].penaltyLocked)
                {
                    nr++;
                    if (allWaypoints[i].inView)
                    {
                        DrawCompleteWaypoint(allWaypoints[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
                    }
                }
            }
            if (nr != penaltyEditedWaypoints.Count)
            {
                UpdatePenaltyEditedWaypoints();
            }

            return penaltyEditedWaypoints;
        }


        internal WaypointSettings[] GetAllWaypoints()
        {
            return allWaypoints;
        }


        internal void DrawCurrentWaypoint(WaypointSettings waypoint, Color selectedColor, Color waypointColor, Color otherLaneColor, Color prevColor, Color giveWayColor)
        {
            colorChanged = true;

            DrawCompleteWaypoint(waypoint, true, selectedColor, false, default, false, default, true, otherLaneColor, false, default, true, prevColor, true);

            for (int i = 0; i < waypoint.neighbors.Count; i++)
            {
                DrawCompleteWaypoint((WaypointSettings)waypoint.neighbors[i], false, waypointColor, false, default, false, default, false, default, false, default, false, default, true);
            }

            colorChanged = true;
            for (int i = 0; i < waypoint.prev.Count; i++)
            {
               
                DrawCompleteWaypoint((WaypointSettings)waypoint.prev[i], false, prevColor, false, default, false, default, false, default, false, default, false, default, true);
            }

            colorChanged = true;
            for (int i = 0; i < waypoint.otherLanes.Count; i++)
            {
               
                DrawCompleteWaypoint((WaypointSettings)waypoint.otherLanes[i], false, otherLaneColor, false, default, false, default, false, default, false, default, false, default, true);
            }

            colorChanged = true;
            for (int i = 0; i < waypoint.giveWayList.Count; i++)
            {
               
                DrawCompleteWaypoint((WaypointSettings)waypoint.giveWayList[i], true, giveWayColor, false, default, false, default, false, default, false, default, false, default, true);
            }
        }


        internal void DrawSelectedWaypoint(WaypointSettings selectedWaypoint, Color color)
        {
            //Handles.color = color;
            //Handles.CubeHandleCap(0, selectedWaypoint.transform.position, Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up), 1, EventType.Repaint);
            colorChanged = true;
            DrawCompleteWaypoint(selectedWaypoint, false, color, false, default, false, default, false, default, false, default, false, default, false);
        }
        #endregion


        #region LoadWaypoints
        private void LoadWaypoints()
        {
            InitializeWaypoints();
            UpdateInViewProperty();
        }


        private void InitializeWaypoints()
        {
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                allWaypoints = FindObjectsByType<WaypointSettings>(FindObjectsSortMode.None);
            }
            else
            {
                allWaypoints = GleyPrefabUtilities.GetScenePrefabRoot().GetComponentsInChildren<WaypointSettings>();
            }


            for (int i = 0; i < allWaypoints.Length; i++)
            {
                allWaypoints[i].position = allWaypoints[i].transform.position;

                if (!priorityToString.ContainsKey(allWaypoints[i].priority))
                {
                    // Key doesn't exist, add the key-value pair
                    priorityToString.Add(allWaypoints[i].priority, allWaypoints[i].priority.ToString());
                }

                if (!speedToString.ContainsKey(allWaypoints[i].maxSpeed))
                {
                    // Key doesn't exist, add the key-value pair
                    speedToString.Add(allWaypoints[i].maxSpeed, allWaypoints[i].maxSpeed.ToString());
                }

                if (allWaypoints[i].neighbors == null)
                {
                    allWaypoints[i].neighbors = new List<WaypointSettingsBase>();
                }
                //check for null assignments;
                for (int j = allWaypoints[i].neighbors.Count - 1; j >= 0; j--)
                {
                    if (allWaypoints[i].neighbors[j] == null)
                    {
                        allWaypoints[i].neighbors.RemoveAt(j);
                    }
                }

                if (allWaypoints[i].prev == null)
                {
                    allWaypoints[i].prev = new List<WaypointSettingsBase>();
                }

                for (int j = allWaypoints[i].prev.Count - 1; j >= 0; j--)
                {
                    if (allWaypoints[i].prev[j] == null)
                    {
                        allWaypoints[i].prev.RemoveAt(j);
                    }
                }

                if (allWaypoints[i].otherLanes == null)
                {
                    allWaypoints[i].otherLanes = new List<WaypointSettingsBase>();
                }

                for (int j = allWaypoints[i].otherLanes.Count - 1; j >= 0; j--)
                {
                    if (allWaypoints[i].otherLanes[j] == null)
                    {
                        allWaypoints[i].otherLanes.RemoveAt(j);
                    }
                }
            }
        }


        private void UpdateDisconnectedWaypoints()
        {
            disconnectedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].neighbors.Count == 0)
                {
                    disconnectedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateVehicleEditedWaypoints()
        {
            vehicleEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].carsLocked == true)
                {
                    vehicleEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateSpeedEditedWaypoints()
        {
            speedEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].speedLocked == true)
                {
                    speedEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdatePriorityEditedWaypoints()
        {
            priorityEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].priorityLocked == true)
                {
                    priorityEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateGiveWayWaypoints()
        {
            giveWayWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].giveWay == true)
                {
                    giveWayWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateComplexGiveWayWaypoints()
        {
            complexGiveWayWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].complexGiveWay == true)
                {
                    complexGiveWayWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateZipperGiveWayWaypoints()
        {
            zipperGiveWayWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].zipperGiveWay == true)
                {
                    zipperGiveWayWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdateEventWaypoints()
        {
            eventWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].triggerEvent == true)
                {
                    eventWaypoints.Add(allWaypoints[i]);
                }
            }
        }


        private void UpdatePenaltyEditedWaypoints()
        {
            penaltyEditedWaypoints = new List<WaypointSettings>();
            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (allWaypoints[i].penaltyLocked == true)
                {
                    penaltyEditedWaypoints.Add(allWaypoints[i]);
                }
            }
        }
        #endregion


        private void UpdateInViewProperty()
        {
            GleyUtilities.SetCamera();
            if (GleyUtilities.SceneCameraMoved())
            {
                for (int i = 0; i < allWaypoints.Length; i++)
                {
                    if (GleyUtilities.IsPointInViewNoValidation(allWaypoints[i].position))
                    {
                        allWaypoints[i].inView = true;
                    }
                    else
                    {
                        allWaypoints[i].inView = false;
                    }
                }
                if (Camera.current != null)
                {
                    towardsCamera = Quaternion.LookRotation(Camera.current.transform.forward, Camera.current.transform.up);
                }
            }
        }


        private void DrawCompleteWaypoint(WaypointSettings waypoint, bool showConnections, Color connectionColor, bool showSpeed, Color speedColor, bool showCars, Color carsColor, bool drawOtherLanes, Color otherLanesColor, bool showPriority, Color priorityColor, bool drawPrev, Color prevColor, bool showDirection)
        {
            SetHandleColor(connectionColor, colorChanged);
            if (colorChanged == true)
            {
                colorChanged = false;
            }
            //clickable button
            if (Handles.Button(waypoint.position, towardsCamera, 0.5f, 0.5f, Handles.DotHandleCap))
            {
                TriggerWaypointClickedEvent(waypoint, Event.current.button == 0);
            }

            if (showConnections)
            {
                for (int i = 0; i < waypoint.neighbors.Count; i++)
                {
                    DrawConnection(waypoint.position, waypoint.neighbors[i].position, showDirection);
                }
            }

            if (drawPrev)
            {
                colorChanged = true;
                SetHandleColor(prevColor, colorChanged);
                for (int i = 0; i < waypoint.prev.Count; i++)
                {
                    DrawConnection(waypoint.position, waypoint.prev[i].position, false);
                }
            }

            if (drawOtherLanes)
            {
                for (int i = 0; i < waypoint.otherLanes.Count; i++)
                {
                    colorChanged = true;
                    SetHandleColor(otherLanesColor, colorChanged);
                    DrawConnection(waypoint.position, waypoint.otherLanes[i].position, true);
                }
            }

            if (showSpeed)
            {
                ShowSpeed(waypoint);
            }
            if (showCars)
            {
                ShowCars(waypoint);
            }
            if (showPriority)
            {
                ShowPriority(waypoint);
            }
        }


        private void ShowCars(WaypointSettings waypoint)
        {
            string text = string.Empty;
            for (int j = 0; j < waypoint.allowedCars.Count; j++)
            {
                text += vehicleTypesToString[waypoint.allowedCars[j]] + "\n";
            }
            Handles.Label(waypoint.position, text, carsStyle);
        }


        private void ShowSpeed(WaypointSettings waypoint)
        {
            Handles.Label(waypoint.position, speedToString[waypoint.maxSpeed], speedStyle);
        }


        private void ShowPriority(WaypointSettings waypoint)
        {
            Handles.Label(waypoint.position, priorityToString[waypoint.priority], priorityStyle);
        }


        private void SetHandleColor(Color newColor, bool colorChanged)
        {
            if (colorChanged)
            {
                Handles.color = newColor;
            }
        }


        private void DrawConnection(Vector3 start, Vector3 end, bool drawDirection)
        {
            Handles.DrawLine(start, end);
            if (drawDirection)
            {
                direction = (start - end).normalized;
                Handles.DrawPolyLine(end, end + arrowSide1 * direction, end + arrowSide2 * direction, end);
            }
        }
    }
}

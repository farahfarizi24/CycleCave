#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Editor;
using Gley.PedestrianSystem.Internal;
#endif
using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class PriorityCrossingWindow : IntersectionWindowBase
    {
        private enum WindowActions
        {
            None,
            AssignRoadWaypoints,
            AssignPedestrianWaypoints,
            AddDirectionWaypoints,
        }

#if GLEY_PEDESTRIAN_SYSTEM
        private PedestrianWaypointDrawer pedestrianWaypointDrawer;
#endif
        private PriorityCrossingSettings selectedCrossing;
        private WindowActions currentAction;
        private float scrollAdjustment = 187;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            selectedIntersection = SettingsWindow.GetSelectedIntersection();
            selectedCrossing = selectedIntersection as PriorityCrossingSettings;
            stopWaypoints = selectedCrossing.enterWaypoints;
            exitWaypoints = selectedCrossing.exitWaypoints;
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypointDrawer = CreateInstance<PedestrianWaypointDrawer>();
            pedestrianWaypointDrawer.Initialize();
            pedestrianWaypointDrawer.onWaypointClicked += PedestrianWaypointClicked;
#endif
            if (stopWaypoints.Count == 0)
            {
                stopWaypoints.Add(new IntersectionStopWaypointsSettings());
            }
            currentAction = WindowActions.None;
            ISetupWindow baseInterface = base.Initialize(windowProperties, window);
            waypointDrawer.onWaypointClicked += TrafficWaypointClicked;
            return baseInterface;
        }


        private void TrafficWaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            switch (currentAction)
            {
                case WindowActions.AssignRoadWaypoints:
                    AddWaypointToList((WaypointSettings)clickedWaypoint, stopWaypoints[selectedRoad].roadWaypoints);
                    break;
            }
            SceneView.RepaintAll();
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }


#if GLEY_PEDESTRIAN_SYSTEM
        private void PedestrianWaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            switch (currentAction)
            {
                case WindowActions.AddDirectionWaypoints:
                    int road = GetRoadFromWaypoint((PedestrianWaypointSettings)clickedWaypoint);
                    AddWaypointToList((PedestrianWaypointSettings)clickedWaypoint, stopWaypoints[road].directionWaypoints);
                    break;
                case WindowActions.AssignPedestrianWaypoints:
                    AddWaypointToList((PedestrianWaypointSettings)clickedWaypoint, stopWaypoints[selectedRoad].pedestrianWaypoints);
                    break;
            }
            SceneView.RepaintAll();
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }
#endif

        public override void DrawInScene()
        {
            base.DrawInScene();
            switch (currentAction)
            {
                case WindowActions.None:
                    DrawExitWaypoints();
                    DrawStopWaypoints(true);
                    break;

                case WindowActions.AssignRoadWaypoints:
                    if (hideWaypoints == false)
                    {
                        waypointDrawer.ShowIntersectionWaypoints(save.waypointColor);
                    }
                    DrawStopWaypoints(false);
                    break;
#if GLEY_PEDESTRIAN_SYSTEM
                case WindowActions.AssignPedestrianWaypoints:
                    if (hideWaypoints == false)
                    {
                        pedestrianWaypointDrawer.DrawAllWaypoints(save.waypointColor, true, save.waypointColor, false, Color.white);
                    }
                    DrawStopWaypoints(true);
                    break;

                case WindowActions.AddDirectionWaypoints:
                    for (int i = 0; i < stopWaypoints.Count; i++)
                    {
                        for (int j = 0; j < stopWaypoints[i].pedestrianWaypoints.Count; j++)
                        {
                            pedestrianWaypointDrawer.DrawPossibleDirectionWaypoints(stopWaypoints[i].pedestrianWaypoints[j], save.waypointColor);
                        }
                        IntersectionDrawer.DrawListWaypoints(stopWaypoints[i].pedestrianWaypoints, intersectionSave.stopWaypointsColor, -1, Color.white, pedestrianWaypointSize);
                        IntersectionDrawer.DrawListWaypoints(stopWaypoints[i].directionWaypoints, intersectionSave.exitWaypointsColor, -1, Color.white, pedestrianWaypointSize);
                    }
                    break;
#endif
            }
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            switch (currentAction)
            {
                case WindowActions.None:
                    IntersectionOverview();
                    break;
                case WindowActions.AssignRoadWaypoints:
                    AddTrafficWaypoints();
                    break;
#if GLEY_PEDESTRIAN_SYSTEM
                case WindowActions.AddDirectionWaypoints:
                    AddDirectionWaypoints();
                    break;
                case WindowActions.AssignPedestrianWaypoints:
                    AddPedestrianWaypoints();
                    break;
#endif
            }
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


        private void IntersectionOverview()
        {
            ViewAssignedStopWaypoints();
#if GLEY_PEDESTRIAN_SYSTEM
            ViewDirectionWaypoints();
#endif
        }


        #region StopWaypoints
        private void ViewAssignedStopWaypoints()
        {

            Color oldColor;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
                "\nEach road that enters in intersection should have its own set of stop waypoints"));

                DisplayList(stopWaypoints[i].roadWaypoints, ref stopWaypoints[i].draw);

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Assign Road Waypoints"))
                {
                    selectedRoad = i;
                    currentAction = WindowActions.AssignRoadWaypoints;
                }

                oldColor = GUI.backgroundColor;
                if (stopWaypoints[i].draw == true)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View Road Waypoints"))
                {
                    ViewRoadWaypoints(i);
                }
                GUI.backgroundColor = oldColor;

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();

#if GLEY_PEDESTRIAN_SYSTEM
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("Pedestrian waypoints:");
                EditorGUILayout.Space();
                DisplayList(stopWaypoints[i].pedestrianWaypoints, ref stopWaypoints[i].draw);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Assign Pedestrian Waypoints"))
                {
                    selectedRoad = i;
                    currentAction = WindowActions.AssignPedestrianWaypoints;
                }

                oldColor = GUI.backgroundColor;
                if (intersectionSave.showPedestrians == true)
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View Pedestrian Waypoints"))
                {
                    ViewAllPedestrianWaypoints();
                }
                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
#endif
            }
        }

#if GLEY_PEDESTRIAN_SYSTEM
        private void ViewAllPedestrianWaypoints()
        {
            intersectionSave.showPedestrians = !intersectionSave.showPedestrians;
            for (int j = 0; j < stopWaypoints[0].pedestrianWaypoints.Count; j++)
            {
                stopWaypoints[0].pedestrianWaypoints[j].draw = intersectionSave.showPedestrians;
            }
        }
#endif

        private void AddTrafficWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Road " + (selectedRoad + 1));
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
            "\nEach road that enters in intersection should have its own set of stop waypoints"));

            DisplayList(stopWaypoints[selectedRoad].roadWaypoints, ref stopWaypoints[selectedRoad].draw);

            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (stopWaypoints[selectedRoad].draw == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Road Waypoints"))
            {
                ViewRoadWaypoints(selectedRoad);
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }

#if GLEY_PEDESTRIAN_SYSTEM
        private void AddPedestrianWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Road " + (selectedRoad + 1));
            EditorGUILayout.LabelField(new GUIContent("Pedestrian stop waypoints:", "pedestrians will stop at this point until the intersection allows them to continue. " +
            "\nEach crossing in intersection should have its own set of stop waypoints corresponding to its road"));

            DisplayList(stopWaypoints[selectedRoad].pedestrianWaypoints, ref stopWaypoints[selectedRoad].draw);

            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (stopWaypoints[selectedRoad].draw == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Road Waypoints"))
            {
                ViewRoadWaypoints(selectedRoad);
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }
#endif

        private void ViewRoadWaypoints(int i)
        {
            stopWaypoints[i].draw = !stopWaypoints[i].draw;
            for (int j = 0; j < stopWaypoints[i].roadWaypoints.Count; j++)
            {
                stopWaypoints[i].roadWaypoints[j].draw = stopWaypoints[i].draw;
            }
        }
        #endregion


#if GLEY_PEDESTRIAN_SYSTEM
        #region DirectionWaypoints
        private void ViewDirectionWaypoints()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Crossing direction waypoints:", "For each stop waypoint a direction needs to be specified\n" +
                "Only if a pedestrian goes to that direction it will stop, otherwise it will pass through stop waypoint"));
            EditorGUILayout.Space();

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                DisplayList(stopWaypoints[i].directionWaypoints, ref intersectionSave.showDirection);
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Direction Waypoints"))
            {
                selectedRoad = -1;
                currentAction = WindowActions.AddDirectionWaypoints;
            }
            Color oldColor = GUI.backgroundColor;
            if (intersectionSave.showDirection == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Direction Waypoints"))
            {
                ViewAllDirectionWaypoints();
            }
            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void AddDirectionWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Crossing direction waypoints:", "For each stop waypoint a direction needs to be specified\n" +
                "Only if a pedestrian goes to that direction it will stop, otherwise it will pass through stop waypoint"));
            EditorGUILayout.Space();

            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                DisplayList(stopWaypoints[i].directionWaypoints, ref intersectionSave.showDirection);
            }
            EditorGUILayout.Space();

            Color oldColor = GUI.backgroundColor;
            if (intersectionSave.showDirection == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Direction Waypoints"))
            {
                ViewAllDirectionWaypoints();
            }
            GUI.backgroundColor = oldColor;

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }


        private void ViewAllDirectionWaypoints()
        {
            intersectionSave.showDirection = !intersectionSave.showDirection;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                for (int j = 0; j < stopWaypoints[i].directionWaypoints.Count; j++)
                {
                    stopWaypoints[i].directionWaypoints[j].draw = intersectionSave.showDirection;
                }
            }
        }
        #endregion


        private int GetRoadFromWaypoint(PedestrianWaypointSettings waypoint)
        {
            int road = -1;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                for (int j = 0; j < stopWaypoints[i].pedestrianWaypoints.Count; j++)
                {
                    for (int k = 0; k < waypoint.prev.Count; k++)
                    {
                        if (waypoint.prev[k] == stopWaypoints[i].pedestrianWaypoints[j])
                        {
                            road = i;
                            break;
                        }

                    }
                    for (int k = 0; k < waypoint.neighbors.Count; k++)
                    {
                        if (waypoint.neighbors[k] == stopWaypoints[i].pedestrianWaypoints[j])
                        {
                            road = i;
                            break;
                        }
                    }
                }
            }
            return road;
        }
#endif

        private void Cancel()
        {
            selectedRoad = -1;
            currentAction = WindowActions.None;
            SceneView.RepaintAll();
        }


        public override void DestroyWindow()
        {
            EditorUtility.SetDirty(selectedIntersection);
            waypointDrawer.onWaypointClicked -= TrafficWaypointClicked;
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypointDrawer.onWaypointClicked -= PedestrianWaypointClicked;
#endif
            base.DestroyWindow();
        }
    }
}

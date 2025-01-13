#if GLEY_PEDESTRIAN_SYSTEM
using Gley.PedestrianSystem.Editor;
using Gley.PedestrianSystem.Internal;
#endif
using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficLightsCrossingWindow : IntersectionWindowBase
    {
        private enum WindowActions
        {
            None,
            AssignRoadWaypoints,
            AssignPedestrianWaypoints,
            AddDirectionWaypoints,
        }


        private TrafficLightsCrossingSettings selectedTrafficLightsIntersection;
        private float scrollAdjustment = 250;
        private WindowActions currentAction;

#if GLEY_PEDESTRIAN_SYSTEM
        private PedestrianWaypointDrawer pedestrianWaypointDrawer;
        private List<PedestrianWaypointSettings> pedestrianWaypoints = new List<PedestrianWaypointSettings>();
        private List<PedestrianWaypointSettings> directionWaypoints = new List<PedestrianWaypointSettings>();
#endif
        private List<GameObject> redLightObjects = new List<GameObject>();
        private List<GameObject> greenLightObjects = new List<GameObject>();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            selectedIntersection = SettingsWindow.GetSelectedIntersection();
            selectedTrafficLightsIntersection = selectedIntersection as TrafficLightsCrossingSettings;
            stopWaypoints = selectedTrafficLightsIntersection.stopWaypoints;
            exitWaypoints = selectedTrafficLightsIntersection.exitWaypoints;
            if (stopWaypoints.Count == 0)
            {
                stopWaypoints.Add(new IntersectionStopWaypointsSettings());
            }

#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypoints = selectedTrafficLightsIntersection.pedestrianWaypoints;
            directionWaypoints = selectedTrafficLightsIntersection.directionWaypoints;
            redLightObjects = selectedTrafficLightsIntersection.redLightObjects;
            greenLightObjects = selectedTrafficLightsIntersection.greenLightObjects;
            pedestrianWaypointDrawer = CreateInstance<PedestrianWaypointDrawer>();
            pedestrianWaypointDrawer.Initialize();
            pedestrianWaypointDrawer.onWaypointClicked += PedestrianWaypointClicked;
#endif

            currentAction = WindowActions.None;
            ISetupWindow baseInterface = base.Initialize(windowProperties, window);
            waypointDrawer.onWaypointClicked += WaypointClicked;
            return baseInterface;
        }


        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            switch (currentAction)
            {
                case WindowActions.AssignRoadWaypoints:
                    AddWaypointToList(clickedWaypoint, stopWaypoints[selectedRoad].roadWaypoints);
                    break;
            }
            SceneView.RepaintAll();
            SettingsWindowBase.TriggerRefreshWindowEvent();
        }
#if GLEY_PEDESTRIAN_SYSTEM
        private void PedestrianWaypointClicked(PedestrianWaypointSettings clickedWaypoint, bool leftClick)
        {
            switch (currentAction)
            {
                case WindowActions.AddDirectionWaypoints:
                    AddWaypointToList(clickedWaypoint, directionWaypoints);
                    break;
                case WindowActions.AssignPedestrianWaypoints:
                    AddWaypointToList(clickedWaypoint, pedestrianWaypoints);
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
                    IntersectionDrawer.DrawListWaypoints(exitWaypoints, intersectionSave.exitWaypointsColor, 0, save.textColor, trafficWaypointSize);
                    DrawStopWaypoints(false);
#if GLEY_PEDESTRIAN_SYSTEM
                    IntersectionDrawer.DrawListWaypoints(pedestrianWaypoints, intersectionSave.stopWaypointsColor, -1, Color.white, pedestrianWaypointSize);
                    IntersectionDrawer.DrawListWaypoints(directionWaypoints, intersectionSave.exitWaypointsColor, -1, Color.white, pedestrianWaypointSize);
#endif
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
                        IntersectionDrawer.DrawListWaypoints(pedestrianWaypoints, intersectionSave.stopWaypointsColor, -1, Color.white, pedestrianWaypointSize);
                    }
                    break;

                case WindowActions.AddDirectionWaypoints:
                    for (int i = 0; i < pedestrianWaypoints.Count; i++)
                    {
                        pedestrianWaypointDrawer.DrawPossibleDirectionWaypoints(pedestrianWaypoints[i], save.waypointColor);
                    }
                    IntersectionDrawer.DrawListWaypoints(pedestrianWaypoints, intersectionSave.stopWaypointsColor, -1, Color.white, pedestrianWaypointSize);
                    IntersectionDrawer.DrawListWaypoints(directionWaypoints, intersectionSave.exitWaypointsColor, -1, Color.white, pedestrianWaypointSize);
                    break;
#endif
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            selectedTrafficLightsIntersection.greenLightTime = EditorGUILayout.FloatField("Green Light Time", selectedTrafficLightsIntersection.greenLightTime);
            selectedTrafficLightsIntersection.yellowLightTime = EditorGUILayout.FloatField("Yellow Light Time", selectedTrafficLightsIntersection.yellowLightTime);
            selectedTrafficLightsIntersection.redLightTime = EditorGUILayout.FloatField("Red Light Time", selectedTrafficLightsIntersection.redLightTime);
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
            EditorGUILayout.Space();
#if GLEY_PEDESTRIAN_SYSTEM
            ViewPedestrianWaypoints();
            EditorGUILayout.Space();

            ViewDirectionWaypoints();
            EditorGUILayout.Space();
#endif
        }


        #region StopWaypoints
        private void ViewAssignedStopWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
               "\nEach road that enters in intersection should have its own set of stop waypoints"));
            Color oldColor;
            for (int i = 0; i < stopWaypoints.Count; i++)
            {
                EditorGUILayout.Space();
                DisplayList(stopWaypoints[i].roadWaypoints, ref stopWaypoints[i].draw);
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();


                if (GUILayout.Button("Assign Road"))
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
            }
            EditorGUILayout.Space();
        }


        private void AddTrafficWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Stop waypoints:", "The vehicle will stop at this point until the intersection allows it to continue. " +
               "\nEach road that enters in intersection should have its own set of stop waypoints"));
            Color oldColor;


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Road " + (selectedRoad + 1));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            DisplayList(stopWaypoints[selectedRoad].roadWaypoints, ref stopWaypoints[selectedRoad].draw);
            EditorGUILayout.Space();

            oldColor = GUI.backgroundColor;
            if (stopWaypoints[selectedRoad].draw == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Road Waypoints"))
            {
                ViewRoadWaypoints(selectedRoad);

            }
            GUI.backgroundColor = oldColor;

            EditorGUILayout.Space();
            AddLightObjects("Red Light", stopWaypoints[selectedRoad].redLightObjects);
            AddLightObjects("Yellow Light", stopWaypoints[selectedRoad].yellowLightObjects);
            AddLightObjects("Green Light", stopWaypoints[selectedRoad].greenLightObjects);
            EditorGUILayout.Space();
            if (GUILayout.Button("Done"))
            {
                Cancel();
            }

            EditorGUILayout.EndVertical();
        }


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
        #region PedestrianWaypoints
        private void ViewPedestrianWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Pedestrian waypoints:", "Pedestrian waypoints are used for waiting before crossing the road. " +
                "Pedestrians will stop on those waypoints and wait for green color."));


            EditorGUILayout.EndHorizontal(); EditorGUILayout.Space();
            DisplayList(pedestrianWaypoints, ref intersectionSave.showPedestrians);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Pedestrian Waypoints"))
            {
                selectedRoad = -1;
                currentAction = WindowActions.AssignPedestrianWaypoints;
            }
            Color oldColor = GUI.backgroundColor;
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
        }


        private void AddPedestrianWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Pedestrian stop waypoints:", "pedestrians will stop at this point until the intersection allows them to continue. " +
            "\nEach crossing in intersection should have its own set of stop waypoints corresponding to its road"));

            DisplayList(pedestrianWaypoints, ref intersectionSave.showPedestrians);

            EditorGUILayout.Space();
            Color oldColor = GUI.backgroundColor;
            if (intersectionSave.showPedestrians == true)
            {
                GUI.backgroundColor = Color.green;
            }
            if (GUILayout.Button("View Pedestrian Waypoints"))
            {
                ViewAllPedestrianWaypoints();
            }
            GUI.backgroundColor = oldColor;

            AddLightObjects("Red Light - Pedestrians", redLightObjects);
            AddLightObjects("Green Light - Pedestrians", greenLightObjects);

            if (GUILayout.Button("Done"))
            {
                Cancel();
            }
            EditorGUILayout.EndVertical();
        }


        private void ViewAllPedestrianWaypoints()
        {
            intersectionSave.showPedestrians = !intersectionSave.showPedestrians;
            for (int i = 0; i < pedestrianWaypoints.Count; i++)
            {
                pedestrianWaypoints[i].draw = intersectionSave.showPedestrians;
            }
        }
        #endregion


        #region DirectionWaypoints
        private void ViewDirectionWaypoints()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(new GUIContent("Crossing direction waypoints:", "For each stop waypoint a direction needs to be specified\n" +
                "Only if a pedestrian goes to that direction it will stop, otherwise it will pass through stop waypoint"));
            EditorGUILayout.Space();
            DisplayList(directionWaypoints, ref intersectionSave.showDirection);

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

            DisplayList(directionWaypoints, ref intersectionSave.showDirection);

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
            for (int i = 0; i < directionWaypoints.Count; i++)
            {
                directionWaypoints[i].draw = intersectionSave.showDirection;
            }
        }
        #endregion
#endif

        void Cancel()
        {
            selectedRoad = -1;
            currentAction = WindowActions.None;
            SceneView.RepaintAll();
        }


        public override void DestroyWindow()
        {
            EditorUtility.SetDirty(selectedIntersection);
            waypointDrawer.onWaypointClicked -= WaypointClicked;
#if GLEY_PEDESTRIAN_SYSTEM
            pedestrianWaypointDrawer.onWaypointClicked -= PedestrianWaypointClicked;
#endif
            base.DestroyWindow();
        }
    }
}

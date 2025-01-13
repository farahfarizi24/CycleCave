using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public abstract class ShowWaypointsTrafficBase : SetupWindowBase
    {
        protected ViewWaypointsSettings save;
        protected RoadColors roadColors;
        protected float scrollAdjustment = 220;
        protected WaypointDrawer waypointDrawer;
        protected TrafficSettingsLoader settingsLoader;
        protected List<WaypointSettings> waypointsOfInterest;
        private bool waypointsLoaded = false;

        protected abstract List<WaypointSettings> GetWaypointsOfInterest();

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            waypointDrawer = CreateInstance<WaypointDrawer>().Initialize();
            waypointDrawer.onWaypointClicked += WaipointClicked;
            settingsLoader = LoadSettingsLoader();
            roadColors = settingsLoader.LoadRoadColors();
            base.Initialize(windowProperties, window);
            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUI.BeginChangeCheck();
            roadColors.waypointColor = EditorGUILayout.ColorField("Waypoint Color ", roadColors.waypointColor);

            EditorGUILayout.BeginHorizontal();
            save.showConnections = EditorGUILayout.Toggle("Show Connections", save.showConnections);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showOtherLanes = EditorGUILayout.Toggle("Show Lane Change", save.showOtherLanes);
            roadColors.laneChangeColor = EditorGUILayout.ColorField(roadColors.laneChangeColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showSpeed = EditorGUILayout.Toggle("Show Speed", save.showSpeed);
            roadColors.speedColor = EditorGUILayout.ColorField(roadColors.speedColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showCars = EditorGUILayout.Toggle("Show Cars", save.showCars);
            roadColors.carsColor = EditorGUILayout.ColorField(roadColors.carsColor);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            save.showPriority = EditorGUILayout.Toggle("Show Waypoint Priority", save.showPriority);
            roadColors.priorityColor = EditorGUILayout.ColorField(roadColors.priorityColor);
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndChangeCheck();
            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }

        protected override void ScrollPart(float width, float height)
        {
            if (waypointsOfInterest != null)
            {
                if (waypointsOfInterest.Count == 0)
                {
                    EditorGUILayout.LabelField("No " + GetWindowTitle());
                }
                for (int i = 0; i < waypointsOfInterest.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(waypointsOfInterest[i].name);
                    if (GUILayout.Button("View", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        GleyUtilities.TeleportSceneCamera(waypointsOfInterest[i].transform.position);
                        SceneView.RepaintAll();
                    }
                    if (GUILayout.Button("Edit", GUILayout.Width(BUTTON_DIMENSION)))
                    {
                        OpenEditWindow(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            else
            {
                EditorGUILayout.LabelField("No " + GetWindowTitle());
            }
            base.ScrollPart(width, height);
        }


        public override void DrawInScene()
        {
            waypointsOfInterest = GetWaypointsOfInterest();

            if (waypointsLoaded == false)
            {
                SettingsWindowBase.TriggerRefreshWindowEvent();
                waypointsLoaded = true;
            }
            base.DrawInScene();
        }


        protected TrafficSettingsLoader LoadSettingsLoader()
        {
            return new TrafficSettingsLoader(Internal.Constants.windowSettingsPath);
        }


        protected void OpenEditWindow(int index)
        {
            SettingsWindow.SetSelectedWaypoint((WaypointSettings)waypointsOfInterest[index]);
            GleyUtilities.TeleportSceneCamera(waypointsOfInterest[index].transform.position);
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        protected virtual void WaipointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaipointClicked;
            base.DestroyWindow();
        }
    }
}
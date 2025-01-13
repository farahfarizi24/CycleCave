using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class SpeedRoutesSetupWindow : SetupWindowBase
    {
        private List<int> speeds;
        private SpeedRoutesSave save;
        private float scrollAdjustment = 112;
        private WaypointDrawer waypointDrawer;
        private TrafficSettingsLoader settingsLoader;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            waypointDrawer = CreateInstance<WaypointDrawer>().Initialize();
            speeds = GetDifferentSpeeds(waypointDrawer.GetAllWaypoints());
            settingsLoader = new TrafficSettingsLoader(Internal.Constants.windowSettingsPath);
            save = settingsLoader.LoadSpeedRoutes();
            if (save.routesColor.Count < speeds.Count)
            {
                int nrOfColors = speeds.Count - save.routesColor.Count;
                for (int i = 0; i < nrOfColors; i++)
                {
                    save.routesColor.Add(Color.white);
                    save.active.Add(true);
                }
            }

            waypointDrawer.onWaypointClicked += WaypointClicked;
            return this;
        }

        private List<int> GetDifferentSpeeds(WaypointSettings[] allWaypoints)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < allWaypoints.Length; i++)
            {
                if (!result.Contains(allWaypoints[i].maxSpeed))
                {
                    result.Add(allWaypoints[i].maxSpeed);
                }
            }
            return result;
        }


        private void WaypointClicked(WaypointSettings clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        public override void DrawInScene()
        {
            for (int i = 0; i < speeds.Count; i++)
            {
                if (save.active[i])
                {
                    waypointDrawer.ShowWaypointsWithSpeed(speeds[i], save.routesColor[i]);
                }
            }

            base.DrawInScene();
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("SpeedRoutes: ");
            for (int i = 0; i < speeds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(speeds[i].ToString(), GUILayout.MaxWidth(50));
                save.routesColor[i] = EditorGUILayout.ColorField(save.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (save.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View"))
                {
                    save.active[i] = !save.active[i];
                    SceneView.RepaintAll();
                }

                GUI.backgroundColor = oldColor;
                EditorGUILayout.EndHorizontal();
            }

            base.ScrollPart(width, height);
            EditorGUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            settingsLoader.SaveSpeedRoutes(save);
            base.DestroyWindow();
        }
    }
}

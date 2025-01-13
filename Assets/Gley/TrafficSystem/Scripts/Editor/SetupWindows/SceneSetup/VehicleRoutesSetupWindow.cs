using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class VehicleRoutesSetupWindow : AgentRoutesSetupWindowBase
    {
        private TrafficSettingsLoader settingsLoader;
        private WaypointDrawer waypointDrawer;
        private int nrOfCars;
        private float scrollAdjustment = 112;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            settingsLoader = LoadSettingsLoader();
            save = settingsLoader.LoadCarRoutes();
            waypointDrawer = CreateInstance<WaypointDrawer>().Initialize();
            waypointDrawer.onWaypointClicked += WaypointClicked;
            nrOfCars = GetNrOfDifferentAgents();
            if (save.routesColor.Count < nrOfCars)
            {
                for (int i = save.routesColor.Count; i < nrOfCars; i++)
                {
                    save.routesColor.Add(Color.white);
                    save.active.Add(true);
                }
            }
            return base.Initialize(windowProperties, window);
        }

        public override void DrawInScene()
        {
            for (int i = 0; i < nrOfCars; i++)
            {
                if (save.active[i])
                {
                    waypointDrawer.ShowWaypointsWithVehicle(i, save.routesColor[i]);
                }
            }

            base.DrawInScene();
        }

        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            EditorGUILayout.LabelField("Car Routes: ");
            for (int i = 0; i < nrOfCars; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ConvertIndexToEnumName(i), GUILayout.MaxWidth(150));
                save.routesColor[i] = EditorGUILayout.ColorField(save.routesColor[i]);
                Color oldColor = GUI.backgroundColor;
                if (save.active[i])
                {
                    GUI.backgroundColor = Color.green;
                }
                if (GUILayout.Button("View", GUILayout.MaxWidth(BUTTON_DIMENSION)))
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

        protected override int GetNrOfDifferentAgents()
        {
            return System.Enum.GetValues(typeof(VehicleTypes)).Length;
        }


        protected TrafficSettingsLoader LoadSettingsLoader()
        {
            return new TrafficSettingsLoader(Internal.Constants.windowSettingsPath);
        }


        protected override void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick)
        {
            window.SetActiveWindow(typeof(EditWaypointWindow), true);
        }


        protected override string ConvertIndexToEnumName(int i)
        {
            return ((VehicleTypes)i).ToString();
        }

        public override void DestroyWindow()
        {
            settingsLoader.SaveCarRoutes(save);
            waypointDrawer.onWaypointClicked -= WaypointClicked;
            base.DestroyWindow();
        }
    }
    
}
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class ConnectRoadsWindowBase : SetupWindowBase
    {
        protected ConnectRoadsSave save;
        protected int nrOfCars;
        protected bool[] allowedCarIndex;
        protected RoadDrawer roadDrawer;
        protected RoadColors roadColors;
        protected float scrollAdjustment;
        protected bool drawAllConnections;
        protected bool showCustomizations;

        protected List<RoadBase> roadsOfInterest;
        int nrOfRoads;

        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            roadDrawer = CreateInstance<RoadDrawer>().Initialize();

            return this;
        }


        protected virtual void DrawButton()
        {

        }


        protected override void TopPart()
        {
            base.TopPart();
            string drawButton = "Draw All Connections";
            if (drawAllConnections == true)
            {
                drawButton = "Clear All";
            }

            if (GUILayout.Button(drawButton))
            {
                DrawButton();

            }

            EditorGUI.BeginChangeCheck();
            ShowCustomizations();
            EditorGUI.EndChangeCheck();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }
        }


        public override void DrawInScene()
        {
            base.DrawInScene();
            if (roadsOfInterest != null)
            {
                if (roadsOfInterest.Count != nrOfRoads)
                {
                    nrOfRoads = roadsOfInterest.Count;
                    SettingsWindowBase.TriggerRefreshWindowEvent();
                }
            }
        }

        protected virtual void ShowCustomizations()
        {

        }


        protected override void ScrollPart(float width, float height)
        {
            base.ScrollPart(width, height);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            DrawConnections();
            EditorGUILayout.Space();
            GUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            EditorGUILayout.Space();
        }


        protected override void BottomPart()
        {
            save.waypointDistance = EditorGUILayout.FloatField("Waypoint distance ", save.waypointDistance);
            if (save.waypointDistance <= 0)
            {
                Debug.LogWarning("Waypoint distance needs to be >0. will be set to 1 by default");
                save.waypointDistance = 1;
            }

            if (GUILayout.Button("Generate Selected Connections"))
            {
                GenerateSelectedConnections();
            }
            base.BottomPart();
        }


        protected virtual void GenerateSelectedConnections()
        {
            SceneView.RepaintAll();
        }


        protected virtual void View(int i, int j)
        {

            SceneView.RepaintAll();
        }


        protected virtual void DrawConnections()
        {

        }


        public override void DestroyWindow()
        {

            base.DestroyWindow();
        }
    }
}

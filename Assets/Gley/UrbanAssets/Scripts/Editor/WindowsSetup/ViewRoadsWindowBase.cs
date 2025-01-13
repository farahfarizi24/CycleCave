using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class ViewRoadsWindowBase : SetupWindowBase
    {
        protected const float minValue = 184;

        protected float minValueColor = 220;
        protected string drawAllRoadsText;
        protected RoadDrawer roadDrawer;
        protected ViewRoadsSave save;
        protected RoadColors roadColors;
        protected float scrollAdjustment;
        protected bool drawAllRoads;
        protected bool showCustomizations;

        protected List<RoadBase> roadsOfInterest;
        int nrOfRoads;

        protected abstract void SetTexts();
        protected abstract void DeleteCurrentRoad(RoadBase road);
        protected abstract void SelectWaypoint(RoadBase road);


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);

            roadDrawer = CreateInstance<RoadDrawer>().Initialize();

            scrollAdjustment = minValue;
            SetTexts();

            return this;
        }


        protected virtual void ViewLaneChangesToggle()
        {
            save.viewRoadsSettings.viewLaneChanges = EditorGUILayout.Toggle("View Lane Changes", save.viewRoadsSettings.viewLaneChanges);
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


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            if (roadsOfInterest != null)
            {
                if (roadsOfInterest.Count == 0)
                {
                    EditorGUILayout.LabelField("Nothing in view");
                }
                for (int i = 0; i < roadsOfInterest.Count; i++)
                {
                    MakeSelectRoadRow(roadsOfInterest[i]);
                }
            }
            GUILayout.EndScrollView();
        }


        private void MakeSelectRoadRow(RoadBase road)
        {

            EditorGUILayout.BeginHorizontal();
            road.draw = EditorGUILayout.Toggle(road.draw, GUILayout.Width(TOGGLE_DIMENSION));
            GUILayout.Label(road.gameObject.name);



            if (GUILayout.Button("View"))
            {
                GleyUtilities.TeleportSceneCamera(road.transform.position);
                SceneView.RepaintAll();
            }
            if (GUILayout.Button("Select"))
            {
                SelectWaypoint(road);
            }
            if (GUILayout.Button("Delete"))
            {
                EditorGUI.BeginChangeCheck();
                if (EditorUtility.DisplayDialog("Delete " + road.name + "?", "Are you sure you want to delete " + road.name + "? \nYou cannot undo this operation.", "Delete", "Cancel"))
                {
                    DeleteCurrentRoad(road);
                }
                EditorGUI.EndChangeCheck();
            }

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            EditorGUILayout.EndHorizontal();

        }


        protected void UndoPerformed()
        {
            roadDrawer.Initialize();
        }


        public override void DestroyWindow()
        {
            Undo.undoRedoPerformed -= UndoPerformed;

            base.DestroyWindow();
        }
    }
}

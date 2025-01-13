using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class ShowEventWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            save = settingsLoader.LoadGiveWayWaypointsSave();
            return this;
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));
            base.ScrollPart(width, height);
            GUILayout.EndScrollView();
        }


        public override void DestroyWindow()
        {
            settingsLoader.SaveGiveWayWaypointsSettings(save, roadColors);
            base.DestroyWindow();
        }


        protected override List<WaypointSettings> GetWaypointsOfInterest()
        {
            return waypointDrawer.ShowEventWaypoints(roadColors.waypointColor);
        }
    }
}
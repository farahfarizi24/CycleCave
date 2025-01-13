using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;

namespace Gley.TrafficSystem.Editor
{
    public class ShowAllWaypoints : ShowWaypointsTrafficBase
    {
        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            save = settingsLoader.LoadAllWaypointsSave();
            return this;
        }


        public override void DrawInScene()
        {
            waypointDrawer.ShowAllWaypoints(roadColors.waypointColor, save.showConnections, save.showSpeed, roadColors.speedColor, save.showCars, roadColors.carsColor, save.showOtherLanes, roadColors.laneChangeColor, save.showPriority, roadColors.priorityColor);
            base.DrawInScene();
        }


        public override void DestroyWindow()
        {
            settingsLoader.SaveAllWaypointsSettings(save, roadColors);
            base.DestroyWindow();
        }


        protected override List<WaypointSettings> GetWaypointsOfInterest()
        {
            return null;
        }
    }
}

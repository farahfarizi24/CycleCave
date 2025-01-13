using Gley.UrbanAssets.Internal;

namespace Gley.UrbanAssets.Editor
{
    public abstract class AgentRoutesSetupWindowBase : SetupWindowBase
    {


        protected CarRoutesSave save;
       

        protected abstract int GetNrOfDifferentAgents();
        protected abstract string ConvertIndexToEnumName(int i);
        protected abstract void WaypointClicked(WaypointSettingsBase clickedWaypoint, bool leftClick);


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            return this;
        }


        public override void DestroyWindow()
        {
            base.DestroyWindow();
        }
    }
}
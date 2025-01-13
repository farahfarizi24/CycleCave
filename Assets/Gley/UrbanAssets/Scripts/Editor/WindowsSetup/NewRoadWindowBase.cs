using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class NewRoadWindowBase : SetupWindowBase
    {
        protected Vector3 firstClick;
        protected Vector3 secondClick;
        protected CreateRoadSave save;
        protected RoadColors roadColors;
        protected RoadDrawer roadDrawer;

        protected abstract void SetTopText();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            base.Initialize(windowProperties, window);
            firstClick = secondClick = Vector3.zero;

            roadDrawer = CreateInstance<RoadDrawer>().Initialize();

            return this;
        }


        protected override void TopPart()
        {
            base.TopPart();
            SetTopText();
        }


        public override void UndoAction()
        {
            base.UndoAction();
            if (secondClick == Vector3.zero)
            {
                if (firstClick != Vector3.zero)
                {
                    firstClick = Vector3.zero;
                }
            }
        }


        public override void LeftClick(Vector3 mousePosition, bool clicked)
        {
            if (firstClick == Vector3.zero)
            {
                firstClick = mousePosition;
            }
            else
            {
                secondClick = mousePosition;
                CreateRoad();
            }
            base.LeftClick(mousePosition, clicked);
        }


        protected virtual void CreateRoad()
        {          
            firstClick = Vector3.zero;
            secondClick = Vector3.zero;
        }
    }
}

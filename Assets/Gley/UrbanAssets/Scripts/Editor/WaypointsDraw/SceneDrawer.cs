using Gley.TrafficSystem.Editor;
using Gley.UrbanAssets.Internal;
using System;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public static class SceneDrawer
    {
        #region ViewGrid
        public static void DrawTrafficGrid(GridRow[] grid)
        {
            GleyUtilities.SetCamera();
            int columnLength = grid.Length;
            if (columnLength <= 0)
                return;
            int rowLength = grid[0].row.Length;
            if (GleyUtilities.SceneCameraMoved())
            {
                UpdateInViewPropertyForGrid(grid, columnLength, rowLength);
            }
            bool green = false;
            Handles.color = Color.white;
            for (int i = 0; i < columnLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (grid[i].row[j].hasTrafficWaypoints)
                    {
                        if (green == false)
                        {
                            green = true;
                            Handles.color = Color.green;
                        }
                    }
                    else
                    {
                        if (green == true)
                        {
                            green = false;
                            Handles.color = Color.white;
                        }
                    }

                    if (grid[i].row[j].inView)
                    {
                        Handles.DrawWireCube(grid[i].row[j].center, grid[i].row[j].size);
                    }
                }
            }
        }

        private static void UpdateInViewPropertyForGrid(GridRow[] grid, int columnLength, int rowLength)
        {
            for (int i = 0; i < columnLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (GleyUtilities.IsPointInViewNoValidation(grid[i].row[j].center))
                    {
                        grid[i].row[j].inView = true;
                    }
                    else
                    {
                        grid[i].row[j].inView = false;
                    }
                }
            }
        }

        public static void DrawPedestrianGrid(GridRow[] grid)
        {
            GleyUtilities.SetCamera();
            int columnLength = grid.Length;
            if (columnLength <= 0)
                return;
            int rowLength = grid[0].row.Length;
            if (GleyUtilities.SceneCameraMoved())
            {
                UpdateInViewPropertyForGrid(grid, columnLength, rowLength);
            }

            bool green = false;
            Handles.color = Color.white;
            for (int i = 0; i < columnLength; i++)
            {
                for (int j = 0; j < rowLength; j++)
                {
                    if (grid[i].row[j].hasPedestrianWaypoints)
                    {
                        if (green == false)
                        {
                            green = true;
                            Handles.color = Color.green;
                        }
                    }
                    else
                    {
                        if (green == true)
                        {
                            green = false;
                            Handles.color = Color.white;
                        }
                    }

                    if (grid[i].row[j].inView)
                    {
                        Handles.DrawWireCube(grid[i].row[j].center, grid[i].row[j].size);
                    }
                }
            }
        }
        #endregion
    }
}
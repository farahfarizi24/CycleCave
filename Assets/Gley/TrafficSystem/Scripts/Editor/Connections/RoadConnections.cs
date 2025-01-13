using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using Gley.UrbanAssets.Internal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class RoadConnections : UnityEditor.Editor
    {

        private List<ConnectionPool> connectionPools;


        internal RoadConnections Initialize()
        {
            LoadAllConnections();
            return this;
        }


        internal List<ConnectionPool> ConnectionPools
        {
            get
            {
                return connectionPools;
            }
        }


        protected void LoadAllConnections()
        {
            connectionPools = new List<ConnectionPool>();
            List<RoadBase> allRoads = CreateInstance<RoadDrawer>().Initialize().GetAllRoads();
            for (int i = 0; i < allRoads.Count; i++)
            {
                if (allRoads[i].isInsidePrefab && !GleyPrefabUtilities.EditingInsidePrefab())
                {
                    continue;
                }
                ConnectionPool connectionsScript = allRoads[i].transform.parent.GetComponent<ConnectionPool>();
                if (!connectionPools.Contains(connectionsScript))
                {
                    connectionPools.Add(connectionsScript);
                }
            }
        }


        protected ConnectionPool GetConnectionPool()
        {
            return RoadCreator.GetRoadWaypointsHolder<ConnectionPool>(Internal.Constants.trafficWaypointsHolderName).GetComponent<ConnectionPool>();
        }


        protected void GenerateConnectorWaypoints(ConnectionPool connections, int index, float waypointDistance)
        {
            CreateInstance<TrafficConnectionWaypoints>().GenerateConnectorWaypoints(connections, index, waypointDistance);
        }


        internal void DeleteConnection(ConnectionCurve connectingCurve)
        {
            CreateInstance<TrafficConnectionWaypoints>().RemoveConnectionHolder(connectingCurve.holder);
            for (int i = 0; i < ConnectionPools.Count; i++)
            {
                if (ConnectionPools[i].connectionCurves != null)
                {
                    if (ConnectionPools[i].connectionCurves.Contains(connectingCurve))
                    {
                        ConnectionPools[i].RemoveConnection(connectingCurve);
                        EditorUtility.SetDirty(ConnectionPools[i]);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }


        internal void MakeConnection(ConnectionPool connectionPool, RoadBase fromRoad, int fromIndex, RoadBase toRoad, int toIndex, float waypointDistance)
        {
            Vector3 offset = Vector3.zero;
            if (!GleyPrefabUtilities.EditingInsidePrefab())
            {
                if (GleyPrefabUtilities.IsInsidePrefab(fromRoad.gameObject) && Gley.UrbanAssets.Editor.GleyPrefabUtilities.GetInstancePrefabRoot(fromRoad.gameObject) == Gley.UrbanAssets.Editor.GleyPrefabUtilities.GetInstancePrefabRoot(toRoad.gameObject))
                {
                    connectionPool = GetConnectionPool();
                    offset = fromRoad.positionOffset;
                }
                else
                {
                    connectionPool = GetConnectionPool();
                    offset = fromRoad.positionOffset;
                }
            }
            ((ConnectionPool)connectionPool).AddConnection(fromRoad.lanes[fromIndex].laneEdges.outConnector, toRoad.lanes[toIndex].laneEdges.inConnector, fromRoad, fromIndex, toRoad, toIndex, offset);
            GenerateConnectorWaypoints(connectionPool, connectionPool.connectionCurves.Count - 1, waypointDistance);

            EditorUtility.SetDirty(connectionPool);
            AssetDatabase.SaveAssets();
            LoadAllConnections();
        }


        internal void GenerateSelectedConnections(float waypointDistance)
        {
            for (int i = 0; i < ConnectionPools.Count; i++)
            {
                int nrOfConnections = ConnectionPools[i].GetNrOfConnections();
                for (int j = 0; j < nrOfConnections; j++)
                {
                    if (ConnectionPools[i].connectionCurves[j].draw)
                    {
                        if (GleyUtilities.IsPointInViewWithValidation(((ConnectionPool)ConnectionPools[i]).GetInConnector(j).transform.position) ||
                            GleyUtilities.IsPointInViewNoValidation(((ConnectionPool)ConnectionPools[i]).GetOutConnector<WaypointSettingsBase>(j).transform.position))
                        {

                            GenerateConnectorWaypoints(ConnectionPools[i], j, waypointDistance);
                        }
                    }
                }
            }
        }
    }
}
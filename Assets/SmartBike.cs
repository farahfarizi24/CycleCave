using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SmartBike : MonoBehaviour
{
private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;

    private Thread receiveThread;
    private string receivedData;

    private string serverIP = "127.0.0.1"; //Match with main computer IP, similar at the Python file used for bike   
    private int serverPort = 5005; //Match Python's Server port
    private bool isReceiving = true;

    //Data variables
    public float speed;
    public float cadence;
    public int power;

    //Data collection with unique ID 
    private List<LogEntry> dataLog = new List<LogEntry>();
    private int logCounter = 0;

    void Start()
    {
        //Set up UDP client
        udpClient = new UdpClient(serverPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        Debug.Log("UDP Client started on" + serverIP + ":" + serverPort);

        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();

    }
    void ReceiveData(){
        try{
            while (isReceiving){
                //Receive data from server
                byte[] data = udpClient.Receive(ref remoteEndPoint); //Blocks until data is received
                string message = Encoding.UTF8.GetString(data);

                //Log received data
                Debug.Log("Received: " + message);

                // Deserialize JSON data
                DataPayload payload = JsonUtility.FromJson<DataPayload>(message);

                if (payload != null)
                {
                    //Updating data
                    speed = payload.speed;
                    cadence = payload.cadence;
                    power = payload.power;
                    Debug.Log($"Updated speed: {speed}, Cadence: {cadence}, Power: {power}");

                    //Creating new log entries
                    // logCounter++;
                    // LogEntry newLog = new LogEntry
                    // {
                    //     logID = logCounter,
                    //     speed = payload.speed,
                    //     cadence = payload.cadence,
                    //     power = payload.power,
                    //     timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    // };

                    // //Add LogEntry to data collection
                    // dataLog.Add(newLog);
                }
                else
                {
                    Debug.LogError("Failed to deserialize payload.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    //Moving the cameras along
    // void Update(){
    //     //Using speed to use transform.position to move along with the bike
    //     var step = Time.deltaTime * speed;

    //     //moving forward for now, according to value
    //     transform.position += transform.forward * step;
        
    // }

    // void SaveDataToFile(LogEntry log){
    //             string filePath = "bikeDataLog.txt";
    //     string logText = $"{log.logID}, {log.speed}, {log.cadence}, {log.power}, {log.timestamp}\n";
    //     System.IO.File.AppendAllText(filePath, logText);
    // }

    void OnApplicationQuit()
    {
        isReceiving = false;
        udpClient.Close();
        receiveThread.Join(); //Wait for receiving thread to finish before quitting
    }


    // Used for JsonUtility
    [Serializable]
    public class DataPayload
    {
        public float speed;
        public float cadence;
        public int power;
    }

    //Used for data collection log entry
    [Serializable]
    public class LogEntry{
        public int logID;
        public float speed;
        public float cadence;
        public int power;
        public string timestamp;
    }
}


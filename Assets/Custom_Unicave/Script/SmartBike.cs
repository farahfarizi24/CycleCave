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
    //For UDP connection - subject to change with direct connection using Kickr cable.
    private UdpClient udpClient;
    private IPEndPoint remoteEndPoint;
    public Cameras CamScript;
    private Thread receiveThread;

    private string serverIP = "10.148.112.66"; //Match with main computer IP, similar at the Python file used for bike   
    //private string serverIP = "127.0.0.1"; //For testing purposes
    private int serverPort = 5005; //Match Python's Server port
    private bool isReceiving = true;

    //Data variables
    public float speed;
    public float cadence;

    //Data collection with unique ID 
    private List<LogEntry> dataLog = new List<LogEntry>();
    private int logCounter = 0;
    private string sessionFilePath; //Stores unique file path per session

    //Notifying listeners when logging starts
    public event Action OnLoggingStarted;
    //Flag to control logging
    private bool isLogging = false;
    //Flag to check sessions
    public bool isSessionActive = false;

    //Referencing to Manager to get the sessionid out of it.
    public Manager manager;

    void Start()
    { 
        //Set up UDP client
        udpClient = new UdpClient(serverPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        Debug.Log("UDP Client started on" + serverIP + ":" + serverPort);

        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }

    public void setSessionActive(bool active){
        isSessionActive = active;
        Debug.Log($"Session active: {isSessionActive}");
    }

    public void StartLogging(){
        if (!isLogging){
            isLogging = true;
            Debug.Log("SmartBike: Data logging started.");
            //Generate file for this session
            string sessionTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            sessionFilePath = $"bikeDataLog_{manager.sessionid}_{sessionTimestamp}.csv";
            Debug.Log("New session file: " + sessionFilePath);

            //Initialize headers for file
            string headerText = "LogID, UserID, Speed, Cadence, Timestamp\n";
            System.IO.File.WriteAllText(sessionFilePath, headerText);
            OnLoggingStarted?.Invoke(); //Notify listeners
        }
    }

    void ReceiveData(){
        try{
            while (isReceiving){
                // Receive data from server
                byte[] data = udpClient.Receive(ref remoteEndPoint); // Blocks until data is received
                string message = Encoding.UTF8.GetString(data);

                if (isSessionActive){
                    // Log received data
                    Debug.Log("Received: " + message);

                    // Deserialize JSON data
                    DataPayload payload = JsonUtility.FromJson<DataPayload>(message);
                    if (payload != null)
                    {
                        // Updating data
                        speed = payload.speed;
                        cadence = payload.cadence;
                        Debug.Log($"Updated speed: {speed}, updated cadence: {cadence}");

                        logCounter++;
                        //Figure out how to connect the sessionID from lobby into here. 
                        LogEntry newLog = new LogEntry
                        {
                            logID = logCounter,
                            sessionID = manager.sessionid,
                            speed = payload.speed,
                            cadence = payload.cadence,
                            timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        };

                        // Add LogEntry to data collection
                        //Figure out how to make a separate file involving sessionIDs, for now.
                        dataLog.Add(newLog);

                        // Save the log data to file
                        SaveDataToFile(newLog);
                    }
                    else
                    {
                        Debug.LogError("Failed to deserialize payload.");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }


    //Moving the cameras along
    void Update(){
        if(isSessionActive == true){
            
            var step = Time.deltaTime * speed * 6.0f;
            CamScript.cameraMoveSpeed = speed;
            CamScript.MoveCode();
            //moving forward for now, according to value
            //transform.position += transform.forward * step;
        }
        
    }

void SaveDataToFile(LogEntry log){
    // Append the log entry data
    string logText = $"{log.logID}, {log.sessionID}, {log.speed}, {log.cadence}, {log.timestamp}\n";
    System.IO.File.AppendAllText(sessionFilePath, logText);

    Debug.Log("Log saved to: " + sessionFilePath);
}

    void OnApplicationQuit()
    {
        isReceiving = false;
        udpClient.Close();
        receiveThread.Join(); //Wait for receiving thread to finish before quitting
    }


    // Used for receiving data from bike
    [System.Serializable]
    public class DataPayload
    {
        public float speed;
        public float cadence;
    }

    //Used for data collection log entry
    [System.Serializable]
    public class LogEntry{
        public int logID;
        public string sessionID;
        public float speed;
        public float cadence;
        public string timestamp;
    }
}


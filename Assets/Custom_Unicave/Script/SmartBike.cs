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
    Cameras CamScript;
    private Thread receiveThread;

    private string serverIP = "10.148.112.66"; //Match with main computer IP, similar at the Python file used for bike   
    //private string serverIP = "127.0.0.1"; //For testing purposes
    private int serverPort = 5005; //Match Python's Server port
    //private int serverPort = 1567;
    private bool isReceiving = true;
    public Manager SceneManager;
    //Data variables
    public float speed;
    public float cadence;
    public bool brake;

    // brake control vairables
    public float newSpeed;
    public float preSpeed;
    public float preMeasuredSpeed;
    public bool brakeOverride;
    public const float ZeroSpeedThreshold = 1.5f;
    public const float BrakingRate = 0.7f;
    public const float CoastingRate = 0.6f;
    public const float ReleaseGainRate = 0.7f; // lower values will make it less obvious though also less responsive for longer

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

    private void Awake()
    {
        CamScript = GetComponent<Cameras>();
        StartLogging();
        setSessionActive(true);
        SceneManager = GameObject.FindGameObjectWithTag("ManagerTag").GetComponent<Manager>();
    }

    void Start()
    {
        //Set up UDP client
        udpClient = new UdpClient(serverPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        Debug.Log("UDP Client started on" + serverIP + ":" + serverPort);

        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }

    public void setSessionActive(bool active)
    {
        isSessionActive = active;
        Debug.Log($"Session active: {isSessionActive}");
    }

    public void StartLogging()
    {
        if (!isLogging)
        {
            isLogging = true;
            Debug.Log("SmartBike: Data logging started.");
            //Generate file for this session
            string sessionTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            sessionFilePath = $"bikeDataLog_{Manager.instance.sessionid}_{sessionTimestamp}.csv";
            Debug.Log("New session file: " + sessionFilePath);

            //Initialize headers for file
            string headerText = "LogID, SessionID, Speed, Cadence, Timestamp\n";
            System.IO.File.WriteAllText(sessionFilePath, headerText);
            OnLoggingStarted?.Invoke(); //Notify listeners
        }
    }

    void ReceiveData()
    {
        try
        {
            while (isReceiving)
            {
                // Receive data from server
                byte[] data = udpClient.Receive(ref remoteEndPoint); // Blocks until data is received
                string message = Encoding.UTF8.GetString(data);

                if (isSessionActive)
                {
                    // Log received data
                    Debug.Log("Received: " + message);

                    // Deserialize JSON data
                    DataPayload payload = JsonUtility.FromJson<DataPayload>(message);
                    if (payload != null)
                    {

                        // Updating data
                        newSpeed = payload.speed;
                        cadence = payload.cadence;
                        brake = payload.brake;
                        Debug.Log($"Updated speed: {newSpeed}, updated cadence: {cadence}, updated brake: {brake}");

                        logCounter++;
                        //Figure out how to connect the sessionID from lobby into here. 
                        //Decide on if to log brake data - assuming this goes into the CSV
                        LogEntry newLog = new LogEntry
                        {
                            logID = logCounter,
                            sessionID = Manager.instance.sessionid,
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

    // block the read speed value and simulate braking if the brake was enabled and padelling has stopped
    void ApplyBrake() {
        // start/end brake
        if (brake == true) {
            brakeOverride = true;
            //Debug.Log("Brake Enabled");
        }
        // turn off brake if pedalling or at 0 speed
        else if (brakeOverride == true && (cadence > 0 || newSpeed == 0 || newSpeed - preMeasuredSpeed > 1 )) {
            brakeOverride = false;
            //Debug.Log("Brake Disabled");
        }

        // brake/catch up to measured speed
        if (brakeOverride == true) {
            if (brake == true) {
                speed = preSpeed - (preSpeed * BrakingRate * Time.fixedDeltaTime);
                Debug.Log("Hard braking");
            }
            else {
                speed = preSpeed - (preSpeed * CoastingRate * Time.fixedDeltaTime);
                Debug.Log("Coast braking");
            }
        }
        else {
            if (preSpeed < newSpeed) {
                speed = preSpeed + ((newSpeed - preSpeed) * ReleaseGainRate * Time.fixedDeltaTime);
                Debug.Log("Speed catching up");
            }
            else {
                speed = newSpeed;
                Debug.Log("Speed caught up");
            }
        }

        // zero speed if below threshold
        if ((speed < ZeroSpeedThreshold && newSpeed < ZeroSpeedThreshold) || (speed < ZeroSpeedThreshold && brakeOverride)) {
            speed = 0;
        }

        preSpeed = speed;
        preMeasuredSpeed = newSpeed;
    }

    void FixedUpdate() 
    {
        if (isSessionActive == true)
        {

            // FIXME: need to scale rates based on Time.deltaTime (I think?)
            ApplyBrake();

            var step = Time.deltaTime * speed * 3.5f;
            CamScript.cameraMoveSpeed = speed;
            if (!SceneManager.movementDisabled)
            {
                CamScript.Move();

            }
            //moving forward for now, according to value
            //transform.position += transform.forward * step;
        }
    }

    void SaveDataToFile(LogEntry log)
    {
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
        public bool brake;
    }

    //Used for data collection log entry
    [System.Serializable]
    public class LogEntry
    {
        public int logID;
        public string sessionID;
        public float speed;
        public float cadence;
        public string timestamp;
    }
}


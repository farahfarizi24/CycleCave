using Michsky.MUIP;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Device;
using Application = UnityEngine.Application;


public class Manager : MonoBehaviourPunCallbacks
{
    // References
    public GameObject loadingScreen;
    public GameObject lobbyScreen;
    public GameObject sessionCreationScreen;
    public GameObject UiParent;
    public CustomInputField inputField;

    public ButtonManager front_button;
    public ButtonManager bottom_button;
    public ButtonManager right_button;
    public ButtonManager left_button;
    public ButtonManager top_button;

    public ButtonManager create_session;
    public ButtonManager join_session;

    public GameObject completePanel;

    public Animator animator_fading;

    // Variables
    public Screen currentScreen;
    public string sessionid;
    private Cameras cameras;
    private List<string> currentCameras = new List<string>();

    public Action onCameraListUpdated_action;

    public int cameraCount;

    // Scene management variables
    private List<int> sceneOrder = new List<int>();
    private int currentSceneIndex = -1;

    public int[] availableScenes = { 1, 2, 3, 4 }; // Scene indices from Build Settings

    public static Manager instance;

    public bool movementDisabled = false;

    #region Network

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
        {
            instance = this;
        }



        front_button.onClick.AddListener(() => StartCoroutine(SelectScreen(Screen.front)));
        bottom_button.onClick.AddListener(() => StartCoroutine(SelectScreen(Screen.bottom)));
        right_button.onClick.AddListener(() => StartCoroutine(SelectScreen(Screen.right)));
        left_button.onClick.AddListener(() => StartCoroutine(SelectScreen(Screen.left)));
        top_button.onClick.AddListener(() => StartCoroutine(SelectScreen(Screen.Top)));

        inputField.inputText.onEndEdit.AddListener((value) => sessionid = value);

        create_session.onClick.AddListener(() => PhotonNetwork.CreateRoom(sessionid));
        join_session.onClick.AddListener(() => PhotonNetwork.JoinRoom(sessionid));

       
    }

    

    public void Start()
    {
        setState(State.loading);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        setState(State.session);
        Debug.Log("Joined Lobby");
    }

    public override void OnJoinedRoom()
    {
        setState(State.lobby);
        Debug.Log("Joined Room");
    }

    private async void onCameraListUpdated()
    {
        bool allCamerasSet = areAllCameraSet();

        if(allCamerasSet)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                
                GenerateSceneOrder();

                LoadNextScene(true);

                // Wait for 1 second
                //await Task.Delay(1000); // Use Task.Delay for async waiting

                var camera = PhotonNetwork.Instantiate(
                    "UnicaveSetup_OnSceneLoad",
                    new Vector3(252.699936f, 257.678436f, 270.29306f),
                    Quaternion.identity
                );
            }
            await Task.Delay(1000); // Use Task.Delay for async waiting
             
            this.cameras = GameObject.FindAnyObjectByType<Cameras>();
            SetScreen(currentScreen);
        }
    }


    [PunRPC]
    private void onCameraListUpdated(string camera)
    {
        currentCameras.Add(camera);
        onCameraListUpdated();
    }

    public bool areAllCameraSet()
    {
        return currentCameras.Count == cameraCount;
    }

    // Generates a random order for the scenes
    private void GenerateSceneOrder()
    {
        completePanel.SetActive(false);
        sceneOrder.Clear();
        List<int> tempScenes = new List<int>(availableScenes);

        sceneOrder.Add(1);

        while (tempScenes.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempScenes.Count);
            sceneOrder.Add(tempScenes[randomIndex]);
            tempScenes.RemoveAt(randomIndex);
        }

        Debug.Log("Scene Order: " + string.Join(", ", sceneOrder));
    }

    // Loads the next scene in the sequence
    public void LoadNextScene(bool firstTIme)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentSceneIndex++;

            if (currentSceneIndex < sceneOrder.Count)
            {
                int nextScene = sceneOrder[currentSceneIndex];
                photonView.RPC("LoadSceneRpc", RpcTarget.All, nextScene , firstTIme);
            }
            else
            {
                // All scenes played, show black screen
                photonView.RPC("ShowBlackScreenRpc", RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void LoadSceneRpc(int sceneNo , bool firstTIme)
    {
        animator_fading.Play("Fade_out");
        StartCoroutine(LoadScene(sceneNo, firstTIme));
        
        
    }

    private IEnumerator LoadScene(int scene , bool firstTime)
    {
        movementDisabled = true;
        if(!firstTime)
        yield return new WaitForSeconds(30f); // Adjust this to match the fade duration
        else
            yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel(scene);
        yield return new WaitForSeconds(5f);
        animator_fading.Play("Fade_in");

        FindAnyObjectByType<Cameras>()?.ResetCamera();
        movementDisabled = false;
    }
    [PunRPC]
    private void ShowBlackScreenRpc()
    {
        Debug.Log("All scenes played. Showing black screen.");
        animator_fading.Play("Fade_out");
        completePanel.SetActive(true);
        StartCoroutine(ResetGame());
    }

    private IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(5f);

        Application.Quit();
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateSceneOrder();
            currentSceneIndex = -1;
            LoadNextScene(false);
        }
    }

    public void FinishScene()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadNextScene(false);
        }
    }

    #endregion

    #region UI

    public IEnumerator SelectScreen(Screen screen)
    {
        animator_fading.Play("Fade_out");
        setState(State.game);
        currentScreen = screen;

        yield return new WaitForSeconds(0.5f);
        
        photonView.RPC("onCameraListUpdated", RpcTarget.AllBuffered, screen.ToString());
    }

    
    private void SetScreen(Screen screen)
    {
        Cameras cameras = FindObjectOfType<Cameras>();
        SetAllButonsOff();
        switch (screen)
        {
            case Screen.front:
                cameras.setCamera(Screen.front);
                setState(State.game);
                break;
            case Screen.bottom:
                cameras.setCamera(Screen.bottom);
                setState(State.game);
                break;
            case Screen.right:
                cameras.setCamera(Screen.right);
                setState(State.game);
                break;
            case Screen.left:
                cameras.setCamera(Screen.left);
                setState(State.game);
                break;
            case Screen.Top:
                cameras.setCamera(Screen.Top);
                setState(State.game);
                break;
            default:
                break;
        }
    }

    private void SetAllButonsOff()
    {
        front_button.gameObject.SetActive(false);
        bottom_button.gameObject.SetActive(false);
        right_button.gameObject.SetActive(false);
        left_button.gameObject.SetActive(false);
        top_button.gameObject.SetActive(false);
    }

    public void setState(State state)
    {
        switch (state)
        {
            case State.lobby:
                loadingScreen.SetActive(false);
                sessionCreationScreen.SetActive(false);
                lobbyScreen.SetActive(true);
                break;
            case State.session:
                loadingScreen.SetActive(false);
                lobbyScreen.SetActive(false);
                sessionCreationScreen.SetActive(true);
                break;
            case State.room:
                loadingScreen.SetActive(true);
                lobbyScreen.SetActive(false);
                break;
            case State.game:
                loadingScreen.SetActive(false);
                lobbyScreen.SetActive(false);
                sessionCreationScreen.SetActive(false);
                UiParent.SetActive(false);
                break;
            case State.loading:
                loadingScreen.SetActive(true);
                lobbyScreen.SetActive(false);
                sessionCreationScreen.SetActive(false);
                break;
            default:
                break;
        }
    }

    #endregion

    #region Animation

    public void Fade(bool fade)
    {
        if (fade)
        {
            animator_fading.Play("Fade_in");
        }
        else
        {
            animator_fading.Play("Fade_out");
        }
    }

    #endregion
}

public enum Screen
{
    front, bottom, right, left, Top
}

public enum State
{
    loading, lobby, room, game, session
}

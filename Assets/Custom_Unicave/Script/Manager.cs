using Michsky.MUIP;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviourPunCallbacks
{
    //References
    //-- UI
    public GameObject loadingScreen;
    public GameObject lobbyScreen;
    public GameObject sessionCreationScreen;
    public GameObject UiParent;
    public CustomInputField inputField;

    public ButtonManager front_button;
    public ButtonManager bottom_button;
    public ButtonManager right_button;
    public ButtonManager left_button;

    //sessionButtons
    public ButtonManager create_session;
    public ButtonManager join_session;

    //Camera viewport
    public SliderManager camera_x;
    public SliderManager camera_y;
    public SliderManager camera_W;
    public SliderManager camera_H;
    public ButtonManager camera_apply;
    public GameObject viewportPanel;

    //Variables
    public Screen currentScreen;

    public string sessionid;

    #region Network

    private void Awake()
    {
        front_button.onClick.AddListener(() => SelectScreen(Screen.front));
        bottom_button.onClick.AddListener(() => SelectScreen(Screen.bottom));
        right_button.onClick.AddListener(() => SelectScreen(Screen.right));
        left_button.onClick.AddListener(() => SelectScreen(Screen.left));

        inputField.inputText.onEndEdit.AddListener((value) => sessionid = value);

        create_session.onClick.AddListener(() => PhotonNetwork.CreateRoom(sessionid));
        join_session.onClick.AddListener(() => PhotonNetwork.JoinRoom(sessionid));

        camera_apply.onClick.AddListener(() =>
        {
            // Find the camera and set its Viewport Rect
            Cameras cameras = FindObjectOfType<Cameras>();
            Camera targetCamera = cameras.GetCamera(currentScreen).Camera.GetComponent<Camera>();

            if (targetCamera != null)
            {
                float x = camera_x.mainSlider.value; // Assuming SliderManager has a GetValue() method
                float y = camera_y.mainSlider.value;
                float w = camera_W.mainSlider.value;
                float h = camera_H.mainSlider.value;

                // Update the camera's viewport rect
                targetCamera.rect = new Rect(x, y, w, h);
                Debug.Log($"Viewport updated: X={x}, Y={y}, W={w}, H={h}");
            }
            else
            {
                Debug.LogError("Target camera not found!");
            }
        });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (viewportPanel != null)
            {
                // Toggle the active state of the GameObject
                viewportPanel.SetActive(!viewportPanel.activeSelf);
                Debug.Log($"Target object is now {(viewportPanel.activeSelf ? "ON" : "OFF")}");
            }
            else
            {
                Debug.LogError("Target object is not assigned!");
            }
        }
    }


    public void Start()
    {
        setState(State.loading);
        //Connect to the network
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

    #endregion

    #region Ui

    public void SelectScreen(Screen screen)
    {
        currentScreen = screen;
        Cameras cameras = FindObjectOfType<Cameras>();
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
            default:
                break;
        }
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
}
public enum Screen
{
    front,bottom,right,left
}
public enum State
{
    loading,lobby,room,game,session
}
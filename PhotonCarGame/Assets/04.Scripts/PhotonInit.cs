using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PhotonInit : MonoBehaviour
{
    [SerializeField] string Version = "V_car.1.0";
    [SerializeField] InputField userID; 
    [SerializeField] InputField RoomName;
    [SerializeField] GameObject scrollContents;
    [SerializeField] GameObject roomItem;
    //[SerializeField]
    
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings(Version);
        RoomName.text = "Room" + Random.Range(0, 999).ToString("000");
    }

    void OnJoinedLobby()    // 로비에 입장하면 자동으로 호출
    {
        print("로비 접속!");
        //PhotonNetwork.JoinRandomRoom();
        userID.text = GetUserID();
    }

    string GetUserID()
    {
        string userID = PlayerPrefs.GetString("USER_");
        if(string.IsNullOrEmpty(userID))
        { 
            userID = "USER" + Random.Range(0, 999).ToString("000");
        }
        return userID;
    }

    void OnReceivedRoomListUpdate()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomItem"))
        {
            Destroy(obj);   // 방 목록 최신화
        }

        foreach (RoomInfo _room in PhotonNetwork.GetRoomList())
        {
            GameObject room = (GameObject)Instantiate(roomItem);
            room.transform.SetParent(scrollContents.transform, false);

            RoomData roomData = room.GetComponent<RoomData>();
            roomData.roomName = _room.Name;
            roomData.connectPlayer = _room.PlayerCount;
            roomData.maxPlayers = _room.MaxPlayers;
            roomData.DisplayRoomData();

            roomData.GetComponent<UnityEngine.UI.Button>().
                onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });
        }
    }
    public void OnClickCreateRoom()
    {
        // 1. 방 만들기 버튼 입력시 입력란에 입력했을 경우와 하지 않았을 경우 구분
        string _roomName = RoomName.text;
        if(string.IsNullOrEmpty(RoomName.text))
        {
            _roomName = "Room" + Random.Range(0, 999).ToString("000");
        }
        PhotonNetwork.player.NickName = userID.text;
        PlayerPrefs.SetString("USER_", userID.text);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    void OnPhotonRandomJoinFailed()
    {
        print("No Room!");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 5;
        PhotonNetwork.CreateRoom("RacingCarGame", roomOptions, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        Debug.Log("룸 접속");
        StartCoroutine(LoadPlayScene());

        //CreateCar();
    }

    void OnPhotonCreateRoom(object[] codeAndMsg)
    {
        Debug.Log("Create Room Failed" + codeAndMsg[1]);
    }

    IEnumerator LoadPlayScene()
    {
        PhotonNetwork.isMessageQueueRunning = false;
        AsyncOperation ao = SceneManager.LoadSceneAsync("RacePlay");
        yield return ao;
    }

    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.player.NickName = userID.text;
        PlayerPrefs.SetString("USER_", userID.text);
        PhotonNetwork.JoinRandomRoom();
    }

    void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.player.NickName = userID.text;
        PlayerPrefs.SetString("USER_", userID.text);
        PhotonNetwork.JoinRoom(roomName);   // 해당 방에 입장
    }

    private void OnGUI()
    {
        // 좌측 상단에 접속상태 표시
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }
}

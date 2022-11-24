using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomData : MonoBehaviour
{
    public string roomName = "";
    public int maxPlayers = 0;
    public int connectPlayer = 0;
    public Text RoomNameText;
    public Text ConnectInfoText;

    public void DisplayRoomData()
    {
        RoomNameText.text = roomName;
        ConnectInfoText.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}

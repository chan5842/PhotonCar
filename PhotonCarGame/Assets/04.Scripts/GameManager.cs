using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    PhotonView pv = null;
    int playerClamp;
    int playerCount;
    Vector3[] SpawnCarPos = new Vector3[5];


    void Awake()
    {
        pv = GetComponent<PhotonView>();

        PhotonNetwork.isMessageQueueRunning = true;
        CreateCar();
    }
    private void Start()
    {
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.player.NickName + "]Connected</color>";
        pv.RPC("LogMsg", PhotonTargets.AllBuffered, msg);    // 자기 자신을 포함한 모두 메시지로
    }

    void CreateCar()
    {
        Room room = PhotonNetwork.room;
        playerClamp = room.MaxPlayers;  // 최대 수용인원 전달
        playerCount = room.PlayerCount; // 현재 방 플레이어 수

        if (playerCount > 5) return;

        Quaternion SpawnRot = Quaternion.Euler(-4f, -2f, -1.3f);

        Vector3 SpawnCarPos1 = new Vector3(433f, 0.05f, 230f);
        Vector3 SpawnCarPos2 = new Vector3(438f, 0.05f, 230f);
        Vector3 SpawnCarPos3 = new Vector3(443f, 0.05f, 230f);
        Vector3 SpawnCarPos4 = new Vector3(458f, 0.05f, 230f);
        Vector3 SpawnCarPos5 = new Vector3(453f, 0.05f, 230f);

        for(int i=0; i<SpawnCarPos.Length; i++)
        {
            SpawnCarPos[i] = new Vector3(433f + 5*i, 0.05f, 230f);
        }

        if (playerCount == 1)
        {
            PhotonNetwork.Instantiate("Player_Car_01", SpawnCarPos[0], SpawnRot, 0);
        }
        else if (playerCount == 2)
        {
            PhotonNetwork.Instantiate("Player_Car_01", SpawnCarPos[1], SpawnRot, 0);
        }
        else if (playerCount == 3)
        {
            PhotonNetwork.Instantiate("Player_Car_01", SpawnCarPos[2], SpawnRot, 0);
        }
        else if (playerCount == 4)
        {
            PhotonNetwork.Instantiate("Player_Car_01", SpawnCarPos[3], SpawnRot, 0);
        }
        else if (playerCount == 5)
        {
            PhotonNetwork.Instantiate("Player_Car_01", SpawnCarPos[4], SpawnRot, 0);
        }
    }

}

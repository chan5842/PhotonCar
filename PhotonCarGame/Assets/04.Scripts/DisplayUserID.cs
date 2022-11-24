using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayUserID : MonoBehaviour
{
    public Text UserID;
    PhotonView pv = null;
    void Start()
    {
        pv = PhotonView.Get(this);
        UserID.text = pv.owner.NickName;
    }

}

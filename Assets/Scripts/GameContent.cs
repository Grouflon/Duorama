using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomData
{
    public string id;
    public Room roomPrefab;
}

public class GameContent : MonoBehaviour
{
    public List<RoomData> rooms;

    public RoomData FindRoomData(string _id)
    {
        foreach (RoomData data in rooms)
        {
            if (_id == data.id)
            {
                return data;
            }
        }

        return new RoomData();
    }
}

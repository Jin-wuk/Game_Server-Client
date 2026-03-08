// 1. 보내는 용도 (Request)
[System.Serializable]
public class PositionSyncDto
{
    public int characterID;
    public float posX;
    public float posY;
}

// 2. 받는 용도 (Response)
[System.Serializable]
public class SyncResponseDto
{
    public string message;
}
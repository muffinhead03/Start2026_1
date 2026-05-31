public static class PuzzleConfigData
{
    public static PuzzleConfig GetConfig(string puzzleId)
    {
        switch (puzzleId)
        {
            case "wine_glass_room": return WineGlassRoomData.GetConfig();
            case "organ_room":      return OrganRoomData.GetConfig();
            // 새 씬 추가 시 여기에 case 한 줄만 추가
            default: return null;
        }
    }
}
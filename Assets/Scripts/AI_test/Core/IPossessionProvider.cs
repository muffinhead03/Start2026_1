using System.Collections.Generic;

/// <summary>
/// 플레이어가 현재 손/인벤토리에 무엇을 들고 있는지 HintManager에 알려주는 인터페이스.
/// HintManager는 이 인터페이스만 참조하고, 혜교님 파트의 구체 타입
/// (PerceiveObjectHandPivot, InventoryData 등)은 직접 참조하지 않음.
/// 각 씬의 XxxHintBridge가 이 인터페이스를 구현해서 자기 씬의 실제 데이터를 연결함
/// (해석: IPuzzleDataProvider / ISceneContextProvider와 같은 브릿지 패턴 — HintManager를
///  특정 씬의 구체 클래스에 결합시키지 않기 위한 것)
/// </summary>
public interface IPossessionProvider
{
    /// <summary>
    /// 플레이어가 현재 손에 들고 있는 오브젝트의 objectName. 없으면 null.
    /// </summary>
    string GetHandObjectName();

    /// <summary>
    /// 인벤토리에 들어있는 오브젝트들의 objectName 목록. 없으면 빈 리스트.
    /// </summary>
    List<string> GetInventoryObjectNames();
}
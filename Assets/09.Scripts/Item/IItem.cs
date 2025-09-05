using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    void Use(GameObject target); // 아이템 사용 메서드, target은 아이템을 사용할 대상
    // 느슨한 결합을 위해 인터페이스를 사용 느슨한 커플링
}
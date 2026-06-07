using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 최근 입력한 게임 입력 장치
// 이 객체는 씬에 항상 처음부터 있다.
// 이 객체에는 항상 플레이어 인풋이 등록되어 있다.
// 플레이어 인풋은 항상 활성화 되 있다.
// 정적 데이터로 최근 입력 장치를 가지고 있자.
public class SJ_PlayerInputRecentType : MonoBehaviour
{
    static public InputDevice inputDevice_Recent;

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onActionChange += OnActionChange;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnActionChange(object obj, InputActionChange change)
    {
        // 액션이 수행되었을 때(버튼 클릭, 스틱 이동 등)
        if (change == InputActionChange.ActionStarted)
        {
            var action = (InputAction)obj;
            var device = action.activeControl.device;
            inputDevice_Recent = device;
            // if (device is Keyboard || device is Mouse)
            // {
            //     lastUsedDevice = "Keyboard & Mouse";
            // }
            // else if (device is Gamepad)
            // {
            //     lastUsedDevice = "Gamepad";
            // }
        }
    }

    static public bool Check_KeyboardMouse()
    {
        if( inputDevice_Recent is Keyboard || inputDevice_Recent is Mouse ) return true;
        return false;
    }

    static public bool Check_Joypad()
    {
        if( inputDevice_Recent is Gamepad ) return true;
        return false;
    }

}

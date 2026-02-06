using TarodevController;
using UnityEngine;

public class InputReader : MonoBehaviour
{
    private ScriptableStats _stats;
    private PlayerController _pc;
    public FrameInput frameInput;

    private float _time;
    public InputReader(ScriptableStats stats, PlayerController pc)
    {
        _stats = stats;
        _pc = pc;
    }
    private void Awake()
    {

    }

    private void GatherInput()
    {
        //frameInput = new FrameInput
        //{
        //    JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
        //    JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
        //    Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        //};

        //if (_stats.SnapInput)
        //{
        //    frameInput.Move.x = Mathf.Abs(frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.x);
        //    frameInput.Move.y = Mathf.Abs(frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(frameInput.Move.y);
        //}

        //if (frameInput.JumpDown)
        //{
        //    _pc.jumpToConsume = true;
        //    _pc.timeJumpWasPressed = _time;
        //}
    }

    // Update is called once per frame
    public void Tick(float dt)
    {
        //_time += dt;
        //GatherInput();
    }



}

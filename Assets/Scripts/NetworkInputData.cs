using Fusion;
using UnityEngine;
enum MyButtons {
    Shoot = 0
}

public struct NetworkInputData : INetworkInput
{
	public NetworkButtons NetworkButtons;
	public const byte MOUSEBUTTON1 = 0x01;
	public const byte MOUSEBUTTON2 = 0x02;
	public byte buttons;
	public Vector3 direction;
	public float power;
	public float angle;
}


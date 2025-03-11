using Sandbox;
using Sandbox.Physics;

namespace Climb;
public sealed class ClimbingController : Component
{
	[Property] public float ArmLength { get; set; } = 64;
	[Property] public GameObject Arm { get; set; }
	[Property] public GameObject Hammer { get; set; }
	Rigidbody HammerBody { get; set; }
	protected override void OnStart()
	{
		Mouse.Visible = true;
		HammerBody = Hammer.GetComponent<Rigidbody>();
	}
	
	protected override void OnFixedUpdate()
	{
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();
		HammerBody.SmoothMove( tr.EndPosition.WithY( 0 ), 0.1f, Time.Delta );
		
		var dir = Vector3.Direction( Arm.WorldPosition, Hammer.WorldPosition );
		if ( Vector3.DistanceBetween( Arm.WorldPosition, Hammer.WorldPosition ) >= ArmLength )
		{
			HammerBody.WorldPosition = Arm.WorldPosition + dir * ArmLength;
		}
	}
}

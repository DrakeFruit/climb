using Sandbox;
using Sandbox.Physics;

namespace Climb;
public sealed class ClimbingController : Component
{
	[Property] public float ArmLength { get; set; } = 64;
	[Property] public GameObject Arm { get; set; }
	[Property] public GameObject Hammer { get; set; }
	Sandbox.Physics.FixedJoint GrabJoint { get; set; }
	PhysicsBody CursorBody { get; set; }
	Rigidbody HammerBody { get; set; }
	protected override void OnStart()
	{
		Mouse.Visible = true;
		HammerBody = Hammer.GetComponent<Rigidbody>();
		CursorBody = new PhysicsBody( Scene.PhysicsWorld ) { BodyType = PhysicsBodyType.Keyframed };
		
		GrabJoint = PhysicsJoint.CreateFixed( new PhysicsPoint( CursorBody ), new PhysicsPoint( HammerBody.PhysicsBody ) );
		GrabJoint.Point1 = new PhysicsPoint( CursorBody );
		GrabJoint.Point2 = new PhysicsPoint( HammerBody.PhysicsBody );
		
		var maxForce = 50 * HammerBody.Mass * Scene.PhysicsWorld.Gravity.Length;
		GrabJoint.SpringLinear = new PhysicsSpring( 15, 1, maxForce );
		GrabJoint.SpringAngular = new PhysicsSpring( 15, 1, 0 );
	}
	
	protected override void OnFixedUpdate()
	{
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();
		
		var newPos = tr.EndPosition.WithY( 0 );
		var dir = newPos - Arm.WorldPosition;
		dir = dir.ClampLength( ArmLength );
		newPos = Arm.WorldPosition + dir;
		
		CursorBody.Position = newPos;
	}
}

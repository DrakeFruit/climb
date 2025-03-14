using Sandbox;
using Sandbox.Physics;

namespace Climb;
public sealed class ClimbingController : Component
{
	[Property] public GameObject Hammer { get; set; }
	[Property] public GameObject Head { get; set; }
	Sandbox.Physics.FixedJoint GrabJoint { get; set; }
	PhysicsBody CursorBody { get; set; }
	Rigidbody HammerBody { get; set; }
	Rigidbody PlayerBody { get; set; }
	protected override void OnStart()
	{
		Mouse.Visible = true;
		HammerBody = Hammer.GetComponent<Rigidbody>();
		CursorBody = new PhysicsBody( Scene.PhysicsWorld ) { BodyType = PhysicsBodyType.Keyframed };
		PlayerBody = Components.Get<Rigidbody>();

		PhysicsPoint point1 = new( CursorBody );
		PhysicsPoint point2 = new( HammerBody.PhysicsBody, Head.LocalPosition );
		GrabJoint = PhysicsJoint.CreateFixed( point1, point2 );
		GrabJoint.Point1 = point1;
		GrabJoint.Point2 = point2;
		
		var maxForce = 50 * HammerBody.Mass * Scene.PhysicsWorld.Gravity.Length;
		GrabJoint.SpringLinear = new PhysicsSpring( 15, 2, maxForce );
		GrabJoint.SpringAngular = new PhysicsSpring( 15, 2, 0 );

		HammerBody.GameObject.Flags = HammerBody.GameObject.Flags.WithFlag( GameObjectFlags.Absolute, true );
	}
	
	protected override void OnFixedUpdate()
	{
		var tr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();
		
		//Movement
		var newPos = tr.EndPosition.WithY( 0 );
		CursorBody.Position = newPos;

		var dir = Vector3.Direction( HammerBody.WorldPosition, tr.EndPosition.WithY( 0 ) ).Normal; 
		var colTr = Scene.Trace.Ray( HammerBody.WorldPosition, HammerBody.WorldPosition + dir * 32 )
            .IgnoreGameObjectHierarchy( GameObject )
            .Run();
		if ( colTr.Hit )
		{
			PlayerBody.Velocity += -colTr.Direction.Normal.WithY( 0 ) * PlayerBody.Mass * Time.Delta / 4;
		}
	}
}

using System;
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
	PhysicsSpring TempSpring { get; set; }
	protected override void OnStart()
	{
		if ( IsProxy )
		{
			foreach ( var i in Components.GetAll<Rigidbody>() )
			{
				Log.Info( i  );
				i.Enabled = false;
			}
			foreach ( var i in Components.GetAll<Collider>() )
			{
				Log.Info( i  );
				i.Enabled = false;
			}

			return;
		}
		
		HammerBody = Hammer.GetComponent<Rigidbody>();
		CursorBody = new PhysicsBody( Scene.PhysicsWorld ) { BodyType = PhysicsBodyType.Keyframed };
		PlayerBody = Components.Get<Rigidbody>();
		
		Mouse.Visible = true;

		PhysicsPoint point1 = new( CursorBody );
		PhysicsPoint point2 = new( HammerBody.PhysicsBody, Head.LocalPosition );
		GrabJoint = PhysicsJoint.CreateFixed( point1, point2 );
		GrabJoint.Point1 = point1;
		GrabJoint.Point2 = point2;
		
		var maxForce = 100 * HammerBody.Mass * Scene.PhysicsWorld.Gravity.Length;
		GrabJoint.SpringLinear = new PhysicsSpring( 15, 2, maxForce );
		GrabJoint.SpringAngular = new PhysicsSpring( 15, 2, 0 );
		TempSpring = GrabJoint.SpringLinear;

		Hammer.Flags = Hammer.Flags.WithFlag( GameObjectFlags.Absolute, true );
	}
	
	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;

		Scene.Camera.WorldPosition = WorldPosition.WithY( -512 ) + Vector3.Zero.WithZ( 64 );
		var mouseTr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();
		CursorBody.Position = mouseTr.EndPosition.WithY( 0 );
		HammerBody.ApplyForce( PlayerBody.Velocity );

		if ( HammerBody.Touching.Any() )
		{
			var dir = Head.WorldPosition - CursorBody.Position;
			var force = dir.Normal * MathF.Pow( dir.Length, 2 );
			force = force.ClampLength( 0, 125 );
			PlayerBody.ApplyForceAt( Head.WorldPosition, force * PlayerBody.Mass * 25 );
		}
	}
}

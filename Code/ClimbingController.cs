using System;
using Sandbox;
using Sandbox.Physics;
using SliderJoint = Sandbox.SliderJoint;

namespace Climb;
public sealed class ClimbingController : Component
{
	[Property] public GameObject Hammer { get; set; }
	[Property] public SliderJoint Slider { get; set; }
	[Property] public GameObject Handle { get; set; }
	[Property] public GameObject Head { get; set; }
	[Property] public float Length { get; set; }
	[Property] public Vector3 Offset { get; set; }
	Sandbox.Physics.FixedJoint GrabJoint { get; set; }
	PhysicsBody CursorBody { get; set; }
	Rigidbody HammerBody { get; set; }
	Rigidbody PlayerBody { get; set; }
	Rigidbody SliderBody { get; set; }
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
		SliderBody = Slider.Components.Get<Rigidbody>();
		
		Mouse.Visible = true;

		PhysicsPoint point1 = new( CursorBody );
		PhysicsPoint point2 = new( HammerBody.PhysicsBody, Head.LocalPosition );
		GrabJoint = PhysicsJoint.CreateFixed( point1, point2 );
		GrabJoint.Point1 = point1;
		GrabJoint.Point2 = point2;
		
		var maxForce = 100 * HammerBody.Mass * Scene.PhysicsWorld.Gravity.Length;
		GrabJoint.SpringLinear = new PhysicsSpring( 15, 2, maxForce );
		GrabJoint.SpringAngular = new PhysicsSpring( 0, 0, 0 );

		Hammer.Flags = Hammer.Flags.WithFlag( GameObjectFlags.Absolute, true );
	}
	
	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		
		Scene.Camera.WorldPosition = WorldPosition.WithY( -300 ) + Vector3.Zero.WithZ( 42 );

		var offset = WorldPosition + Offset;
		var mouseTr = Scene.Trace.Ray( Scene.Camera.ScreenPixelToRay( Mouse.Position ), 1000 ).Run();
		var pos = offset + ( mouseTr.EndPosition.WithY( 0 ) - ( offset ) ).ClampLength( Length );
		CursorBody.Position = pos;

		var rot = Rotation.LookAt( CursorBody.Position - Slider.WorldPosition, Slider.LocalRotation.Forward );
		Gizmo.Draw.Arrow( Slider.WorldPosition, Slider.WorldPosition + rot.Forward * 20 );

		var pitchRotation = Rotation.FromAxis(Vector3.Left, 90);
		Slider.WorldRotation = rot * pitchRotation;

		if ( HammerBody.Touching.Any( x => !x.Tags.Has( "Player" ) ) )
		{
			var dir = Head.WorldPosition - CursorBody.Position;
			var force = dir.Normal * MathF.Pow( dir.Length, 2 );
			force = force.ClampLength( 0, 125 );
			PlayerBody.ApplyForceAt( Head.WorldPosition, force * PlayerBody.Mass * 25 );
		}
	}
}

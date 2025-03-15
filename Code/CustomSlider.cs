using Sandbox;
using Sandbox.Physics;

public sealed class CustomSlider : Component
{
	[Property] Rigidbody Body { get; set; }
	[Property] int Length { get; set; }
	[Property] Vector3 Offset { get; set; }
	public Rigidbody SelfBody { get; set; }
	protected override void OnStart()
	{
		if ( IsProxy ) return;
		
		SelfBody = Components.Get<Rigidbody>();
		var phys1 = new PhysicsPoint(SelfBody.PhysicsBody, SelfBody.WorldPosition + Offset);
		var phys2 = new PhysicsPoint(Body.PhysicsBody);
		PhysicsJoint joint = PhysicsJoint.CreateLength( phys1, phys2, Length );
		joint.Point1 = new PhysicsPoint( SelfBody.PhysicsBody, Offset );
		joint.Point2 = new PhysicsPoint( Body.PhysicsBody );
	}
	
	protected override void OnFixedUpdate()
	{
		if ( IsProxy ) return;
		
		var dir = WorldPosition + Offset - Body.WorldPosition;
		if ( dir.Length > Length )
		{
			
		}
	}
}

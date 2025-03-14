using Sandbox;
using Sandbox.Physics;

public sealed class CustomSlider : Component
{
	[Property] int Length;
	[Property] Rigidbody Body;
	protected override void OnStart()
	{
		Rigidbody rb = Components.Get<Rigidbody>();
		PhysicsJoint joint = PhysicsJoint.CreateLength( new PhysicsPoint( rb.PhysicsBody ), new PhysicsPoint( Body.PhysicsBody ), Length );
		joint.Point1 = new PhysicsPoint( rb.PhysicsBody );
		joint.Point2 = new PhysicsPoint( Body.PhysicsBody );
	}
}

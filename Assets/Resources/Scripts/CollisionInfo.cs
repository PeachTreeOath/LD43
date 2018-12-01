using UnityEngine;

public struct CollisionInfo {

    public Vector2 normal;
    public Vector2 contactPoint;
    public Vector2 impulse;

    public static CollisionInfo FromCollision(Collision2D collision) {
        CollisionInfo info = new CollisionInfo();

        info.normal = Vector2.zero;
        info.contactPoint = Vector2.zero;
        info.impulse = Vector2.zero;

        for(var i = 0; i < collision.contactCount; i++) {
            var contact = collision.GetContact(i);

            info.normal += contact.normal;
            info.contactPoint += contact.point;
            info.impulse += contact.normal * contact.normalImpulse; //TODO factor in tangent impulse?
        }

        info.normal *= collision.contactCount;
        info.contactPoint *= collision.contactCount;
        info.impulse *= collision.contactCount;

        return info;
    }
}

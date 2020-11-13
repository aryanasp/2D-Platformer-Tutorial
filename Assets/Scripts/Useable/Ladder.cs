using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Ladder : MonoBehaviour, IUseable
{

    [SerializeField]
    private Collider2D platformCollider;
    

    public void Use()
    {
        Player.Instance.OnLadder = true;
        if (Player.Instance.IsClimbing)
        {
            //we need to stop climbing
            UseLadder(false, 9.8f, 0, 1, "landing");
        }
        else
        {
            // we need to start climbing
            UseLadder(true, 0, 1, 0, "reset");
            Physics2D.IgnoreCollision(Player.Instance.GetComponent<Collider2D>(), platformCollider, true);
        }
    }

    private void UseLadder(bool onLadder, float gravity, int layerWeight, int animSpeed, string trigger)
    {
        Player.Instance.IsClimbing = onLadder;
        Player.Instance.MyRigidBody.gravityScale = gravity;
        Player.Instance.MyAnimator.SetLayerWeight(2, layerWeight);
        Player.Instance.MyAnimator.speed = animSpeed;
        Player.Instance.MyAnimator.SetTrigger(trigger);
    }

    public void ExitFromUseArea()
    {
        Player.Instance.OnLadder = false;
        UseLadder(false, 9.8f, 0, 1, "landing");
        Player.Instance.IsClimbing = false;
        Physics2D.IgnoreCollision(Player.Instance.GetComponent<Collider2D>(), platformCollider, false);
    }
}

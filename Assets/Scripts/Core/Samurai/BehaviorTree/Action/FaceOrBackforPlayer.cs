using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public class FaceOrBackforPlayer : SamuraiAction
    {
        public bool face = true;

        public override void OnStart()
        {
            var player = redHood;
            if (player != null)
            {
                if (face)
                {
                    samurai.FaceTarget(player.transform.position);
                }
                else
                {
                    samurai.FaceTarget(-player.transform.position);
                }
                
            }
        }
    }
}
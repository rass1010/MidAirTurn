using BepInEx;
using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using Utilla;

namespace DyingLight2RotateJump
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [ModdedGamemode]
    public class Plugin : BaseUnityPlugin
    {

        bool inAllowedRoom = false;

        bool holding;
        bool canTurn = true;

        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Update()
        {
            if (inAllowedRoom)
            {
                if (ControllerInputPoller.instance.rightControllerPrimary2DAxis.y < -0.9 && !holding && canTurn)
                {
                    //GorillaLocomotion.Player.Instance.rightControllerTransform.parent.eulerAngles += new Vector3(0, 180, 0);
                    StartCoroutine("RotateFunction");

                    
                    holding = true;
                }
                else if (ControllerInputPoller.instance.rightControllerPrimary2DAxis.y >= -0.9 && holding)
                {
                    holding = false;
                }
            }
        }

        IEnumerator RotateFunction()
        {
            Transform poll = GorillaLocomotion.Player.Instance.turnParent.transform;
            Vector3 pivotPos = GorillaLocomotion.Player.Instance.headCollider.transform.position;
            float degree = 180;
            Vector3 oldVel = new Vector3(GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity.x * -1, GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity.y, GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity.z * -1);
            float timeSinceStarted = 0f;
            canTurn = false;
            while (true)
            {
                timeSinceStarted += Time.deltaTime * 1f;
                poll.RotateAround(pivotPos, new Vector3(0, 1, 0), degree * Time.deltaTime * 2);
                GorillaLocomotion.Player.Instance.transform.GetComponent<Rigidbody>().velocity = (GorillaTagger.Instance.offlineVRRig.transform.up * 0.073f) * GorillaLocomotion.Player.Instance.scale;
                // If the object has arrived, stop the coroutine
                if (timeSinceStarted >= 0.45f)
                {
                    canTurn = true;
                    GorillaLocomotion.Player.Instance.bodyCollider.attachedRigidbody.velocity = oldVel;
                    yield break;
                }

                // Otherwise, continue next frame
                yield return null;
            }
        }

        [ModdedGamemodeJoin]
        private void RoomJoined(string gamemode)
        {
            // The room is modded. Enable mod stuff.
            inAllowedRoom = true;
        }

        [ModdedGamemodeLeave]
        private void RoomLeft(string gamemode)
        {
            // The room was left. Disable mod stuff.
            inAllowedRoom = false;
        }

    }
}
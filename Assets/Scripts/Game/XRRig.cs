using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace BinaryLift
{
    /// <summary>
    /// Minimal head + two-hand rig driven straight from UnityEngine.XR input devices
    /// (no XR Interaction Toolkit). Falls back to mouse look + click when no headset is active,
    /// so the game can be tried in the editor.
    /// </summary>
    public sealed class XRRig : MonoBehaviour
    {
        /// <summary>Degrees to tilt the controller ray down (+) / up (-) from the grip pose. Tune on device.</summary>
        public static float PointerPitch = 0f;

        public Camera Head { get; private set; }
        public Transform LeftHand { get; private set; }
        public Transform RightHand { get; private set; }
        public bool XRActive { get; private set; }
        public bool LeftTracked { get; private set; }
        public bool RightTracked { get; private set; }
        public bool SelectPressed { get; private set; }
        public bool RestartPressed { get; private set; }

        bool prevSelect, prevRestart, originSet;
        float yaw, pitch;
        static readonly List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();

        public static XRRig Create()
        {
            var go = new GameObject("XRRig");
            return go.AddComponent<XRRig>();
        }

        void Awake()
        {
            var headGo = new GameObject("Head");
            headGo.tag = "MainCamera";
            headGo.transform.SetParent(transform, false);
            Head = headGo.AddComponent<Camera>();
            Head.clearFlags = CameraClearFlags.SolidColor;
            Head.backgroundColor = new Color(0.05f, 0.07f, 0.12f);
            Head.nearClipPlane = 0.05f;
            Head.farClipPlane = 200f;
            headGo.AddComponent<AudioListener>();
            headGo.transform.localPosition = new Vector3(0f, 1.6f, 0f);

            LeftHand = MakeHand("LeftHand");
            RightHand = MakeHand("RightHand");
        }

        Transform MakeHand(string name)
        {
            var hand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hand.name = name;
            Destroy(hand.GetComponent<Collider>());
            hand.transform.SetParent(transform, false);
            hand.transform.localScale = new Vector3(0.05f, 0.05f, 0.12f);
            hand.SetActive(false);
            return hand.transform;
        }

        void OnEnable() { Application.onBeforeRender += ApplyPoses; }
        void OnDisable() { Application.onBeforeRender -= ApplyPoses; }

        /// <summary>Call once per frame from Update, before reading input state.</summary>
        public void Poll()
        {
            XRActive = XRSettings.isDeviceActive;
            bool select = false, restart = false;

            if (XRActive)
            {
                TrySetFloorOrigin();
                select = Button(XRNode.RightHand, CommonUsages.triggerButton)
                         || Button(XRNode.LeftHand, CommonUsages.triggerButton)
                         || Button(XRNode.RightHand, CommonUsages.primaryButton)
                         || Button(XRNode.LeftHand, CommonUsages.primaryButton);
                restart = Button(XRNode.RightHand, CommonUsages.secondaryButton)
                          || Button(XRNode.LeftHand, CommonUsages.secondaryButton);
            }
            else
            {
                try
                {
                    if (Input.GetMouseButton(1))
                    {
                        yaw += Input.GetAxis("Mouse X") * 3f;
                        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 3f, -85f, 85f);
                    }
                    select = Input.GetMouseButton(0);
                    restart = Input.GetKey(KeyCode.R);
                }
                catch (System.InvalidOperationException)
                {
                    // Project is set to the new Input System only; no editor fallback input.
                }
                Head.transform.localPosition = new Vector3(0f, 1.6f, 0f);
                Head.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
                LeftHand.gameObject.SetActive(false);
                RightHand.gameObject.SetActive(false);
                LeftTracked = RightTracked = false;
            }

            SelectPressed = select && !prevSelect;
            RestartPressed = restart && !prevRestart;
            prevSelect = select;
            prevRestart = restart;
        }

        void ApplyPoses()
        {
            if (!XRActive) return;

            var head = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (head.isValid)
            {
                Vector3 p; Quaternion r;
                bool gotP = head.TryGetFeatureValue(CommonUsages.centerEyePosition, out p)
                            || head.TryGetFeatureValue(CommonUsages.devicePosition, out p);
                bool gotR = head.TryGetFeatureValue(CommonUsages.centerEyeRotation, out r)
                            || head.TryGetFeatureValue(CommonUsages.deviceRotation, out r);
                if (gotP) Head.transform.localPosition = p;
                if (gotR) Head.transform.localRotation = r;
            }

            LeftTracked = ApplyHand(XRNode.LeftHand, LeftHand);
            RightTracked = ApplyHand(XRNode.RightHand, RightHand);
        }

        static bool ApplyHand(XRNode node, Transform target)
        {
            var dev = InputDevices.GetDeviceAtXRNode(node);
            bool tracked = dev.isValid
                           && dev.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 p)
                           && dev.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion r)
                           && (!dev.TryGetFeatureValue(CommonUsages.isTracked, out bool t) || t);
            if (tracked)
            {
                dev.TryGetFeatureValue(CommonUsages.devicePosition, out p);
                dev.TryGetFeatureValue(CommonUsages.deviceRotation, out r);
                target.localPosition = p;
                target.localRotation = r;
            }
            target.gameObject.SetActive(tracked);
            return tracked;
        }

        static bool Button(XRNode node, InputFeatureUsage<bool> usage)
        {
            var dev = InputDevices.GetDeviceAtXRNode(node);
            return dev.isValid && dev.TryGetFeatureValue(usage, out bool v) && v;
        }

        void TrySetFloorOrigin()
        {
            if (originSet) return;
            SubsystemManager.GetSubsystems(subsystems);
            foreach (var s in subsystems)
                if (s.running && s.TrySetTrackingOriginMode(TrackingOriginModeFlags.Floor))
                    originSet = true;
        }

        public bool TryGetHandRay(bool right, out Ray ray)
        {
            var hand = right ? RightHand : LeftHand;
            bool tracked = right ? RightTracked : LeftTracked;
            Vector3 dir = hand.rotation * Quaternion.Euler(PointerPitch, 0f, 0f) * Vector3.forward;
            ray = new Ray(hand.position, dir);
            return XRActive && tracked;
        }

        public Ray GazeRay => new Ray(Head.transform.position, Head.transform.forward);

        public Ray MouseRay => Head.ScreenPointToRay(Input.mousePosition);
    }
}

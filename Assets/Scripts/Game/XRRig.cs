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
        public static float PointerPitch = 70f;

        public Camera Head { get; private set; }
        public Transform LeftHand { get; private set; }
        public Transform RightHand { get; private set; }
        public bool XRActive { get; private set; }
        public bool LeftTracked { get; private set; }
        public bool RightTracked { get; private set; }
        public bool SelectPressed { get; private set; }
        public bool RestartPressed { get; private set; }
        public bool RightSelectPressed { get; private set; }
        public bool LeftSelectPressed { get; private set; }
        public bool RightGrip { get; private set; }
        public bool LeftGrip { get; private set; }
        public int SnapDirection { get; private set; }

        // Rotation from each controller's tracked pose to its pointing direction. Reset by pressing the thumbstick.
        Quaternion rightAim = Quaternion.identity, leftAim = Quaternion.identity;
        bool prevRightClick, prevLeftClick;
        bool prevSelect, prevRestart, originSet;
        bool prevRightSelect, prevLeftSelect, turnLatched;
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

        void Start()
        {
            Quaternion fallback = Quaternion.Euler(PointerPitch, 0f, 0f);
            rightAim = LoadAim("aimRight", fallback);
            leftAim = LoadAim("aimLeft", fallback);
        }

        static Quaternion LoadAim(string key, Quaternion fallback)
        {
            if (!PlayerPrefs.HasKey(key + "X")) return fallback;
            var q = new Quaternion(PlayerPrefs.GetFloat(key + "X"), PlayerPrefs.GetFloat(key + "Y"),
                PlayerPrefs.GetFloat(key + "Z"), PlayerPrefs.GetFloat(key + "W"));
            return q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w > .5f ? q : fallback;
        }

        static void SaveAim(string key, Quaternion q)
        {
            PlayerPrefs.SetFloat(key + "X", q.x); PlayerPrefs.SetFloat(key + "Y", q.y);
            PlayerPrefs.SetFloat(key + "Z", q.z); PlayerPrefs.SetFloat(key + "W", q.w);
            PlayerPrefs.Save();
        }

        /// <summary>Point the controller straight ahead (at the horizon) and call this: that pose becomes "pointer neutral".</summary>
        public void CalibratePointer(bool right)
        {
            var hand = right ? RightHand : LeftHand;
            Vector3 flat = Head.transform.forward;
            flat.y = 0f;
            if (flat.sqrMagnitude < .01f) flat = Vector3.forward;
            Quaternion aim = Quaternion.Inverse(hand.rotation) * Quaternion.LookRotation(flat.normalized, Vector3.up);
            if (right) { rightAim = aim; SaveAim("aimRight", aim); }
            else { leftAim = aim; SaveAim("aimLeft", aim); }
            Pulse(right, .6f, .15f);
        }

        void OnEnable() { Application.onBeforeRender += ApplyPoses; }
        void OnDisable() { Application.onBeforeRender -= ApplyPoses; }

        /// <summary>Call once per frame from Update, before reading input state.</summary>
        public void Poll()
        {
            XRActive = XRSettings.isDeviceActive;
            bool select = false, restart = false;
            bool rightSelect = false, leftSelect = false;
            float turn = 0;
            RightGrip = LeftGrip = false;
            SnapDirection = 0;

            if (XRActive)
            {
                TrySetFloorOrigin();
                ApplyPoses();
                rightSelect = Button(XRNode.RightHand, CommonUsages.triggerButton)
                         || Button(XRNode.RightHand, CommonUsages.primaryButton);
                leftSelect = Button(XRNode.LeftHand, CommonUsages.triggerButton)
                         || Button(XRNode.LeftHand, CommonUsages.primaryButton);
                select = rightSelect || leftSelect;
                RightGrip = RightTracked && Button(XRNode.RightHand, CommonUsages.gripButton);
                LeftGrip = LeftTracked && Button(XRNode.LeftHand, CommonUsages.gripButton);
                if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 stick))
                    turn = stick.x;
                restart = Button(XRNode.RightHand, CommonUsages.secondaryButton)
                          || Button(XRNode.LeftHand, CommonUsages.secondaryButton);

                bool rightClick = RightTracked && Button(XRNode.RightHand, CommonUsages.primary2DAxisClick);
                bool leftClick = LeftTracked && Button(XRNode.LeftHand, CommonUsages.primary2DAxisClick);
                if (rightClick && !prevRightClick) CalibratePointer(true);
                if (leftClick && !prevLeftClick) CalibratePointer(false);
                prevRightClick = rightClick;
                prevLeftClick = leftClick;
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
                    rightSelect = select;
                    turn = Input.GetKey(KeyCode.Q) ? -1 : Input.GetKey(KeyCode.E) ? 1 : 0;
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
            RightSelectPressed = rightSelect && !prevRightSelect;
            LeftSelectPressed = leftSelect && !prevLeftSelect;
            prevRightSelect = rightSelect;
            prevLeftSelect = leftSelect;
            if (Mathf.Abs(turn) < .3f) turnLatched = false;
            if (!turnLatched && Mathf.Abs(turn) > .75f)
            {
                SnapDirection = turn > 0 ? 1 : -1;
                turnLatched = true;
            }
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
            Vector3 dir = hand.rotation * (right ? rightAim : leftAim) * Vector3.forward;
            ray = new Ray(hand.position, dir);
            return XRActive && tracked;
        }

        public Ray GazeRay => new Ray(Head.transform.position, Head.transform.forward);

        public Ray MouseRay => Head.ScreenPointToRay(Input.mousePosition);

        public void Pulse(bool right, float amplitude = .25f, float duration = .06f)
        {
            var device = InputDevices.GetDeviceAtXRNode(right ? XRNode.RightHand : XRNode.LeftHand);
            if (device.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
                device.SendHapticImpulse(0, Mathf.Clamp01(amplitude), duration);
        }

        public void TeleportFeet(Vector3 destination)
        {
            // Preserve the tracked room offset and head rotation; place the feet under the headset.
            Vector3 offset = Head.transform.position - transform.position;
            offset.y = 0;
            transform.position = destination - offset;
        }

        public void SetDesktopView(float newPitch, float newYaw = 0)
        {
            if (XRActive) return;
            pitch = newPitch; yaw = newYaw;
            Head.transform.localRotation = Quaternion.Euler(pitch, yaw, 0);
        }

        public void SnapTurn(int direction)
        {
            transform.RotateAround(Head.transform.position, Vector3.up, direction * 30f);
        }
    }
}

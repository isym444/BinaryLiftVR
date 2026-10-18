using UnityEngine;

namespace BinaryLift
{
    /// <summary>Approved skinned character. Presentation only: no route or player movement.</summary>
    public sealed class HighlandCalf : MonoBehaviour
    {
        public Animation Motion;
        public Transform Head,Mouth;
        public SkinnedMeshRenderer Body;
        public Transform Funding { get; private set; }
        public string CurrentClip { get; private set; }
        public bool Carrying => Funding!=null&&Funding.gameObject.activeSelf;
        public void BuildFunding(CampusWorld art)
        {
            Funding=art.Root("Carried research funding",Mouth);
            // Compensate for the small visual root: the envelope remains readable at chase distance.
            Funding.localScale=new Vector3(1/.29f,1/.215f,1/.20f);
            art.Box("Funding envelope",Funding,new Vector3(0,-.14f,0),new Vector3(.42f,.29f,.04f),art.Orange);
            art.Box("Envelope seal",Funding,new Vector3(0,-.14f,-.025f),new Vector3(.065f,.06f,.014f),art.Dark);
            SetCarrying(false);Play("Idle",true);
        }
        public void SetCarrying(bool value){if(Funding!=null)Funding.gameObject.SetActive(value);}
        public void Play(string clip,bool restart=false,float speed=1)
        {
            if(Motion==null||Motion[clip]==null)return;
            Motion[clip].speed=speed;
            if(!restart&&CurrentClip==clip&&Motion.IsPlaying(clip))return;
            CurrentClip=clip;if(restart){Motion.Stop();Motion[clip].time=0;Motion.Play(clip);}else Motion.CrossFade(clip,.18f);
        }
        public void ResetPose(){SetCarrying(false);transform.rotation=Quaternion.identity;Play("Idle",true);}
        void LateUpdate(){if(Head!=null)Head.localScale=Vector3.one*1.17f;}
    }
}

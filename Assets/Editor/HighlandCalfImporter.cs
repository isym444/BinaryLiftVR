using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace BinaryLift.EditorTools
{
    // A narrow importer for the reviewed offline export, not a general-purpose glTF loader.
    [ScriptedImporter(2,"calf")]
    public sealed class HighlandCalfImporter : ScriptedImporter
    {
        [Serializable] public class Node { public string name; public int parent; public float[] position,rotation,scale; }
        [Serializable] public class Palette { public string name; public float[] color; }
        [Serializable] public class Part { public int material; public int[] triangles; }
        [Serializable] public class Track { public int node,size; public string property; public float[] times,values; }
        [Serializable] public class Clip { public string name; public float duration; public Track[] tracks; }
        [Serializable] public class Data
        {
            public int version,head; public Node[] nodes; public int[] bones,joints;
            public float[] positions,normals,weights; public Palette[] materials; public Part[] parts; public Clip[] clips;
        }
        static Vector3 V(float[] a,int i=0)=>new Vector3(a[i],a[i+1],-a[i+2]);
        static Quaternion Q(float[] a)=>new Quaternion(-a[0],-a[1],a[2],a[3]);
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var data=JsonUtility.FromJson<Data>(File.ReadAllText(ctx.assetPath));
            if(data.version!=1||data.bones.Length!=42||data.clips.Length!=13)throw new InvalidDataException("Unexpected calf rig/version.");
            var root=new GameObject("Highland calf");
            var visual=new GameObject("Approved proportions").transform;visual.SetParent(root.transform,false);
            var nodes=new Transform[data.nodes.Length];
            for(int i=0;i<nodes.Length;i++)
            {
                var n=data.nodes[i];nodes[i]=new GameObject(n.name).transform;
                nodes[i].SetParent(n.parent<0?visual:nodes[n.parent],false);
                nodes[i].localPosition=V(n.position);nodes[i].localRotation=Q(n.rotation);
                nodes[i].localScale=new Vector3(n.scale[0],n.scale[1],n.scale[2]);
            }
            var bones=data.bones.Select(i=>nodes[i]).ToArray();
            int count=data.positions.Length/3;
            var vertices=new Vector3[count];var normals=new Vector3[count];var weights=new BoneWeight[count];var colors=new Color[count];
            for(int i=0;i<count;i++)
            {
                vertices[i]=V(data.positions,i*3);normals[i]=V(data.normals,i*3);
                weights[i]=new BoneWeight{boneIndex0=data.joints[i*4],boneIndex1=data.joints[i*4+1],boneIndex2=data.joints[i*4+2],boneIndex3=data.joints[i*4+3],
                    weight0=data.weights[i*4],weight1=data.weights[i*4+1],weight2=data.weights[i*4+2],weight3=data.weights[i*4+3]};
            }
            var triangles=data.parts.SelectMany(p=>p.triangles).ToArray();
            for(int i=0;i<triangles.Length;i+=3){int swap=triangles[i+1];triangles[i+1]=triangles[i+2];triangles[i+2]=swap;}
            foreach(var p in data.parts){var c=data.materials[p.material].color;foreach(int i in p.triangles)colors[i]=new Color(c[0],c[1],c[2],1);}
            var mesh=new Mesh{name="Approved calf body and bound coat",vertices=vertices,normals=normals,colors=colors,triangles=triangles,boneWeights=weights};
            mesh.bindposes=bones.Select(b=>b.worldToLocalMatrix*visual.localToWorldMatrix).ToArray();mesh.RecalculateBounds();
            ctx.AddObjectToAsset("mesh",mesh);
            ctx.DependsOnSourceAsset("Assets/Resources/CalfCoat.shader");
            var shader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/Resources/CalfCoat.shader");if(shader==null)throw new InvalidDataException("CalfCoat shader missing.");
            var material=new Material(shader){name="Warm ginger calf palette"};ctx.AddObjectToAsset("material",material);
            var renderer=visual.gameObject.AddComponent<SkinnedMeshRenderer>();renderer.sharedMesh=mesh;renderer.sharedMaterial=material;renderer.bones=bones;renderer.rootBone=nodes[0];
            renderer.quality=SkinQuality.Bone4;renderer.localBounds=new Bounds(new Vector3(0,2,-.5f),new Vector3(8,8,14));
            var animation=root.AddComponent<Animation>();animation.playAutomatically=false;animation.cullingType=AnimationCullingType.AlwaysAnimate;
            foreach(var source in data.clips)
            {
                var clip=new AnimationClip{name=source.name,legacy=true,frameRate=30,wrapMode=WrapMode.Loop};
                foreach(var track in source.tracks)
                {
                    string property=track.property=="position"?"localPosition":track.property=="quaternion"?"localRotation":"localScale";
                    string path=AnimationUtility.CalculateTransformPath(nodes[track.node],root.transform);
                    for(int component=0;component<track.size;component++)
                    {
                        var keys=new Keyframe[track.times.Length];
                        float sign=track.property=="position"&&component==2||track.property=="quaternion"&&component<2?-1:1;
                        for(int k=0;k<keys.Length;k++)keys[k]=new Keyframe(track.times[k],track.values[k*track.size+component]*sign);
                        var curve=new AnimationCurve(keys);
                        for(int k=0;k<keys.Length;k++){AnimationUtility.SetKeyLeftTangentMode(curve,k,AnimationUtility.TangentMode.Linear);AnimationUtility.SetKeyRightTangentMode(curve,k,AnimationUtility.TangentMode.Linear);}
                        clip.SetCurve(path,typeof(Transform),property+"."+"xyzw"[component],curve);
                    }
                }
                clip.EnsureQuaternionContinuity();ctx.AddObjectToAsset("clip_"+source.name,clip);animation.AddClip(clip,source.name);
                if(source.name=="Idle")animation.clip=clip;
            }
            var mouth=new GameObject("Funding mouth socket").transform;
            mouth.SetParent(visual,false);mouth.localPosition=new Vector3(0,3.48f,-4.72f);mouth.SetParent(nodes[data.head],true);
            // Preserve preview proportions above the bind pose; facing +Z is the gameplay contract.
            visual.localScale=new Vector3(.29f,.215f,.20f);visual.localPosition=new Vector3(0,.015f,-.18f);visual.localRotation=Quaternion.Euler(0,180,0);
            var actor=root.AddComponent<HighlandCalf>();actor.Motion=animation;actor.Head=nodes[data.head];actor.Mouth=mouth;actor.Body=renderer;
            ctx.AddObjectToAsset("root",root);ctx.SetMainObject(root);
        }
    }
}

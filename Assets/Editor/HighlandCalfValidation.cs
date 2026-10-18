using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BinaryLift.EditorTools
{
    public static class HighlandCalfValidation
    {
        [MenuItem("BinaryLift/Validate Highland Calf %#h")]
        public static void Validate()
        {
            string report="Highland calf native asset checks\n";int failed=0;
            Action<bool,string> check=(ok,label)=>{report+=(ok?"PASS ":"FAIL ")+label+"\n";if(!ok)failed++;};
            AssetDatabase.ImportAsset("Assets/Resources/Approved/HighlandCalf.calf",ImportAssetOptions.ForceUpdate);
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Approved/HighlandCalf.calf");
            check(prefab!=null,"imported prefab exists");
            if(prefab!=null)
            {
                var instance=UnityEngine.Object.Instantiate(prefab);instance.hideFlags=HideFlags.HideAndDontSave;
                try
                {
                    var calf=instance.GetComponent<HighlandCalf>();var mesh=new Mesh();
                    check(calf.Body.sharedMesh.triangles.Length/3==7594,"approved geometry retained: 7594 triangles");
                    check(calf.Body.sharedMesh.subMeshCount==1&&calf.Body.sharedMaterials.Length==1,"body and coat share one material and submesh");
                    check(!ShaderUtil.ShaderHasError(calf.Body.sharedMaterial.shader),"coat shader compiles");
                    check(calf.Motion.GetClipCount()==13&&calf.Body.bones.Length==42,"13 native clips and 42 bones");
                    foreach(AnimationState state in calf.Motion)
                    {
                        state.clip.SampleAnimation(instance,0);calf.Head.localScale=Vector3.one*1.17f;calf.Body.BakeMesh(mesh);
                        var first=mesh.vertices;bool moving=false,finite=true,contained=true;
                        // Sample every clip at eight points, including its end, for skin/bounds faults.
                        for(int i=1;i<=8;i++)
                        {
                            state.clip.SampleAnimation(instance,state.length*i/8);calf.Head.localScale=Vector3.one*1.17f;calf.Body.BakeMesh(mesh);
                            var vertices=mesh.vertices;
                            for(int v=0;v<vertices.Length;v++)
                            {
                                finite&=float.IsFinite(vertices[v].x)&&float.IsFinite(vertices[v].y)&&float.IsFinite(vertices[v].z);
                                moving|=(vertices[v]-first[v]).sqrMagnitude>.00001f;
                                contained&=calf.Body.localBounds.Contains(vertices[v]);
                            }
                        }
                        check(finite&&moving&&contained,state.name+" deforms with finite vertices inside skin bounds");
                    }
                    UnityEngine.Object.DestroyImmediate(mesh);
                }
                catch(Exception e){check(false,e.ToString());}
                finally{UnityEngine.Object.DestroyImmediate(instance);}
            }
            report+="Result: "+(failed==0?"PASS":"FAIL")+"\nHeadset validation: NOT RUN\n";
            Directory.CreateDirectory("Logs");File.WriteAllText("Logs/calf-asset-checks.txt",report);Debug.Log(report);
        }
    }
}

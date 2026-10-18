using UnityEngine;
namespace BinaryLift
{
    public static class CampusAssets
    {
        public static Transform Place(CampusWorld art,string pack,string name,Transform parent,Vector3 position,Vector3 size)
        {
            var prefab=Resources.Load<GameObject>("Approved/"+pack+"/"+name);if(prefab==null)return null;
            var root=art.Root(name,parent);root.localPosition=position;
            var model=Object.Instantiate(prefab,root,false);model.transform.localPosition=Vector3.zero;
            var renderers=model.GetComponentsInChildren<Renderer>();if(renderers.Length==0)return root;
            var bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
            var scale=new Vector3(size.x/Mathf.Max(bounds.size.x,.001f),size.y/Mathf.Max(bounds.size.y,.001f),size.z/Mathf.Max(bounds.size.z,.001f));
            model.transform.localScale=Vector3.Scale(model.transform.localScale,scale);
            bounds=renderers[0].bounds;foreach(var r in renderers)bounds.Encapsulate(r.bounds);
            model.transform.position+=root.position-new Vector3(bounds.center.x,bounds.min.y,bounds.center.z);
            var material=art.Mat(Color.white);material.mainTexture=Resources.Load<Texture2D>("Approved/"+pack+"/colormap");
            foreach(var r in renderers){if(material.mainTexture!=null){var slots=new Material[r.sharedMaterials.Length];for(int i=0;i<slots.Length;i++)slots[i]=material;r.sharedMaterials=slots;}r.receiveShadows=true;r.shadowCastingMode=UnityEngine.Rendering.ShadowCastingMode.On;}
            return root;
        }
        public static Material Surface(CampusWorld art,string name,Vector2 repeat)
        {
            var mat=art.Mat(Color.white);var shader=Resources.Load<Shader>("CampusPBR");if(shader!=null)mat.shader=shader;
            mat.mainTexture=Resources.Load<Texture2D>("Approved/"+name+"_diff");mat.mainTextureScale=repeat;
            mat.SetTexture("_BumpMap",Resources.Load<Texture2D>("Approved/"+name+"_nor_gl"));
            mat.SetTexture("_Roughness",Resources.Load<Texture2D>("Approved/"+name+"_rough"));return mat;
        }
    }
}

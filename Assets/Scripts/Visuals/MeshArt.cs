using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class MeshArt
 {
  private static readonly Dictionary<string,Material> Materials=new();
  public static Material Mat(string key,Color color,int atlas=-1,bool glow=false){if(Materials.TryGetValue(key,out var cached))return cached;var m=new Material(Resources.Load<Material>("Materials/Surface")){name=key,color=color};m.SetFloat("_Glossiness",.28f);
   if(atlas>=0){m.mainTexture=Resources.Load<Texture2D>("Art/TerrainAtlas");m.mainTextureScale=Vector2.one*.49f;m.mainTextureOffset=new Vector2((atlas%2)*.5f+.005f,atlas<2?.505f:.005f);}
   if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*2.5f);}Materials.Add(key,m);return m;}
  public static GameObject Mesh(Transform parent,string name,Vector3[] v,int[] t,Vector2[] uv,Material material,bool collide=false,Color[] colors=null){var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);var valid=new List<int>();for(int i=0;i<t.Length;i+=3)if(Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).sqrMagnitude>1e-18f)valid.AddRange(new[]{t[i],t[i+1],t[i+2]});t=valid.ToArray();var mesh=new Mesh{name=name};mesh.vertices=v;mesh.triangles=t;if(uv!=null)mesh.uv=uv;if(colors!=null)mesh.colors=colors;mesh.RecalculateNormals();bool invalid=false;foreach(var normal in mesh.normals)if(normal.sqrMagnitude<.01f){invalid=true;break;}
   if(invalid){var expanded=new Vector3[t.Length];var tex=uv!=null?new Vector2[t.Length]:null;var indices=new int[t.Length];for(int i=0;i<t.Length;i++){expanded[i]=v[t[i]];if(tex!=null)tex[i]=uv[t[i]];indices[i]=i;}mesh.Clear();mesh.vertices=expanded;mesh.triangles=indices;if(tex!=null)mesh.uv=tex;mesh.RecalculateNormals();}mesh.RecalculateBounds();go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=material;
   if(collide)go.AddComponent<MeshCollider>().sharedMesh=mesh;return go;}
  // Authored surface of revolution; each ring supplies its own silhouette radius and elevation.
  public static GameObject Lathe(Transform parent,string name,float[] y,float[] radius,int sides,Material material,Vector3 scale,bool faceted=false){var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();
   for(int j=0;j<y.Length;j++)for(int i=0;i<=sides;i++){float a=i*Mathf.PI*2/sides;v.Add(Vector3.Scale(new Vector3(Mathf.Cos(a)*radius[j],y[j],Mathf.Sin(a)*radius[j]),scale));uv.Add(new Vector2((float)i/sides,(float)j/(y.Length-1)));}
   for(int j=0;j<y.Length-1;j++)for(int i=0;i<sides;i++){int a=j*(sides+1)+i,b=a+sides+1;t.AddRange(new[]{a,b,a+1,a+1,b,b+1});}
   if(faceted){var fv=new Vector3[t.Count];var fu=new Vector2[t.Count];var ft=new int[t.Count];for(int i=0;i<t.Count;i++){fv[i]=v[t[i]];fu[i]=uv[t[i]];ft[i]=i;}return Mesh(parent,name,fv,ft,fu,material);}
   return Mesh(parent,name,v.ToArray(),t.ToArray(),uv.ToArray(),material);
  }
  public static GameObject Oval(Transform parent,string name,Vector3 position,Vector3 scale,Material material,int sides=16){var y=new float[13];var r=new float[13];for(int i=0;i<13;i++){float a=Mathf.PI*i/12;y[i]=-Mathf.Cos(a)*.5f;r[i]=Mathf.Sin(a)*.5f;}var go=Lathe(parent,name,y,r,sides,material,scale);go.transform.localPosition=position;return go;}
  public static GameObject Box(Transform parent,string name,Vector3 position,Vector3 scale,Material mat){var v=new[]{new Vector3(-.5f,-.5f,-.5f),new Vector3(.5f,-.5f,-.5f),new Vector3(.5f,.5f,-.5f),new Vector3(-.5f,.5f,-.5f),new Vector3(-.5f,-.5f,.5f),new Vector3(.5f,-.5f,.5f),new Vector3(.5f,.5f,.5f),new Vector3(-.5f,.5f,.5f)};int[] t={0,2,1,0,3,2,4,5,6,4,6,7,0,4,7,0,7,3,1,2,6,1,6,5,3,7,6,3,6,2,0,1,5,0,5,4};var outv=new Vector3[t.Length];var uv=new Vector2[t.Length];var tris=new int[t.Length];for(int i=0;i<t.Length;i++){outv[i]=Vector3.Scale(v[t[i]],scale);uv[i]=new Vector2(i%3==1?1:0,i%3==2?1:0);tris[i]=i;}var go=Mesh(parent,name,outv,tris,uv,mat);go.transform.localPosition=position;return go;}
  public static GameObject Ring(Transform parent,string name,float radius,float width,float y,Material mat,int sides=64,float arc=360){var v=new Vector3[(sides+1)*2];var t=new int[sides*6];for(int i=0;i<=sides;i++){float a=i*arc/sides*Mathf.Deg2Rad;v[i*2]=new Vector3(Mathf.Cos(a)*radius,y,Mathf.Sin(a)*radius);v[i*2+1]=new Vector3(Mathf.Cos(a)*(radius-width),y,Mathf.Sin(a)*(radius-width));if(i<sides){int k=i*6,a0=i*2;t[k]=a0;t[k+1]=a0+1;t[k+2]=a0+2;t[k+3]=a0+2;t[k+4]=a0+1;t[k+5]=a0+3;}}return Mesh(parent,name,v,t,null,mat);}
 }
}

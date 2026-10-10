using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
namespace Kamilunavo.RisingSteps.Visuals
{
 public static class MeshArt
 {
  private static readonly Dictionary<string,Material> Materials=new();
  public static Material Mat(string key,Color color,int atlas=-1,bool glow=false,float emission=2.5f){if(Materials.TryGetValue(key,out var cached))return cached;var m=new Material(Resources.Load<Material>(glow?"Materials/Glow":"Materials/Surface")){name=key,color=color};m.SetFloat("_Glossiness",.28f);
   if(atlas>=0){m.mainTexture=Resources.Load<Texture2D>("Art/TerrainAtlas");m.mainTextureScale=Vector2.one*.49f;m.mainTextureOffset=new Vector2((atlas%2)*.5f+.005f,atlas<2?.505f:.005f);}
   if(glow){m.EnableKeyword("_EMISSION");m.SetColor("_EmissionColor",color*emission);}Materials.Add(key,m);return m;}
  // One neutral baked detail texture is shared by the finite stone material kit.
  // No moss/foliage is painted onto the dry temple or mineral water surfaces.
  private static Texture2D _stoneDetail;
  private static float StoneHash(int x,int y,int salt){unchecked{uint h=(uint)((x*374761393)^(y*668265263)^(salt*1274126177));h=(h^(h>>13))*1274126177u;return ((h^(h>>16))&65535)/65535f;}}
  private static float StoneNoise(float x,float y,int period,int salt){int ix=Mathf.FloorToInt(x),iy=Mathf.FloorToInt(y);float fx=x-ix,fy=y-iy;fx=fx*fx*(3-2*fx);fy=fy*fy*(3-2*fy);
   float Sample(int a,int b)=>StoneHash((a%period+period)%period,(b%period+period)%period,salt);
   return Mathf.Lerp(Mathf.Lerp(Sample(ix,iy),Sample(ix+1,iy),fx),Mathf.Lerp(Sample(ix,iy+1),Sample(ix+1,iy+1),fx),fy);
  }
  public static Material Stone(string key,Color color){var material=Mat(key,color);if(_stoneDetail==null){const int size=256;var pixels=new Color[size*size];for(int y=0;y<size;y++)for(int x=0;x<size;x++){
    float u=(float)x/size,v=(float)y/size;
    float broad=StoneNoise(u*4,v*4,4,11),mineral=StoneNoise(u*13,v*13,13,17),grain=StoneNoise(u*47,v*47,47,23);
    // Periodic domain-warped mineral seams remain faint, open and sparse: no
    // outlined Voronoi polygons, hard tile borders or repeated moss shapes.
    float bend=Mathf.Sin(v*Mathf.PI*2)*.7f+Mathf.Sin(v*Mathf.PI*6+u*Mathf.PI*2)*.28f;
    float seam=Mathf.Abs(Mathf.Sin(u*Mathf.PI*4+bend));float fracture=(1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(0,.026f,seam)))*Mathf.SmoothStep(0,1,Mathf.InverseLerp(.55f,.78f,mineral))*.032f;
    float value=.87f+(broad-.5f)*.055f+(mineral-.5f)*.030f+(grain-.5f)*.016f+(StoneHash(x,y,31)-.5f)*.006f-fracture;
    pixels[y*size+x]=new Color(value,value,value,1);
   }_stoneDetail=new Texture2D(size,size,TextureFormat.RGB24,true){name="NeutralBakedStoneDetail",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear,anisoLevel=2};_stoneDetail.SetPixels(pixels);_stoneDetail.Apply(true,true);}
   material.mainTexture=_stoneDetail;material.mainTextureScale=Vector2.one;material.mainTextureOffset=Vector2.zero;material.SetFloat("_Glossiness",.10f);return material;
  }
  public static GameObject Mesh(Transform parent,string name,Vector3[] v,int[] t,Vector2[] uv,Material material,bool collide=false,Color[] colors=null){var go=new GameObject(name,typeof(MeshFilter),typeof(MeshRenderer));go.transform.SetParent(parent,false);var valid=new List<int>();for(int i=0;i<t.Length;i+=3)if(Vector3.Cross(v[t[i+1]]-v[t[i]],v[t[i+2]]-v[t[i]]).sqrMagnitude>1e-18f)valid.AddRange(new[]{t[i],t[i+1],t[i+2]});t=valid.ToArray();var used=new Dictionary<int,int>();var compact=new List<Vector3>();var cuv=uv!=null?new List<Vector2>():null;var cc=colors!=null?new List<Color>():null;for(int i=0;i<t.Length;i++){int old=t[i];if(!used.TryGetValue(old,out int index)){index=compact.Count;used.Add(old,index);compact.Add(v[old]);if(cuv!=null)cuv.Add(uv[old]);if(cc!=null)cc.Add(colors[old]);}t[i]=index;}v=compact.ToArray();uv=cuv?.ToArray();colors=cc?.ToArray();var mesh=new Mesh{name=name};mesh.vertices=v;mesh.triangles=t;if(uv!=null)mesh.uv=uv;if(colors!=null)mesh.colors=colors;mesh.RecalculateNormals();bool invalid=false;foreach(var normal in mesh.normals)if(normal.sqrMagnitude<.01f){invalid=true;break;}
   if(invalid){var expanded=new Vector3[t.Length];var tex=uv!=null?new Vector2[t.Length]:null;var indices=new int[t.Length];for(int i=0;i<t.Length;i++){expanded[i]=v[t[i]];if(tex!=null)tex[i]=uv[t[i]];indices[i]=i;}mesh.Clear();mesh.vertices=expanded;mesh.triangles=indices;if(tex!=null)mesh.uv=tex;mesh.RecalculateNormals();}mesh.RecalculateBounds();go.GetComponent<MeshFilter>().sharedMesh=mesh;go.GetComponent<MeshRenderer>().sharedMaterial=material;
   ArtLifetime.Own(go,mesh);if(collide)go.AddComponent<MeshCollider>().sharedMesh=mesh;return go;}
  public static void Batch(GameObject root){StaticBatchingUtility.Combine(root);foreach(var filter in root.GetComponentsInChildren<MeshFilter>())ArtLifetime.Own(root,filter.sharedMesh);}
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

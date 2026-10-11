using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Gameplay;
using Kamilunavo.RisingSteps.Core;

namespace Kamilunavo.RisingSteps.Visuals
{
 // A finite original texture kit shared across a course, not one texture per island.
 // Standalone preview/validation roots own their own kit. ArtLifetime releases all assets.
 [ExecuteAlways] public sealed class LandscapePalette:MonoBehaviour
 {
  private readonly Dictionary<int,Material> _materials=new();
  private Texture2D _mineral,_turf,_water,_sand,_ice,_pool,_moss;
  public static Material Pool(Transform parent)=>Owner(parent).Special(100);
  public static Material Moss(Transform parent)=>Owner(parent).Special(101);
  public static Material Surface(Transform parent,int realm,int kind=0)
  {
   realm=(int)RisingRealmCatalog.Theme(realm);kind=Mathf.Clamp(kind,0,7);
   return Owner(parent).Get(realm,kind);
  }
  public static Material Foliage(Transform parent,bool sunlit)=>Owner(parent).Get(0,sunlit?4:3);
  private static LandscapePalette Owner(Transform parent)
  {
   var course=parent.GetComponentInParent<RisingCourse>();var inherited=parent.GetComponentInParent<LandscapePalette>();var owner=course!=null?course.gameObject:inherited!=null?inherited.gameObject:parent.gameObject;
   return owner.GetComponent<LandscapePalette>()??owner.AddComponent<LandscapePalette>();
  }
  private static float Hash(int x,int y){unchecked{uint h=(uint)(x*374761393+y*668265263);h=(h^(h>>13))*1274126177;return ((h^(h>>16))&65535)/65535f;}}
  private static float Noise(float x,float y,int period)
  {
   int ix=Mathf.FloorToInt(x),iy=Mathf.FloorToInt(y);float fx=x-ix,fy=y-iy;fx=fx*fx*(3-2*fx);fy=fy*fy*(3-2*fy);
   float H(int a,int b)=>Hash((a%period+period)%period,(b%period+period)%period);
   return Mathf.Lerp(Mathf.Lerp(H(ix,iy),H(ix+1,iy),fx),Mathf.Lerp(H(ix,iy+1),H(ix+1,iy+1),fx),fy);
  }
  private Texture2D Texture(int kind)
  {
   int width=kind==0?256:128,height=kind==2?256:width;var pixels=new Color[width*height];
   for(int y=0;y<height;y++)for(int x=0;x<width;x++){
    float u=(float)x/width,v=(float)y/height,broad=Noise(u*5,v*5,5),grain=Noise(u*37,v*37,37);
    if(kind==0){
     // Interrupted veins and broad mineral deposits, not outlined repeated polygon cells.
     float bend=Mathf.Sin(v*6.28318f)*.36f+Mathf.Sin(v*18.8495f+u*6.28318f)*.12f;
     float fissure=Mathf.Abs(Mathf.Sin(u*12.56637f+bend));float cut=(1-Mathf.Clamp01(fissure/.032f))*Mathf.Clamp01((broad-.36f)*4);
     float deposits=Noise(u*13+.7f,v*13+.4f,13);
     float bedding=.5f+.5f*Mathf.Sin(v*50.26548f+bend*.8f);
     float sediment=Mathf.Pow(1-bedding,10)*(.30f+.70f*deposits);
     float value=.91f+(broad-.5f)*.17f+(deposits-.5f)*.13f+(grain-.5f)*.045f-cut*.26f-sediment*.13f;
     pixels[y*width+x]=new Color(value,value*(.98f+broad*.02f),value*(.94f+deposits*.04f));
    }else if(kind==1){
     pixels[y*width+x]=Color.Lerp(new Color(.32f,.49f,.19f),new Color(.58f,.70f,.34f),broad*.70f+grain*.20f);
    }else if(kind==3){
     float ripple=.5f+.5f*Mathf.Sin(v*125.6637f+Mathf.Sin(u*12.56637f)*.55f);float value=.82f+broad*.10f+ripple*.035f+grain*.025f;
     pixels[y*width+x]=new Color(value,value*.96f,value*.86f);
    }else if(kind==4){
     float value=.81f+broad*.13f+grain*.025f;float vein=Mathf.Pow(Mathf.Max(0,Mathf.Sin(u*25.13274f+Mathf.Sin(v*12.56637f)*.6f)),15);
     pixels[y*width+x]=new Color(value*.89f+vein*.06f,value*.95f+vein*.02f,value);
    }else if(kind==5){
     // Shallow turquoise over pale pebbles; interrupted fine caustics read in both
     // the gameplay camera and the isolated menu studio without transparency.
     float bend=Mathf.Sin(v*12.56637f)*.48f+Mathf.Sin(u*6.28318f)*.22f;
     float thread=Mathf.Pow(Mathf.Max(0,Mathf.Cos(u*50.26548f+bend)),18);
     float cross=Mathf.Pow(Mathf.Max(0,Mathf.Cos(v*37.69911f+Mathf.Sin(u*12.56637f)*.7f)),22);
     float light=(thread+cross)*(.22f+.28f*broad);
     pixels[y*width+x]=Color.Lerp(new Color(.035f,.34f,.39f),new Color(.15f,.69f,.66f),.24f+broad*.70f)+new Color(.49f,.61f,.48f)*light;
    }else if(kind==6){
     float strands=.5f+.5f*Mathf.Sin(u*75.39822f+Mathf.Sin(v*12.56637f));
     pixels[y*width+x]=Color.Lerp(new Color(.16f,.27f,.085f),new Color(.48f,.61f,.24f),broad*.7f+strands*.15f+grain*.1f);
    }else{
     float streak=Mathf.Pow(Mathf.Max(0,Mathf.Sin(u*100.53096f+Mathf.Sin(v*12.56637f)*.30f)),8);
     float broken=.5f+.5f*Noise(u*11,v*11,11);float white=streak*broken*.65f;
     pixels[y*width+x]=Color.Lerp(new Color(.23f,.64f,.71f),new Color(.92f,.98f,1),white+.25f+broad*.20f);
    }
   }
   var texture=new Texture2D(width,height,TextureFormat.RGB24,true){name=kind==0?"LandscapeMineralAtlas":kind==1?"LandscapeTurfAtlas":kind==3?"LandscapeSandAtlas":kind==4?"LandscapeIceAtlas":kind==5?"LandscapePoolCaustics":kind==6?"LandscapeMossAtlas":"LandscapeWaterAtlas",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear,anisoLevel=2};
   texture.SetPixels(pixels);texture.Apply(true,true);ArtLifetime.Own(gameObject,texture);return texture;
  }
  private static readonly Color[] Mineral={new(.91f,.91f,.83f),new(.93f,.95f,.89f),new(.93f,.76f,.51f),new(.43f,.49f,.31f),new(.32f,.29f,.46f),new(.58f,.67f,.75f),new(.83f,.55f,.29f),new(.27f,.25f,.29f),new(.87f,.88f,.77f),new(.34f,.38f,.61f)};
  private static readonly Color[] Ground={Color.white,new(.93f,.95f,.89f),new(.93f,.76f,.51f),new(.54f,.77f,.48f),new(.48f,.43f,.62f),new(.97f,.99f,1),new(.96f,.77f,.45f),new(.29f,.27f,.29f),new(.99f,.92f,.69f),new(.49f,.52f,.70f)};
  private static readonly Color[] Leaves={new(.25f,.43f,.16f),new(.33f,.49f,.23f),new(.38f,.34f,.20f),new(.12f,.38f,.20f),new(.25f,.36f,.49f),new(.18f,.35f,.33f),new(.26f,.46f,.26f),new(.24f,.24f,.25f),new(.23f,.49f,.36f),new(.28f,.33f,.56f)};
  private static readonly Color[] Accent={new(.50f,.35f,.16f),new(.45f,.58f,.55f),new(.88f,.63f,.25f),new(.31f,.23f,.12f),new(.47f,.34f,.74f),new(.63f,.85f,.95f),new(.94f,.65f,.34f),new(.31f,.29f,.32f),new(.54f,.36f,.20f),new(.72f,.78f,.93f)};
  private Material Special(int key)
  {
   if(_materials.TryGetValue(key,out var existing))return existing;
   var material=new Material(Resources.Load<Material>("Materials/Surface")){name=key==100?"Landscape1PoolSurface":"Landscape1MossSurface",color=Color.white};
   if(key==100){if(_pool==null)_pool=Texture(5);material.mainTexture=_pool;material.SetFloat("_Glossiness",.52f);}
   else{if(_moss==null)_moss=Texture(6);material.mainTexture=_moss;material.SetFloat("_Glossiness",.025f);}
   ArtLifetime.Own(gameObject,material);_materials.Add(key,material);return material;
  }
  private Material Get(int realm,int kind)
  {
   int key=realm*8+kind;if(_materials.TryGetValue(key,out var existing))return existing;
   var material=new Material(Resources.Load<Material>("Materials/Surface")){name="Landscape"+realm+"Surface"+kind};
   if(kind==0){if(_mineral==null)_mineral=Texture(0);material.mainTexture=_mineral;material.color=Mineral[realm];}
   else if(kind==1){
    material.color=Ground[realm];
    if(realm==0||realm==3){if(_turf==null)_turf=Texture(1);material.mainTexture=_turf;}
    else if(realm==6||realm==8){if(_sand==null)_sand=Texture(3);material.mainTexture=_sand;}
    else{if(_mineral==null)_mineral=Texture(0);material.mainTexture=_mineral;}
   }
   else if(kind==2){if(_water==null)_water=Texture(2);material.mainTexture=_water;material.color=realm==8?new Color(.75f,1,.94f):Color.white;}
   else if(kind==3)material.color=Leaves[realm];
   else if(kind==4)material.color=realm==0?new Color(.46f,.61f,.24f):realm==8?new Color(.93f,.99f,.95f):realm==9?new Color(.86f,.89f,.96f):Color.Lerp(Leaves[realm],new Color(.66f,.80f,.45f),.36f);
   else if(kind==5){material.color=Accent[realm];if(realm==5){if(_ice==null)_ice=Texture(4);material.mainTexture=_ice;}}
   else if(kind==6)material.color=realm==7?new Color(.13f,.12f,.15f):Color.Lerp(Mineral[realm],Color.black,.48f);
   else{
    material.color=realm==7?new Color(1,.31f,.035f):realm==9?new Color(.69f,.83f,1):new Color(.41f,.85f,.96f);
    material.EnableKeyword("_EMISSION");material.SetColor("_EmissionColor",material.color*(realm==7?.85f:.36f));
   }
   material.SetFloat("_Glossiness",kind==2?.44f:realm==4&&kind>=5?.42f:realm==5&&kind==5?.57f:.07f);
   ArtLifetime.Own(gameObject,material);_materials.Add(key,material);return material;
  }
 }
}

using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Original editable models share immutable mesh assets. Per-course palette
 // instances are owned normally; no animated or colliding decorative objects.
 public static class NaturalRealmArt
 {
  public static GameObject Instance(Transform parent,int realm){var library=NaturalRealmLibrary.Load();var root=Object.Instantiate(library.Prefabs[realm],parent,false);root.name="OriginalNaturalDetail";root.GetComponent<NaturalRealmBindings>().Apply(parent);return root;}
  public static void Dress(Transform parent,int realm,int index,float size)
  {
   if(realm==2)return;var root=Instance(parent,realm);var b=NaturalRealmLibrary.Load().Bounds[realm];float edge=size*(realm==1?1.25f:1.07f)+.20f;bool right=index%2==0;
   root.transform.localPosition=new Vector3(right?edge-b.min.x:-edge-b.max.x,IslandArt.Surface+.004f,-size*.30f);
   // A connected stone shoulder supports the whole model outside the landing
   // fan. It overlaps the actual cliff core and adds no walkable collision.
   var center=root.transform.localPosition+b.center;float inner=(right?1:-1)*size*.70f,outer=center.x+(right?1:-1)*(b.extents.x+.12f);
   var shoulder=LandscapeGeometry.Rock(parent,"NaturalSculptedShelf",new Vector3((inner+outer)*.5f,IslandArt.Surface-.60f,center.z),new Vector3(1,1.20f,1),LandscapePalette.Surface(parent,realm),realm*71+index);var supportMesh=shoulder.GetComponent<MeshFilter>().sharedMesh;var supportBounds=supportMesh.bounds;var supportVertices=supportMesh.vertices;float width=Mathf.Abs(outer-inner),depth=Mathf.Max(.56f,b.size.z+.20f);
   for(int i=0;i<supportVertices.Length;i++){var p=supportVertices[i];p.x=(p.x-supportBounds.min.x)/supportBounds.size.x*width-width*.5f;p.z=(p.z-supportBounds.min.z)/supportBounds.size.z*depth-depth*.5f;if(p.y>.40f)p.y=.60f;supportVertices[i]=p;}supportMesh.vertices=supportVertices;supportMesh.RecalculateNormals();supportMesh.RecalculateBounds();LandscapeGeometry.JoinNormals(supportMesh);LandscapeGeometry.StoneUv(shoulder,.35f);
   if(realm==1)MeshArt.Oval(parent,"CascadeLilyBasin",new Vector3(center.x,IslandArt.Surface,center.z),new Vector3(b.size.x+.10f,.02f,b.size.z+.10f),LandscapePalette.Pool(parent),12);
  }
#if UNITY_EDITOR
  public static NaturalRealmBindings Author(Transform parent,int realm,Material[] materials)
  {
   var root=new GameObject("OriginalNaturalRealm"+realm);root.transform.SetParent(parent,false);
   GameObject Block(string n,Vector3 p,Vector3 size,int role,float bevel=.035f){var go=LandscapeGeometry.BeveledBlock(root.transform,n,p,size,materials[role],bevel);LandscapeGeometry.StoneUv(go,.35f);return go;}
   void Branch(string n,Vector3[] path,float[] radii,int role,int sides=7)=>LandscapeBotany.Branch(root.transform,n,path,radii,materials[role],sides);
   void Leaves(string n,Vector3 p,int count,float length,int role,int seed=0){var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();for(int i=0;i<count;i++){float a=i*2.399963f+seed*.41f;var d=new Vector3(Mathf.Cos(a),.20f+Mathf.Sin(i)*.08f,Mathf.Sin(a));var side=new Vector3(-d.z,0,d.x)*length*.17f;var tip=p+d*length*(.75f+i%3*.12f);var mid=Vector3.Lerp(p,tip,.55f)+Vector3.up*.045f;int k=v.Count;var points=new[]{p,mid-side,tip,mid+side,mid};var tex=new[]{Vector2.zero,new Vector2(0,.5f),Vector2.up,Vector2.one*.5f,new Vector2(.5f,.5f)};v.AddRange(points);v.AddRange(points);uv.AddRange(tex);uv.AddRange(tex);for(int edge=0;edge<4;edge++){int next=(edge+1)%4;t.AddRange(new[]{k+edge,k+4,k+next,k+5+next,k+9,k+5+edge});}}MeshArt.Mesh(root.transform,n,v.ToArray(),t.ToArray(),uv.ToArray(),materials[role]);}
   void Blossom(Vector3 p,float radius,int petals,int role){var v=new List<Vector3>();var uv=new List<Vector2>();var t=new List<int>();for(int i=0;i<petals;i++){float a=i*Mathf.PI*2/petals;var d=new Vector3(Mathf.Cos(a),.12f,Mathf.Sin(a));var side=new Vector3(-d.z,0,d.x)*radius*.28f;int k=v.Count;var points=new[]{p,p+d*radius*.55f-side,p+d*radius+Vector3.up*.025f,p+d*radius*.55f+side,p+d*radius*.5f+Vector3.up*.045f};var tex=new[]{Vector2.zero,Vector2.right,Vector2.one,Vector2.up,Vector2.one*.5f};v.AddRange(points);v.AddRange(points);uv.AddRange(tex);uv.AddRange(tex);for(int edge=0;edge<4;edge++){int next=(edge+1)%4;t.AddRange(new[]{k+edge,k+4,k+next,k+5+next,k+9,k+5+edge});}}MeshArt.Mesh(root.transform,"OriginalPetalWhorl",v.ToArray(),t.ToArray(),uv.ToArray(),materials[role]);}
   switch(realm){
    case 0:
     Block("MeadowWeatheredGardenStone",new Vector3(0,.08f,0),new Vector3(.72f,.16f,.48f),0,.07f);
     for(int i=0;i<5;i++){float a=i*2.39996f;var p=new Vector3(Mathf.Cos(a)*.23f,.14f,Mathf.Sin(a)*.16f);var top=p+new Vector3(.04f*Mathf.Sin(i),.22f+i%3*.055f,0);Branch("WildflowerStem",new[]{p,top},new[]{.012f,.006f},3,5);Blossom(top,.115f,6,8);}Leaves("MeadowFoldedFronds",Vector3.up*.16f,8,.42f,4);break;
    case 1:
     // Deliberately small: the already lush waterfall destination has less
     // than829 spare vertices under the existing30k island budget.
     for(int i=0;i<3;i++){var p=new Vector3((i-1)*.18f,.015f,Mathf.Sin(i)*.11f);Leaves("CascadeLilyLeaf",p,3,.17f,3,i);Blossom(p+Vector3.up*.065f,.125f,6,8);}break;
    case 3:
     for(int side=-1;side<=1;side+=2)Branch("OrchidButtressRoot",new[]{new Vector3(side*.46f,.01f,.16f),new Vector3(side*.25f,.18f,0),new Vector3(side*.08f,.65f,-.10f)},new[]{.04f,.10f,.065f},5,8);
     Leaves("JungleOrchidLeaves",Vector3.up*.12f,9,.63f,3);Leaves("OrchidUpperLeaves",new Vector3(0,.40f,-.02f),5,.41f,4,2);
     for(int i=0;i<3;i++){var p=new Vector3((i-1)*.16f,.70f+i%2*.20f,-.07f);Branch("OrchidFlowerStem",new[]{new Vector3(0,.20f,0),p},new[]{.018f,.008f},3);Blossom(p,.19f,5,8);}break;
    case 4:
     Block("GeodeShrineFoot",new Vector3(0,.09f,0),new Vector3(.85f,.18f,.70f),0,.07f);Block("ShrineCutCrown",new Vector3(0,.22f,0),new Vector3(.64f,.13f,.52f),6,.035f);
     for(int i=0;i<5;i++){float a=i*Mathf.PI*2/5;var p=new Vector3(Mathf.Cos(a)*.22f,.27f,Mathf.Sin(a)*.20f);var shard=MeshArt.Lathe(root.transform,"PolishedQuartzPoint",new[]{0f,.09f,.47f,.60f},new[]{.11f,.12f,.075f,.014f},6,materials[i==2?7:5],Vector3.one,true);shard.transform.localPosition=p;shard.transform.localRotation=Quaternion.Euler(Mathf.Sin(a)*14,0,-Mathf.Cos(a)*14);}break;
    case 5:
     Branch("SpruceBarkTrunk",new[]{Vector3.zero,new Vector3(.02f,.8f,0),new Vector3(-.035f,1.75f,.03f)},new[]{.13f,.085f,.022f},6,9);
     for(int tier=0;tier<4;tier++)for(int branch=0;branch<5;branch++){float a=branch*Mathf.PI*2/5+tier*.63f,h=.35f+tier*.33f,length=.67f-tier*.12f;var d=new Vector3(Mathf.Cos(a),-.16f,Mathf.Sin(a));var start=Vector3.up*h;var tip=start+d*length;Branch("SpruceNaturalBough",new[]{start,Vector3.Lerp(start,tip,.55f)+Vector3.up*.045f,tip},new[]{.034f,.025f,.006f},3,5);Leaves("SpruceNeedleSprays",tip-d*length*.18f,9,.29f-tier*.035f,3,branch);if(branch%2==0){var snow=MeshArt.Oval(root.transform,"SpruceSnowPillow",Vector3.Lerp(start,tip,.63f)+Vector3.up*.045f,new Vector3(length*.55f,.06f,length*.30f),materials[1],6);snow.transform.localRotation=Quaternion.Euler(0,-a*Mathf.Rad2Deg,0);}}break;
    case 6:
     Branch("RibbedCactusStem",new[]{Vector3.zero,Vector3.up*.14f,Vector3.up*.92f,Vector3.up*1.07f},new[]{.09f,.14f,.12f,.035f},3,12);
     for(int i=0;i<7;i++){float a=i*Mathf.PI*2/7;var p=new Vector3(Mathf.Cos(a)*.13f,0,Mathf.Sin(a)*.13f);Branch("CactusRaisedRib",new[]{p+Vector3.up*.13f,p+Vector3.up*.74f,p*.55f+Vector3.up*.98f},new[]{.012f,.012f,.006f},4,5);}
     for(int side=-1;side<=1;side+=2)Branch("CactusCurvedArm",new[]{new Vector3(side*.08f,.34f,0),new Vector3(side*.30f,.42f,0),new Vector3(side*.31f,.75f,0)},new[]{.07f,.078f,.03f},3,9);Blossom(Vector3.up*1.06f,.18f,7,8);Leaves("DesertYuccaBlades",new Vector3(.35f,.01f,.13f),9,.43f,3);break;
    case 7:
     for(int i=0;i<6;i++){float a=i*Mathf.PI/3;var vent=MeshArt.Lathe(root.transform,"HexagonalVentBasalt",new[]{0f,.10f,.43f,.47f},new[]{.18f,.19f,.16f,.12f},6,materials[0],Vector3.one,true);vent.transform.localPosition=new Vector3(Mathf.Cos(a)*.27f,0,Mathf.Sin(a)*.27f);}
     var bowl=MeshArt.Lathe(root.transform,"VolcanicVentBowl",new[]{.17f,.22f,.28f},new[]{.25f,.24f,.18f},12,materials[6],Vector3.one);MeshArt.Oval(root.transform,"VentMoltenCore",Vector3.up*.275f,new Vector3(.36f,.035f,.36f),materials[7],12);
     for(int i=0;i<3;i++){var ember=MeshArt.Oval(root.transform,"VentEmberTongue",new Vector3((i-1)*.09f,.37f+i%2*.07f,0),new Vector3(.055f,.19f,.05f),materials[7],6);ember.transform.localRotation=Quaternion.Euler(0,0,i*17-20);}break;
    case 8:
     Block("BeaconStoneFoot",new Vector3(0,.08f,0),new Vector3(.82f,.16f,.70f),0,.055f);var tower=MeshArt.Lathe(root.transform,"CoastalBeaconTower",new[]{.16f,.26f,1.26f,1.34f},new[]{.27f,.25f,.19f,.23f},12,materials[0],Vector3.one);MeshArt.Ring(root.transform,"BeaconCornice",.25f,.04f,1.34f,materials[5],18);
     for(int side=-1;side<=1;side+=2)for(int z=-1;z<=1;z+=2)Branch("BeaconLanternPost",new[]{new Vector3(side*.15f,1.34f,z*.15f),new Vector3(side*.15f,1.73f,z*.15f)},new[]{.016f,.016f},5,5);
     MeshArt.Oval(root.transform,"BeaconWarmLens",Vector3.up*1.52f,new Vector3(.17f,.24f,.17f),materials[7],10);var cap=MeshArt.Lathe(root.transform,"BeaconConicalCopperRoof",new[]{1.73f,1.79f,1.98f},new[]{.31f,.30f,.018f},12,materials[5],Vector3.one);Block("BeaconDoor",new Vector3(0,.45f,-.248f),new Vector3(.12f,.31f,.025f),6,.008f);break;
    case 9:
     Block("AstralCarvedFoot",new Vector3(0,.09f,0),new Vector3(.82f,.18f,.72f),0,.055f);Block("MoonstoneObelisk",new Vector3(0,.68f,0),new Vector3(.27f,1.10f,.27f),5,.045f);
     for(int i=0;i<3;i++){var ring=MeshArt.Ring(root.transform,"CelestialOrbitalInlay",.34f+i*.055f,.014f,0,materials[i==1?7:5],28,310);ring.transform.localPosition=Vector3.up*(.54f+i*.26f);ring.transform.localRotation=Quaternion.Euler(35+i*21,i*43,12);}
     MeshArt.Oval(root.transform,"MoonstoneBeacon",Vector3.up*1.34f,new Vector3(.16f,.24f,.16f),materials[7],8);break;
   }
   var binding=root.AddComponent<NaturalRealmBindings>();binding.Realm=realm;var surfaces=new List<NaturalSurfaceBinding>();foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>()){var mesh=renderer.GetComponent<MeshFilter>().sharedMesh;if(mesh.uv.Length!=mesh.vertexCount)LandscapeGeometry.StoneUv(renderer.gameObject,.35f);int role=System.Array.IndexOf(materials,renderer.sharedMaterial);if(role<0)throw new System.InvalidOperationException("Original model lost its material role");surfaces.Add(new NaturalSurfaceBinding{Renderer=renderer,Role=role});}binding.Surfaces=surfaces.ToArray();return binding;
  }
#endif
 }
}

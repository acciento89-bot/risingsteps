using UnityEngine;

namespace Kamilunavo.RisingSteps.Visuals
{
 public static class RunnerArt
 {
  public static RunnerAnimator Build(Transform parent)
  {
   var root=new GameObject("HoodieRunner",typeof(RunnerAnimator));root.transform.SetParent(parent,false);
   var animator=root.GetComponent<RunnerAnimator>();
   // All mutable materials and the detail texture belong to this avatar.
   var skin=Material(root,"RunnerWarmSkin",new Color(.92f,.68f,.47f),.16f);
   var pants=Material(root,"RunnerCargoTwill",new Color(.34f,.275f,.185f),.08f);
   var ivory=Material(root,"RunnerIvoryCanvas",new Color(.94f,.955f,.90f),.14f);
   var dark=Material(root,"RunnerSeamsAndEyes",new Color(.055f,.065f,.05f),.12f);
   var green=Material(root,"RunnerHoodie",new Color(.055f,.32f,.17f),.10f);
   var hair=Material(root,"RunnerChestnutHair",new Color(.47f,.21f,.075f),.14f);
   var gold=Material(root,"RunnerPremiumThread",new Color(1,.88f,.39f),.22f);
   var canvas=Material(root,"RunnerTravelCanvas",new Color(.54f,.345f,.165f),.12f);canvas.mainTexture=CanvasDetail(root);
   hair.mainTexture=HairDetail(root);var cloth=CanvasDetail(root);cloth.name="RunnerClothWeave";green.mainTexture=cloth;pants.mainTexture=cloth;animator.Hoodie=green;
   animator.Hips=Pivot(root.transform,"Hips",new Vector3(0,.85f,0));

   var hoodie=new RunnerMeshBuilder(animator.Hips,"TailoredHoodie",green);
   hoodie.FabricProfile(Vector3.zero,new[]{.045f,.08f,.13f,.20f,.28f,.36f,.43f,.50f,.545f,.57f},new[]{.205f,.215f,.211f,.204f,.210f,.226f,.237f,.245f,.21f,.155f},new[]{.133f,.142f,.142f,.14f,.147f,.150f,.146f,.135f,.12f,.095f},16,.46f);
   // The front pocket has a flat opening and tapered curved lower corners.
   hoodie.Loft(new[]{new Vector3(0,.15f,.145f),new Vector3(0,.19f,.163f),new Vector3(0,.28f,.168f),new Vector3(0,.305f,.156f)},new[]{new Vector2(.11f,.008f),new Vector2(.153f,.018f),new Vector2(.143f,.018f),new Vector2(.10f,.008f)},12,.62f);
   hoodie.Profile(Vector3.zero,new[]{.04f,.075f,.10f},new[]{.207f,.218f,.211f},new[]{.136f,.146f,.141f},16,.72f);hoodie.Finish();

   var hood=new RunnerMeshBuilder(animator.Hips,"WhiteDrapedHood",green);hood.FoldedHood();
   hood.Sweep(new[]{new Vector3(-.18f,.525f,.015f),new Vector3(-.20f,.56f,-.08f),new Vector3(-.15f,.615f,-.155f),new Vector3(0,.64f,-.17f),new Vector3(.15f,.615f,-.155f),new Vector3(.20f,.56f,-.08f),new Vector3(.18f,.525f,.015f)},new[]{.043f,.063f,.076f,.082f,.076f,.063f,.043f},new[]{.025f,.038f,.043f,.047f,.043f,.038f,.025f},10);
   for(int side=-1;side<=1;side+=2)hood.Sweep(new[]{new Vector3(side*.13f,.52f,-.177f),new Vector3(side*.075f,.47f,-.183f),new Vector3(side*.025f,.433f,-.164f)},new[]{.012f,.011f,.007f},new[]{.008f,.009f,.005f},6);
   hood.Finish();

   var hipFabric=new RunnerMeshBuilder(animator.Hips,"CargoWaist",pants);
   hipFabric.Profile(Vector3.zero,new[]{-.055f,.015f,.10f},new[]{.19f,.218f,.205f},new[]{.113f,.133f,.126f},14,.72f);hipFabric.Finish();
   var seams=new RunnerMeshBuilder(animator.Hips,"HoodieAndWaistSeams",dark);
   seams.Stitch(new Vector3(-.10f,.29f,.181f),new Vector3(-.15f,.22f,.18f),.003f);
   seams.Stitch(new Vector3(.10f,.29f,.181f),new Vector3(.15f,.22f,.18f),.003f);
   seams.Stitch(new Vector3(0,.10f,.129f),new Vector3(0,-.025f,.133f),.003f);
   seams.Profile(Vector3.zero,new[]{-.010f,.022f,.036f},new[]{.217f,.221f,.218f},new[]{.135f,.139f,.136f},16,.72f);
   // Belt fittings share the existing Hips seam batch. The marker preserves
   // the authored feature name without spending a renderer beside premium crests.
   Pivot(animator.Hips,"TravelBeltHardware",Vector3.zero);
   var hardware=seams;
   hardware.Sweep(new[]{new Vector3(-.033f,.006f,.142f),new Vector3(-.033f,.033f,.142f),new Vector3(.033f,.033f,.142f),new Vector3(.033f,.006f,.142f),new Vector3(-.033f,.006f,.142f)},new[]{.005f,.005f,.005f,.005f,.005f},new[]{.003f,.003f,.003f,.003f,.003f},5);
   for(int side=-1;side<=1;side+=2){hardware.Stitch(new Vector3(side*.060f,.365f,-.429f),new Vector3(side*.10f,.365f,-.429f),.005f);hardware.Stitch(new Vector3(side*.080f,.352f,-.43f),new Vector3(side*.080f,.384f,-.43f),.004f);}
   BuildPack(animator,canvas,seams);seams.Finish();

   for(int side=-1;side<=1;side+=2){BuildLeg(animator,side,pants,skin,canvas,ivory,green,dark);BuildArm(animator,side,green,skin,dark);}
   BuildHead(animator,skin,ivory,dark,hair);
   for(int style=4;style<8;style++){
    var crest=Pivot(animator.Hips,"PremiumCrest"+style,new Vector3(0,.285f,-.422f));
    var thread=new RunnerMeshBuilder(crest,"EmbroideredThread",gold);int rays=style==5?4:style==7?5:3;
    for(int k=0;k<rays;k++){float angle=(k*360f/rays+style*9)*Mathf.Deg2Rad;thread.Stitch(new Vector3(Mathf.Sin(angle)*.022f,Mathf.Cos(angle)*.022f,-.006f),new Vector3(Mathf.Sin(angle)*.083f,Mathf.Cos(angle)*.083f,-.009f),.005f);}
    thread.Finish();crest.gameObject.SetActive(false);
   }
   animator.CaptureRestPose();return animator;
  }

  static void BuildLeg(RunnerAnimator animator,int side,Material pants,Material skin,Material canvas,Material ivory,Material green,Material dark)
  {
   var leg=Pivot(animator.Hips,side<0?"LeftLeg":"RightLeg",new Vector3(side*.119f,.005f,side*.018f));
   var knee=Pivot(leg,side<0?"LeftKnee":"RightKnee",new Vector3(0,-.35f,.012f));
   if(side<0){animator.LeftLeg=leg;animator.LeftKnee=knee;}else{animator.RightLeg=leg;animator.RightKnee=knee;}
   var thigh=new RunnerMeshBuilder(leg,"CargoThigh",pants);
   thigh.FabricProfile(Vector3.zero,new[]{-.315f,-.29f,-.235f,-.16f,-.09f,-.025f,.03f},new[]{.100f,.110f,.116f,.12f,.119f,.124f,.10f},new[]{.099f,.110f,.109f,.110f,.109f,.109f,.083f},12,side*.71f);
   thigh.Loft(new[]{new Vector3(side*.109f,-.265f,.006f),new Vector3(side*.125f,-.24f,.01f),new Vector3(side*.128f,-.13f,.01f),new Vector3(side*.112f,-.105f,.007f)},new[]{new Vector2(.013f,.05f),new Vector2(.022f,.071f),new Vector2(.023f,.072f),new Vector2(.009f,.055f)},8,.6f);
   // A thick folded hem ends above the knee, rather than a long trouser tube.
   thigh.Profile(new Vector3(0,0,.007f),new[]{-.327f,-.310f,-.281f},new[]{.106f,.116f,.110f},new[]{.105f,.119f,.110f},12,.69f);thigh.Finish();
   var pocket=new RunnerMeshBuilder(leg,"CargoPocketFlap",dark);
   pocket.Stitch(new Vector3(side*.151f,-.137f,-.05f),new Vector3(side*.151f,-.137f,.067f),.004f);pocket.Stitch(new Vector3(side*.114f,-.30f,.067f),new Vector3(side*.112f,-.21f,.10f),.003f);
   pocket.Sweep(new[]{new Vector3(side*.151f,-.135f,-.051f),new Vector3(side*.154f,-.155f,-.032f),new Vector3(side*.155f,-.163f,.031f),new Vector3(side*.151f,-.14f,.069f)},new[]{.004f,.004f,.004f,.004f},new[]{.002f,.002f,.002f,.002f},5);pocket.Finish();
   var shin=new RunnerMeshBuilder(knee,"LowerLegSkin",skin);
   shin.Profile(new Vector3(0,0,-.012f),new[]{-.30f,-.245f,-.12f,.035f,.067f},new[]{.060f,.065f,.076f,.088f,.077f},new[]{.061f,.068f,.075f,.081f,.069f},12,.85f);shin.Finish();
   var footOrigin=new Vector3(0,-.36f,-.007f);
   var boot=new RunnerMeshBuilder(knee,"HikingBootLast",canvas);
   boot.Shoe(footOrigin,-.12f,.014f,.010f);
   boot.Loft(new[]{footOrigin+new Vector3(0,.010f,.123f),footOrigin+new Vector3(0,.040f,.061f),footOrigin+new Vector3(0,.088f,-.012f),footOrigin+new Vector3(0,.142f,-.061f)},new[]{new Vector2(.043f,.005f),new Vector2(.047f,.009f),new Vector2(.045f,.009f),new Vector2(.04f,.007f)},8,.72f);
   boot.Profile(footOrigin+new Vector3(0,0,-.065f),new[]{-.050f,.030f,.114f,.146f},new[]{.077f,.088f,.085f,.076f},new[]{.060f,.070f,.068f,.060f},12,.72f);
   boot.Profile(footOrigin+new Vector3(0,0,-.065f),new[]{.123f,.144f,.159f},new[]{.083f,.090f,.080f},new[]{.064f,.073f,.060f},12,.70f);boot.Finish();
   var trim=new RunnerMeshBuilder(knee,"HikingBootLacesAndSole",dark);
   trim.Shoe(footOrigin,-.13f,-.090f,0);
   for(int lace=0;lace<4;lace++){float z=-.039f+lace*.033f,y=.124f-lace*.026f;trim.Stitch(footOrigin+new Vector3(-.043f,y,z),footOrigin+new Vector3(.043f,y-.017f,z+.025f),.004f);trim.Stitch(footOrigin+new Vector3(.043f,y,z),footOrigin+new Vector3(-.043f,y-.017f,z+.025f),.004f);}trim.Finish();
   var tread=new RunnerMeshBuilder(knee,"BootTreadAndAnkleTrim",dark);
   tread.Profile(footOrigin+new Vector3(0,0,-.065f),new[]{.139f,.153f,.170f},new[]{.087f,.088f,.078f},new[]{.069f,.069f,.062f},12,.72f);
   for(int i=0;i<4;i++)tread.Stitch(footOrigin+new Vector3(-.084f,-.134f,-.070f+i*.080f),footOrigin+new Vector3(.084f,-.134f,-.070f+i*.080f),.006f);
   for(int sideEdge=-1;sideEdge<=1;sideEdge+=2){tread.Stitch(footOrigin+new Vector3(sideEdge*.098f,-.064f,-.07f),footOrigin+new Vector3(sideEdge*.103f,-.061f,.09f),.004f);}tread.Finish();
   leg.localRotation=Quaternion.Euler(side<0?-3:4,0,side*2);knee.localRotation=Quaternion.Euler(side<0?6:2,0,0);

  }

  static void BuildArm(RunnerAnimator animator,int side,Material green,Material skin,Material dark)
  {
   var arm=Pivot(animator.Hips,side<0?"LeftArm":"RightArm",new Vector3(side*.246f,.505f,0));arm.localRotation=Quaternion.Euler(0,0,side*11);
   var elbow=Pivot(arm,side<0?"LeftElbow":"RightElbow",new Vector3(0,-.265f,.008f));elbow.localRotation=Quaternion.Euler(-12,0,0);
   if(side<0){animator.LeftArm=arm;animator.LeftElbow=elbow;}else{animator.RightArm=arm;animator.RightElbow=elbow;}
   var sleeve=new RunnerMeshBuilder(arm,"TailoredRolledSleeve",green);
   sleeve.FabricProfile(Vector3.zero,new[]{-.28f,-.245f,-.20f,-.135f,-.07f,.025f,.07f},new[]{.080f,.086f,.089f,.101f,.11f,.095f,.045f},new[]{.080f,.088f,.091f,.098f,.106f,.091f,.045f},12,side*.33f);
   sleeve.Profile(Vector3.zero,new[]{-.285f,-.255f,-.233f},new[]{.085f,.096f,.092f},new[]{.085f,.096f,.092f},12,.72f);sleeve.Finish();
   var limb=new RunnerMeshBuilder(elbow,"ForearmAndHand",skin);
   limb.Profile(Vector3.zero,new[]{-.245f,-.20f,-.10f,.01f},new[]{.048f,.055f,.068f,.072f},new[]{.047f,.052f,.067f,.072f},12);
   limb.Loft(new[]{new Vector3(0,-.34f,.036f),new Vector3(0,-.322f,.021f),new Vector3(0,-.28f,.015f),new Vector3(0,-.245f,.003f)},new[]{new Vector2(.039f,.022f),new Vector2(.053f,.030f),new Vector2(.058f,.039f),new Vector2(.046f,.035f)},10,.72f);
   // Thumb and curled finger ridges share the hand mesh and no collider.
   limb.Sweep(new[]{new Vector3(-side*.046f,-.268f,.024f),new Vector3(-side*.070f,-.289f,.05f),new Vector3(-side*.064f,-.319f,.058f)},new[]{.021f,.023f,.013f},new[]{.020f,.018f,.011f},7);
   for(int finger=0;finger<3;finger++){float x=-.028f+finger*.027f;limb.Sweep(new[]{new Vector3(x,-.304f,.044f),new Vector3(x,-.329f,.056f),new Vector3(x,-.344f,.035f)},new[]{.013f,.014f,.010f},new[]{.011f,.012f,.009f},6);}limb.Finish();
   var band=new RunnerMeshBuilder(elbow,"WristBand",dark);band.Profile(Vector3.zero,new[]{-.245f,-.214f},new[]{.053f,.055f},new[]{.052f,.054f},12);band.Finish();
  }

  static void BuildHead(RunnerAnimator animator,Material skin,Material ivory,Material dark,Material hair)
  {
   animator.Head=Pivot(animator.Hips,"Head",new Vector3(0,.81f,.003f));animator.Head.localRotation=Quaternion.Euler(-2,-5,0);
   var neck=new RunnerMeshBuilder(animator.Hips,"Neck",skin);neck.Profile(new Vector3(0,0,.008f),new[]{.56f,.65f,.68f},new[]{.071f,.068f,.073f},new[]{.071f,.068f,.073f},16);neck.Finish();
   var face=new RunnerMeshBuilder(animator.Head,"FaceSkin",skin);
   face.Profile(new Vector3(0,0,.015f),new[]{-.215f,-.195f,-.175f,-.13f,-.085f,-.0275f,.03f,.075f,.12f,.1575f,.195f,.21f,.225f},new[]{.085f,.11f,.132f,.165f,.181f,.192f,.196f,.195f,.19f,.178f,.151f,.121f,.085f},new[]{.066f,.085f,.105f,.127f,.140f,.152f,.157f,.154f,.145f,.133f,.115f,.09f,.06f},24,.9f);face.Finish();
   var features=new RunnerMeshBuilder(animator.Head,"FaceFeatures",skin);
   features.Pebble(new Vector3(0,-.047f,.181f),new Vector3(.057f,.082f,.052f),12);
   for(int side=-1;side<=1;side+=2)features.Pebble(new Vector3(side*.196f,-.028f,-.005f),new Vector3(.067f,.108f,.070f),10);features.Finish();
   var eyes=new RunnerMeshBuilder(animator.Head,"EyeWhites",ivory);var expression=new RunnerMeshBuilder(animator.Head,"EyesAndExpression",dark);
   for(int side=-1;side<=1;side+=2){eyes.Pebble(new Vector3(side*.079f,.021f,.164f),new Vector3(.069f,.080f,.022f),10);expression.Pebble(new Vector3(side*.079f,.018f,.178f),new Vector3(.031f,.049f,.012f),8);expression.Sweep(new[]{new Vector3(side*.043f,.081f,.164f),new Vector3(side*.074f,.089f,.163f),new Vector3(side*.109f,.080f,.153f)},new[]{.008f,.011f,.006f},new[]{.004f,.005f,.003f},6);}
   expression.Sweep(new[]{new Vector3(-.042f,-.128f,.145f),new Vector3(0,-.136f,.153f),new Vector3(.043f,-.128f,.145f)},new[]{.003f,.004f,.003f},new[]{.002f,.002f,.002f},5);eyes.Finish();expression.Finish();
   var crown=new RunnerMeshBuilder(animator.Head,"SweptChestnutHair",hair);crown.HairCrown();
   // Broad swept ridges grow from one crown; locks rake back rather than
   // forming a radial collection of identical cone spikes.
   for(int lockIndex=0;lockIndex<6;lockIndex++){
    float x=-.17f+lockIndex*.068f,y=.195f+(1-Mathf.Abs(x)/.18f)*.065f;
    crown.Sweep(new[]{new Vector3(x,y,.044f),new Vector3(x+.022f,y+.075f,-.013f),new Vector3(x+.042f,y+.084f+(lockIndex%3)*.011f,-.092f)},new[]{.061f,.045f,.007f},new[]{.041f,.027f,.005f},7);
   }
   for(int fringe=0;fringe<5;fringe++){
    float x=-.125f+fringe*.056f;
    crown.Sweep(new[]{new Vector3(x-.025f,.18f,.108f),new Vector3(x,.145f,.143f),new Vector3(x+.030f,.092f+(fringe%2)*.020f,.159f)},new[]{.042f,.032f,.004f},new[]{.024f,.018f,.003f},7);
   }
   crown.Finish();
   // Broad overlapping locks wrap over the occiput. Their tapered tips change
   // the rear contour seen during play, where tiny top ridges were insufficient.
   var rear=new RunnerMeshBuilder(animator.Head,"RearSweptLocks",hair);
   // Two overlapping rows of short uneven tapered tufts, instead of long
   // parallel rib-shaped locks. Every tuft stays in the same head mesh/joint.
   for(int row=0;row<2;row++)for(int lockIndex=0;lockIndex<5;lockIndex++){
    float x=-.16f+lockIndex*.081f,arch=1-Mathf.Abs(x)/.205f,y=(row==0?.22f:.125f)+arch*.062f+.015f*Mathf.Sin(lockIndex*2.31f+row);
    float sweep=.026f*Mathf.Sin(lockIndex*1.7f+row*2);
    rear.Sweep(new[]{new Vector3(x-.022f,y+.016f,-.118f-row*.028f),new Vector3(x+sweep,y+.035f,-.193f-row*.022f),new Vector3(x+.042f+sweep,y-.082f-(lockIndex%2)*.016f,-.231f-row*.025f)},new[]{.075f,.050f,.004f},new[]{.035f,.033f,.003f},7);
   }
   for(int side=-1;side<=1;side+=2)for(int lockIndex=0;lockIndex<3;lockIndex++){
    float y=.115f+lockIndex*.048f;
    rear.Sweep(new[]{new Vector3(side*.158f,y+.072f,.038f),new Vector3(side*.218f,y+.031f,-.048f),new Vector3(side*.235f,y-.018f,-.145f),new Vector3(side*.210f,y-.079f,-.196f)},new[]{.035f,.042f,.031f,.004f},new[]{.024f,.030f,.023f,.003f},6);
   }
   rear.Finish();
  }

  static void BuildPack(RunnerAnimator animator,Material canvas,RunnerMeshBuilder seams)
  {
   var pack=Pivot(animator.Hips,"AdventurerPack",new Vector3(0,.103f,-.150f));
   var body=new RunnerMeshBuilder(pack,"CanvasPackBody",canvas);
   body.Loft(new[]{new Vector3(0,.04f,-.075f),new Vector3(0,.095f,-.106f),new Vector3(0,.19f,-.120f),new Vector3(0,.31f,-.118f),new Vector3(0,.445f,-.092f),new Vector3(0,.49f,-.068f)},new[]{new Vector2(.13f,.070f),new Vector2(.18f,.090f),new Vector2(.177f,.095f),new Vector2(.18f,.095f),new Vector2(.159f,.077f),new Vector2(.118f,.053f)},14,.82f);
   // Curved flap, front pouch and two side pockets share a single rigid buffer.
   body.Loft(new[]{new Vector3(0,.29f,-.204f),new Vector3(0,.325f,-.226f),new Vector3(0,.375f,-.211f),new Vector3(0,.454f,-.170f),new Vector3(0,.485f,-.090f)},new[]{new Vector2(.117f,.018f),new Vector2(.149f,.019f),new Vector2(.172f,.031f),new Vector2(.166f,.040f),new Vector2(.112f,.047f)},12,.86f);
   body.Loft(new[]{new Vector3(0,.07f,-.177f),new Vector3(0,.10f,-.217f),new Vector3(0,.225f,-.222f),new Vector3(0,.26f,-.184f)},new[]{new Vector2(.104f,.027f),new Vector2(.135f,.038f),new Vector2(.129f,.037f),new Vector2(.109f,.018f)},10,.64f);
   for(int side=-1;side<=1;side+=2)body.Loft(new[]{new Vector3(side*.186f,.085f,-.096f),new Vector3(side*.201f,.125f,-.103f),new Vector3(side*.200f,.275f,-.106f),new Vector3(side*.175f,.302f,-.093f)},new[]{new Vector2(.033f,.052f),new Vector2(.045f,.073f),new Vector2(.045f,.071f),new Vector2(.020f,.052f)},10,.68f);
   // Rolled leather flap edge, hand-shaped gussets and twin closure straps
   // share the pack mesh; they never become separate decorative draw calls.
   body.Sweep(new[]{new Vector3(-.118f,.29f,-.228f),new Vector3(-.153f,.325f,-.246f),new Vector3(-.168f,.375f,-.234f),new Vector3(-.159f,.443f,-.204f)},new[]{.006f,.007f,.007f,.005f},new[]{.004f,.004f,.004f,.004f},6);
   body.Sweep(new[]{new Vector3(.118f,.29f,-.228f),new Vector3(.153f,.325f,-.246f),new Vector3(.168f,.375f,-.234f),new Vector3(.159f,.443f,-.204f)},new[]{.006f,.007f,.007f,.005f},new[]{.004f,.004f,.004f,.004f},6);
   for(int side=-1;side<=1;side+=2){
    body.Sweep(new[]{new Vector3(side*.08f,.095f,-.255f),new Vector3(side*.08f,.20f,-.263f),new Vector3(side*.08f,.295f,-.229f),new Vector3(side*.08f,.36f,-.241f)},new[]{.011f,.013f,.012f,.010f},new[]{.004f,.005f,.005f,.004f},6);
    body.Sweep(new[]{new Vector3(side*.206f,.114f,-.153f),new Vector3(side*.223f,.18f,-.164f),new Vector3(side*.206f,.27f,-.15f)},new[]{.005f,.005f,.004f},new[]{.004f,.004f,.003f},5);
   }
   body.Finish();Pivot(pack,"PackPouches",Vector3.zero);
   var straps=new RunnerMeshBuilder(animator.Hips,"PackShoulderStraps",canvas);
   for(int side=-1;side<=1;side+=2){
    straps.Sweep(new[]{new Vector3(side*.16f,.12f,.113f),new Vector3(side*.174f,.34f,.142f),new Vector3(side*.183f,.53f,.101f),new Vector3(side*.177f,.592f,-.008f),new Vector3(side*.172f,.51f,-.162f),new Vector3(side*.152f,.32f,-.256f)},new[]{.017f,.021f,.024f,.025f,.023f,.019f},new[]{.008f,.010f,.011f,.012f,.010f,.008f},8);
    seams.Sweep(new[]{new Vector3(side*.080f,.18f,-.416f),new Vector3(side*.080f,.32f,-.416f),new Vector3(side*.080f,.43f,-.398f),new Vector3(side*.080f,.49f,-.372f)},new[]{.007f,.007f,.007f,.007f},new[]{.004f,.004f,.004f,.004f},6);
    seams.Stitch(new Vector3(side*.057f,.366f,-.426f),new Vector3(side*.103f,.366f,-.426f),.006f);
    seams.Stitch(new Vector3(side*.057f,.394f,-.416f),new Vector3(side*.103f,.394f,-.416f),.005f);
   }
   straps.Sweep(new[]{new Vector3(-.065f,.555f,-.243f),new Vector3(-.057f,.635f,-.232f),new Vector3(.057f,.635f,-.232f),new Vector3(.065f,.555f,-.243f)},new[]{.009f,.009f,.009f,.009f},new[]{.009f,.009f,.009f,.009f},7);straps.Finish();
  }
  static Texture2D CanvasDetail(GameObject owner)
  {
   const int size=64;var pixels=new Color[size*size];for(int y=0;y<size;y++)for(int x=0;x<size;x++){float weave=((x+y)%2==0?.045f:-.045f)+Mathf.Sin(x*.61f+y*.31f)*.018f;pixels[y*size+x]=new Color(.88f+weave,.88f+weave,.88f+weave);}
   var texture=new Texture2D(size,size,TextureFormat.RGB24,true){name="RunnerCanvasWeave",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear};texture.SetPixels(pixels);texture.Apply(true,true);ArtLifetime.Own(owner,texture);return texture;
  }
  static Material Material(GameObject owner,string name,Color color,float gloss)
  {var material=new Material(Resources.Load<Material>("Materials/Surface")){name=name,color=color};material.SetFloat("_Glossiness",gloss);ArtLifetime.Own(owner,material);return material;}
  static Texture2D HairDetail(GameObject owner)
  {
   var original=Resources.Load<Texture2D>("Art/HairFlow-v1");
   if(original==null||!original.isReadable)throw new System.InvalidOperationException("Missing readable original hair artwork");
   var texture=new Texture2D(original.width,original.height,TextureFormat.RGB24,true){name="RunnerHairStrandDetail",wrapMode=TextureWrapMode.Repeat,filterMode=FilterMode.Bilinear};texture.SetPixels(original.GetPixels());texture.Apply(true,true);ArtLifetime.Own(owner,texture);return texture;
  }
  static Transform Pivot(Transform parent,string name,Vector3 at)
  {var transform=new GameObject(name).transform;transform.SetParent(parent,false);transform.localPosition=at;return transform;}
 }
}

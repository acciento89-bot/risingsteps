using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.Gameplay
{
    public sealed class RisingCourse:MonoBehaviour
    {
        public Transform Player;public Text HeightText;public Text CrystalsText;public Image Progress;public Text Toast;public Renderer PlayerRenderer;
        private readonly List<StepMarker> _steps=new();private int _height;private int _crystals;private Vector3 _safe;private int _style;
        private static readonly Color Rock=new(.32f,.30f,.39f);private static readonly Color Grass=new(.28f,.58f,.25f);private static readonly Color Gold=new(1f,.80f,.20f);

        public void Build(){Random.InitState(260906);var x=0f;var y=0f;var z=0f;for(var i=0;i<12;i++){if(i>0){x=Mathf.Clamp(x+Random.Range(-2f,2f),-5f,5f);y+=Random.Range(.75f,1.05f);z+=Random.Range(4.2f,5f);}var root=new GameObject($"Island_{i+1:00}");root.transform.SetParent(transform,false);root.transform.position=new Vector3(x,y,z);var rock=GameObject.CreatePrimitive(PrimitiveType.Cylinder);rock.transform.SetParent(root.transform,false);rock.transform.localScale=new Vector3(2.6f,.55f,2.6f);rock.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=Rock};var marker=rock.AddComponent<StepMarker>();marker.Index=i;_steps.Add(marker);var grass=GameObject.CreatePrimitive(PrimitiveType.Cylinder);grass.name="GrassTop";grass.transform.SetParent(root.transform,false);grass.transform.localPosition=new Vector3(0,.62f,0);grass.transform.localScale=new Vector3(2.7f,.10f,2.7f);Destroy(grass.GetComponent<Collider>());grass.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=Grass};var glow=GameObject.CreatePrimitive(PrimitiveType.Cube);glow.name="RouteGlow";glow.transform.SetParent(root.transform,false);glow.transform.localPosition=new Vector3(0,.78f,.3f);glow.transform.localScale=new Vector3(2.2f,.05f,1.4f);Destroy(glow.GetComponent<Collider>());glow.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=Gold};}Decorate();_safe=_steps[0].transform.position+Vector3.up*1.8f;Player.position=_safe;Refresh();}

        private void Decorate(){for(var i=0;i<7;i++){var island=GameObject.CreatePrimitive(PrimitiveType.Sphere);island.name="ScenicIsland";island.transform.position=new Vector3((i%2==0?-1:1)*(10+i*1.7f),Random.Range(0,12),12+i*9);island.transform.localScale=new Vector3(5,2.2f,5);island.GetComponent<Renderer>().material=new Material(Shader.Find("Standard")){color=Rock};Destroy(island.GetComponent<Collider>());}}

        private void Update(){if(_steps.Count==0||Player==null)return;var y=_steps[Mathf.Clamp(_height,0,_steps.Count-1)].transform.position.y;if(Player.position.y<y-8)Respawn();}

        public void Land(StepMarker step){if(step.Index<=_height||step.Index>_height+1)return;_height=step.Index;_crystals+=10+_height*2;_safe=step.transform.position+Vector3.up*1.8f;if(Toast!=null)Toast.text=_height==11?"PORTAL READY":"STEP CLEARED";Refresh();}

        public void Respawn(){var cc=Player.GetComponent<CharacterController>();if(cc!=null)cc.enabled=false;Player.position=_safe;if(cc!=null)cc.enabled=true;if(Toast!=null)Toast.text="READY";}

        public void ClaimDaily(){_crystals+=100;if(Toast!=null)Toast.text="+100 DAILY";Refresh();}
        public void CycleStyle(){_style=(_style+1)%3;if(PlayerRenderer!=null){var colors=new[]{new Color(.12f,.46f,.18f),new Color(.10f,.38f,.80f),new Color(.95f,.62f,.08f)};PlayerRenderer.material.color=colors[_style];}if(Toast!=null)Toast.text=$"STYLE {_style+1}";}

        private void Refresh(){HeightText.text=$"HEIGHT {_height} / 12";CrystalsText.text=$"◇  {_crystals}";if(Progress!=null)Progress.fillAmount=_height/12f;}
    }
}

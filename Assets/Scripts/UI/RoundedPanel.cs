using UnityEngine;
using UnityEngine.UI;
namespace Kamilunavo.RisingSteps.UI
{
 public sealed class RoundedPanel:Image
 {
  private static Material _panel;protected override void Awake(){base.Awake();if(_panel==null)_panel=new Material(Shader.Find("Rising/Panel"));material=_panel;}
  protected override void OnPopulateMesh(VertexHelper vh){base.OnPopulateMesh(vh);var rect=rectTransform.rect;var v=new UIVertex();for(int i=0;i<vh.currentVertCount;i++){vh.PopulateUIVertex(ref v,i);v.uv1=new Vector4((v.position.x-rect.xMin)/Mathf.Max(1,rect.width),(v.position.y-rect.yMin)/Mathf.Max(1,rect.height),rect.width,rect.height);vh.SetUIVertex(v,i);}}
 }
 public sealed class CanvasOrientation:MonoBehaviour
 {
  private int _w,_h;private void Update(){if(_w==Screen.width&&_h==Screen.height)return;_w=Screen.width;_h=Screen.height;GetComponent<CanvasScaler>().referenceResolution=_w>_h?new Vector2(844,390):new Vector2(390,844);}
 }
}

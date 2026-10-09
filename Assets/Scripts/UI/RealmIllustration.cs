using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.UI
{
    // Cropped chapter photography uses the existing authored fantasy assets.
    // The original textures remain intact; UV windows preserve the subject's aspect.
    public sealed class RealmIllustration : RawImage
    {
        public int Realm;
        public bool Hero;
        private Rect _subject;
        protected override void OnPopulateMesh(VertexHelper mesh)
        {
            base.OnPopulateMesh(mesh);RoundedPanel.SetGalleryCoordinates(mesh,rectTransform.rect);
        }
        protected override void OnRectTransformDimensionsChange(){base.OnRectTransformDimensionsChange();if(texture!=null)Crop();}
        private void Configure()
        {
            texture=Resources.Load<Texture2D>(Hero||Realm==2?"Art/RisingStepsIcon":"Art/SkyPanorama");
            // Meadow and waterfall use different horizon islands. Temple is a close
            // crop of the actual golden destination rather than a repeated diagram.
            _subject=Hero?new Rect(0,0,1,1):Realm==0?new Rect(.18f,.06f,.29f,.61f):Realm==1?new Rect(.74f,.04f,.26f,.72f):new Rect(.57f,.50f,.43f,.50f);
            color=Color.white;material=RoundedPanel.GalleryMaterial;raycastTarget=false;Crop();
        }
        private void Crop()
        {
            var crop=_subject;float target=rectTransform.rect.width/Mathf.Max(1,rectTransform.rect.height);float aspect=texture.width*crop.width/(texture.height*crop.height);
            if(aspect>target){float width=crop.width*target/aspect;crop.x+=(crop.width-width)*.5f;crop.width=width;}
            else{float height=crop.height*aspect/target;crop.y+=(crop.height-height)*.5f;crop.height=height;}
            uvRect=crop;
        }
        public static RealmIllustration Add(Transform parent,int realm,Vector2 min,Vector2 max,bool hero=false)
        {
            var go=new GameObject("RealmArt",typeof(RectTransform),typeof(CanvasRenderer),typeof(RealmIllustration));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;var art=go.GetComponent<RealmIllustration>();art.Realm=realm;art.Hero=hero;art.Configure();return art;
        }
    }
}

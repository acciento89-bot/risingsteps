using UnityEngine;
using UnityEngine.UI;

namespace Kamilunavo.RisingSteps.UI
{
    // Original chapter illustrations and atlas artwork are menu assets only.
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
            texture=Resources.Load<Texture2D>(Hero?"Art/AtlasHero-v3":Realm==0?"Art/MeadowChapter-v2":Realm==1?"Art/CascadeChapter-v2":Realm==2?"Art/TempleChapter-v2":"Art/RealmChapterAtlas-v1");
            // Meadow and waterfall use different horizon islands. Temple is a close
            // crop of the actual golden destination rather than a repeated diagram.
            _subject=Realm==1&&!Hero?new Rect(.02f,0,.98f,1):new Rect(0,0,1,1);
            if(Realm>=3&&!Hero){int panel=Realm-3;_subject=new Rect(panel%2*.5f+.003f,.75f-panel/2*.25f+.003f,.494f,.244f);}
            color=Color.white;material=RoundedPanel.GalleryMaterial;raycastTarget=false;Crop();
        }
        private void Crop()
        {
            var crop=_subject;float target=rectTransform.rect.width/Mathf.Max(1,rectTransform.rect.height);float aspect=texture.width*crop.width/(texture.height*crop.height);
            if(aspect>target){float width=crop.width*target/aspect;crop.x+=(crop.width-width)*.5f;crop.width=width;}
            else{float height=crop.height*aspect/target;crop.y+=(crop.height-height)*.5f;crop.height=height;}
            uvRect=crop;
        }
        public static Graphic Add(Transform parent,int realm,Vector2 min,Vector2 max,bool hero=false)
        {
            var go=new GameObject("RealmArt",typeof(RectTransform),typeof(CanvasRenderer),typeof(RealmIllustration));go.transform.SetParent(parent,false);var rect=(RectTransform)go.transform;rect.anchorMin=min;rect.anchorMax=max;rect.offsetMin=rect.offsetMax=Vector2.zero;var art=go.GetComponent<RealmIllustration>();art.Realm=Mathf.Clamp(realm,0,9);art.Hero=hero;art.Configure();return art;
        }
    }
}

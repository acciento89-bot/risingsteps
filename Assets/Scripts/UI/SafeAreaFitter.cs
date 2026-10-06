using UnityEngine;

namespace Kamilunavo.RisingSteps.UI
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class SafeAreaFitter:MonoBehaviour
    {
        private RectTransform _r;private Rect _last;
        private void Awake(){_r=GetComponent<RectTransform>();Apply();}
        private void Update(){if(Screen.safeArea!=_last)Apply();}
        private void Apply(){_last=Screen.safeArea;var min=_last.position;var max=_last.position+_last.size;min.x/=Screen.width;min.y/=Screen.height;max.x/=Screen.width;max.y/=Screen.height;_r.anchorMin=min;_r.anchorMax=max;_r.offsetMin=Vector2.zero;_r.offsetMax=Vector2.zero;}
    }
}

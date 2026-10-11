using UnityEngine;
namespace Kamilunavo.RisingSteps.UI
{
 // The gallery uses the same stable realm/local-level identity as the campaign authority.
 public sealed class RealmRouteTag:MonoBehaviour { public int Realm,Level;public int Id=>Realm*10+Level; }
}

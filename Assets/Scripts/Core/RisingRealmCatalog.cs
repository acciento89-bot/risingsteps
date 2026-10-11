namespace Kamilunavo.RisingSteps.Core
{
 public enum RisingRealmTheme { Meadow,Waterfall,Temple,Jungle,Crystal,Frost,Desert,Volcano,CloudCoast,Starlit }
 // Stable IDs preserve the original three ten-level slots; descriptions are presentation only.
 public static class RisingRealmCatalog
 {
  public const int Count=10;
  static readonly string[] De={"Wiesenhimmel","Wasserfall-Reich","Tempelpfad","Dschungelhain","Kristallgrotten","Frostgipfel","Dünenmeer","Vulkanpfad","Wolkenküste","Sternenreise"};
  static readonly string[] En={"Meadow Sky","Waterfall Realm","Temple Trail","Jungle Grove","Crystal Caverns","Frost Peaks","Dune Sea","Volcano Trail","Cloud Coast","Starlit Voyage"};
  static readonly string[] DetailDe={"Blühende Wiesen · Heller Himmel","Klare Becken · Rauschende Fälle","Alte Ruinen · Goldene Wege","Hohe Baumkronen · Verborgene Wege","Leuchtende Kristalle · Tiefe Grotten","Schneekämme · Stille Wälder","Sanfte Dünen · Ferne Oasen","Basaltklippen · Glühende Flüsse","Weiße Wolken · Türkise Buchten","Sternenbrücken · Mondlicht"};
  static readonly string[] DetailEn={"Flowering meadows · Open blue skies","Clear pools · Cascading waterfalls","Ancient ruins · Golden paths","High canopies · Hidden trails","Luminous crystals · Deep caverns","Snowy ridges · Quiet forests","Soft dunes · Distant oases","Basalt cliffs · Glowing rivers","White clouds · Turquoise bays","Star bridges · Moonlight"};
  static int Index(int realm)=>realm<0?0:realm>=Count?Count-1:realm;
  public static RisingRealmTheme Theme(int realm)=>(RisingRealmTheme)Index(realm);
  public static string Name(int realm,bool german)=> (german?De:En)[Index(realm)];
  public static string Description(int realm,bool german)=> (german?DetailDe:DetailEn)[Index(realm)];
 }
}

public static class RoomData
{   
    // The enumerated type for what types of rooms there are
    public enum RoomType
    {
        Command,
        Engineering,
        Operations,
        Science,
        Weapons
    }

    // The enumerated type for availalbe tile types for each given room
    public enum RoomTiles
    {
        Empty,
        Floor,
        Area,
        Wall,
        Star,
    }

    // A set of double arryas for all of the basic rooms needed to create ships

    // all cannon orientations
    // TODO: do we need all orientations, or can we do it programmatically
    public static RoomTiles[,] CannonUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] CannonRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] CannonDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] CannonLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // cargobay
    public static RoomTiles[,] CargoBay = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    // all cloaking device orientations
    public static RoomTiles[,] CloakUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] CloakRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] CloakDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] CloakLeft = {
         {RoomTiles.Empty,  RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
         {RoomTiles.Wall,   RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
         {RoomTiles.Wall,   RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
         {RoomTiles.Wall,   RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
         {RoomTiles.Wall,   RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
         {RoomTiles.Wall,   RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
         {RoomTiles.Empty,  RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
     };

    // all engine orientations
    public static RoomTiles[,] EngineUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] EngineRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] EngineDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] EngineLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all helm orientations
    public static RoomTiles[,] HelmUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] HelmRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] HelmDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] HelmLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all hyperdrive orientations
    public static RoomTiles[,] HyperdriveUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] HyperdriveRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] HyperdriveDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] HyperdriveLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    // life support
    public static RoomTiles[,] LifeSupport = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    // all minelayer orientations
    public static RoomTiles[,] MineLayerUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] MineLayerRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] MineLayerDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] MineLayerLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all missile bay orientations
    public static RoomTiles[,] MissileBayUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] MissileBayRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty ,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] MissileBayDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };
    public static RoomTiles[,] MissileBayLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all science bay orientations
    public static RoomTiles[,] ScienceUp =  {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] ScienceRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] ScienceDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] ScienceLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all SickBay orientations
    public static RoomTiles[,] SickBayUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] SickBayDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all teleporter orientations
    public static RoomTiles[,] TeleporterUp =  {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] TeleporterDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // put together the room information classes for use in ship building
    public static RoomInfo CannonUpInfo         = new RoomInfo("Cannon Up",         RoomType.Weapons,       CannonUp);
    public static RoomInfo CannonRightInfo      = new RoomInfo("Cannon Right",      RoomType.Weapons,       CannonRight);
    public static RoomInfo CannonDownInfo       = new RoomInfo("Cannon Down",       RoomType.Weapons,       CannonDown);
    public static RoomInfo CannonLeftInfo       = new RoomInfo("Cannon Left",       RoomType.Weapons,       CannonLeft);

    public static RoomInfo CargoBayInfo         = new RoomInfo("Cargo Bay Up",      RoomType.Operations,    CargoBay);

    public static RoomInfo CloakUpInfo          = new RoomInfo("Cloak Up",          RoomType.Science,       CloakUp);
    public static RoomInfo CloakRightInfo       = new RoomInfo("Cloak Right",       RoomType.Science,       CloakRight);
    public static RoomInfo CloakDownInfo        = new RoomInfo("Cloak Down",        RoomType.Science,       CloakDown);
    public static RoomInfo CloakLeftInfo        = new RoomInfo("Cloak Left",        RoomType.Science,       CloakLeft);

    public static RoomInfo EngineUpInfo         = new RoomInfo("Engine Up",         RoomType.Engineering,   EngineUp);
    public static RoomInfo EngineRightInfo      = new RoomInfo("Engine Right",      RoomType.Engineering,   EngineRight);
    public static RoomInfo EngineDownInfo       = new RoomInfo("Engine Down",       RoomType.Engineering,   EngineDown);
    public static RoomInfo EngineLeftInfo       = new RoomInfo("Engine Left",       RoomType.Engineering,   EngineLeft);

    public static RoomInfo HelmUpInfo           = new RoomInfo("Helm Up",           RoomType.Command,       HelmUp);
    public static RoomInfo HelmRightInfo        = new RoomInfo("Helm Right",        RoomType.Command,       HelmRight);
    public static RoomInfo HelmDownInfo         = new RoomInfo("Helm Down",         RoomType.Command,       HelmDown);
    public static RoomInfo HelmLeftInfo         = new RoomInfo("Helm Left",         RoomType.Command,       HelmLeft);

    public static RoomInfo HyperdriveUpInfo     = new RoomInfo("Hyperdrive Up",     RoomType.Science,       HyperdriveUp);
    public static RoomInfo HyperdriveRightInfo  = new RoomInfo("Hyperdrive Right",  RoomType.Science,       HyperdriveRight);
    public static RoomInfo HyperdriveDownInfo   = new RoomInfo("Hyperdrive Down",   RoomType.Science,       HyperdriveDown);
    public static RoomInfo HyperdriveLeftInfo   = new RoomInfo("Hyperdrive Left",   RoomType.Science,       HyperdriveLeft);

    public static RoomInfo LifeSupportInfo      = new RoomInfo("Life Support Up",   RoomType.Operations,    LifeSupport);

    public static RoomInfo MineLayerUpInfo      = new RoomInfo("Mine Layer Up",     RoomType.Engineering,   MineLayerUp);
    public static RoomInfo MineLayerRightInfo   = new RoomInfo("Mine Layer Right",  RoomType.Engineering,   MineLayerRight);
    public static RoomInfo MineLayerDownInfo    = new RoomInfo("Mine Layer Down",   RoomType.Engineering,   MineLayerDown);
    public static RoomInfo MineLayerLeftInfo    = new RoomInfo("Mine Layer Left",   RoomType.Engineering,   MineLayerLeft);

    public static RoomInfo MissileBayUpInfo     = new RoomInfo("Missile Bay Up",    RoomType.Weapons,       MissileBayUp);
    public static RoomInfo MissileBayRightInfo  = new RoomInfo("Missile Bay Right", RoomType.Weapons,       MissileBayRight);
    public static RoomInfo MissileBayDownInfo   = new RoomInfo("Missile Bay Down",  RoomType.Weapons,       MissileBayDown);
    public static RoomInfo MissileBayLeftInfo   = new RoomInfo("Missile Bay Left",  RoomType.Weapons,       MissileBayLeft);

    public static RoomInfo ScienceUpInfo        = new RoomInfo("Science Up",        RoomType.Science,       ScienceUp);
    public static RoomInfo ScienceRightInfo     = new RoomInfo("Science Right",     RoomType.Science,       ScienceRight);
    public static RoomInfo ScienceDownInfo      = new RoomInfo("Science Down",      RoomType.Science,       ScienceDown);
    public static RoomInfo ScienceLeftInfo      = new RoomInfo("Science Left",      RoomType.Science,       ScienceLeft);

    public static RoomInfo SickBayUpInfo        = new RoomInfo("Sick Bay Up",       RoomType.Science,       SickBayUp);
    public static RoomInfo SickBayDownInfo      = new RoomInfo("Sick Bay Down",     RoomType.Science,       SickBayDown);

    public static RoomInfo TeleporterUpInfo     = new RoomInfo("Teleporter Up",     RoomType.Science,       TeleporterUp);
    public static RoomInfo TeleporterDownInfo   = new RoomInfo("Teleporter Down",   RoomType.Science,       TeleporterDown);

}

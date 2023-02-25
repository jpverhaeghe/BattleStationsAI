
public static class RoomData
{   
    // The enumerated type for what types of rooms there are (for color coding)
    public enum RoomType
    {
        Command,
        Engineering,
        Operations,
        Science,
        Weapons
    }

    public enum RoomFacing
    {
        Up,
        Right,
        Down,
        Left
    }

    public enum ModuleType
    {
        Cannon,
        CargoBay,
        Cloak,
        Hyperdrive,
        LifeSupport,
        MineLayer,
        MissileBay,
        Science,
        SickBay,
        Teleporter,
        Engine,                 // placed here so it can be ignored for normal module placement
        Helm                    // placed here so it can be ignored for normal module placement
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

    // Room constant values to be used by other classes - like for exit locations
    public static int ROOM_TILES_LAST_INDEX = 6;
    public static int EXIT_POS = 3;

    // A set of double arryas for all of the basic rooms needed to create ships

    // all cannon orientations - External facing
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

    // all cloaking device orientations - External facing
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
         {RoomTiles.Empty,  RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
     };

    // all engine orientations - External facing - special case placed last
    public static RoomTiles[,] EngineUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] EngineRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] EngineDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty}
    };

    public static RoomTiles[,] EngineLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all helm orientations - External facing - special case placed first
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

    // all minelayer orientations - External facing
    public static RoomTiles[,] MineLayerUp = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] MineLayerRight = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] MineLayerDown = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    public static RoomTiles[,] MineLayerLeft = {
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Empty},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Star,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Wall},
        {RoomTiles.Wall,    RoomTiles.Area,     RoomTiles.Area,     RoomTiles.Floor,    RoomTiles.Floor,    RoomTiles.Area,     RoomTiles.Wall},
        {RoomTiles.Empty,   RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty,    RoomTiles.Wall,     RoomTiles.Wall,     RoomTiles.Empty},
    };

    // all missile bay orientations - External facing
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

    // all science bay orientations - External facing
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
    // TODO: May be able to remove facings of rooms that are not external facing as they will be generated up....though other craft may have them differently...
    public static RoomInfo CannonUpInfo         = new RoomInfo("Cannon Up",         RoomType.Weapons,       RoomFacing.Up,          ModuleType.Cannon,      CannonUp,           true);
    public static RoomInfo CannonRightInfo      = new RoomInfo("Cannon Right",      RoomType.Weapons,       RoomFacing.Right,       ModuleType.Cannon,      CannonRight,        true);
    public static RoomInfo CannonDownInfo       = new RoomInfo("Cannon Down",       RoomType.Weapons,       RoomFacing.Down,        ModuleType.Cannon,      CannonDown,         true);
    public static RoomInfo CannonLeftInfo       = new RoomInfo("Cannon Left",       RoomType.Weapons,       RoomFacing.Left,        ModuleType.Cannon,      CannonLeft,         true);

    public static RoomInfo CargoBayInfo         = new RoomInfo("Cargo Bay Up",      RoomType.Operations,    RoomFacing.Up,          ModuleType.CargoBay,    CargoBay,           false);

    public static RoomInfo CloakUpInfo          = new RoomInfo("Cloak Up",          RoomType.Science,       RoomFacing.Up,          ModuleType.Cloak,       CloakUp,            true);
    public static RoomInfo CloakRightInfo       = new RoomInfo("Cloak Right",       RoomType.Science,       RoomFacing.Right,       ModuleType.Cloak,       CloakRight,         true);
    public static RoomInfo CloakDownInfo        = new RoomInfo("Cloak Down",        RoomType.Science,       RoomFacing.Down,        ModuleType.Cloak,       CloakDown,          true);
    public static RoomInfo CloakLeftInfo        = new RoomInfo("Cloak Left",        RoomType.Science,       RoomFacing.Left,        ModuleType.Cloak,       CloakLeft,          true);

    public static RoomInfo HyperdriveUpInfo     = new RoomInfo("Hyperdrive Up",     RoomType.Science,       RoomFacing.Up,          ModuleType.Hyperdrive,  HyperdriveUp,       false);
    public static RoomInfo HyperdriveRightInfo  = new RoomInfo("Hyperdrive Right",  RoomType.Science,       RoomFacing.Right,       ModuleType.Hyperdrive,  HyperdriveRight,    false);
    public static RoomInfo HyperdriveDownInfo   = new RoomInfo("Hyperdrive Down",   RoomType.Science,       RoomFacing.Down,        ModuleType.Hyperdrive,  HyperdriveDown,     false);
    public static RoomInfo HyperdriveLeftInfo   = new RoomInfo("Hyperdrive Left",   RoomType.Science,       RoomFacing.Left,        ModuleType.Hyperdrive,  HyperdriveLeft,     false);

    public static RoomInfo LifeSupportInfo      = new RoomInfo("Life Support Up",   RoomType.Operations,    RoomFacing.Up,          ModuleType.LifeSupport, LifeSupport,        false);

    public static RoomInfo MineLayerUpInfo      = new RoomInfo("Mine Layer Up",     RoomType.Engineering,   RoomFacing.Up,          ModuleType.MineLayer,   MineLayerUp,        true);
    public static RoomInfo MineLayerRightInfo   = new RoomInfo("Mine Layer Right",  RoomType.Engineering,   RoomFacing.Right,       ModuleType.MineLayer,   MineLayerRight,     true);
    public static RoomInfo MineLayerDownInfo    = new RoomInfo("Mine Layer Down",   RoomType.Engineering,   RoomFacing.Down,        ModuleType.MineLayer,   MineLayerDown,      true);
    public static RoomInfo MineLayerLeftInfo    = new RoomInfo("Mine Layer Left",   RoomType.Engineering,   RoomFacing.Left,        ModuleType.MineLayer,   MineLayerLeft,      true);

    public static RoomInfo MissileBayUpInfo     = new RoomInfo("Missile Bay Up",    RoomType.Weapons,       RoomFacing.Up,          ModuleType.MissileBay,  MissileBayUp,       true);
    public static RoomInfo MissileBayRightInfo  = new RoomInfo("Missile Bay Right", RoomType.Weapons,       RoomFacing.Right,       ModuleType.MissileBay,  MissileBayRight,    true);
    public static RoomInfo MissileBayDownInfo   = new RoomInfo("Missile Bay Down",  RoomType.Weapons,       RoomFacing.Down,        ModuleType.MissileBay,  MissileBayDown,     true);
    public static RoomInfo MissileBayLeftInfo   = new RoomInfo("Missile Bay Left",  RoomType.Weapons,       RoomFacing.Left,        ModuleType.MissileBay,  MissileBayLeft,     true);

    public static RoomInfo ScienceUpInfo        = new RoomInfo("Science Up",        RoomType.Science,       RoomFacing.Up,          ModuleType.Science,     ScienceUp,          true);
    public static RoomInfo ScienceRightInfo     = new RoomInfo("Science Right",     RoomType.Science,       RoomFacing.Right,       ModuleType.Science,     ScienceRight,       true);
    public static RoomInfo ScienceDownInfo      = new RoomInfo("Science Down",      RoomType.Science,       RoomFacing.Down,        ModuleType.Science,     ScienceDown,        true);
    public static RoomInfo ScienceLeftInfo      = new RoomInfo("Science Left",      RoomType.Science,       RoomFacing.Left,        ModuleType.Science,     ScienceLeft,        true);

    public static RoomInfo SickBayUpInfo        = new RoomInfo("Sick Bay Up",       RoomType.Science,       RoomFacing.Up,          ModuleType.SickBay,     SickBayUp,          false);
    public static RoomInfo SickBayDownInfo      = new RoomInfo("Sick Bay Down",     RoomType.Science,       RoomFacing.Down,        ModuleType.SickBay,     SickBayDown,        false);

    public static RoomInfo TeleporterUpInfo     = new RoomInfo("Teleporter Up",     RoomType.Science,       RoomFacing.Up,          ModuleType.Teleporter,  TeleporterUp,       false);
    public static RoomInfo TeleporterDownInfo   = new RoomInfo("Teleporter Down",   RoomType.Science,       RoomFacing.Down,        ModuleType.Teleporter,  TeleporterDown,     false);

    public static RoomInfo EngineUpInfo         = new RoomInfo("Engine Up",         RoomType.Engineering,   RoomFacing.Up,          ModuleType.Engine,      EngineUp,           true);
    public static RoomInfo EngineRightInfo      = new RoomInfo("Engine Right",      RoomType.Engineering,   RoomFacing.Right,       ModuleType.Engine,      EngineRight,        true);
    public static RoomInfo EngineDownInfo       = new RoomInfo("Engine Down",       RoomType.Engineering,   RoomFacing.Down,        ModuleType.Engine,      EngineDown,         true);
    public static RoomInfo EngineLeftInfo       = new RoomInfo("Engine Left",       RoomType.Engineering,   RoomFacing.Left,        ModuleType.Engine,      EngineLeft,         true);

    public static RoomInfo HelmUpInfo           = new RoomInfo("Helm Up",           RoomType.Command,       RoomFacing.Up,          ModuleType.Helm,        HelmUp,             true);
    public static RoomInfo HelmRightInfo        = new RoomInfo("Helm Right",        RoomType.Command,       RoomFacing.Right,       ModuleType.Helm,        HelmRight,          true);
    public static RoomInfo HelmDownInfo         = new RoomInfo("Helm Down",         RoomType.Command,       RoomFacing.Down,        ModuleType.Helm,        HelmDown,           true);
    public static RoomInfo HelmLeftInfo         = new RoomInfo("Helm Left",         RoomType.Command,       RoomFacing.Left,        ModuleType.Helm,        HelmLeft,           true);

    // a list of all module rooms for creating modules (not including helm as they are placed only once)
    // duplicating single and double rooms so we can just randomize a facing
    public static RoomInfo[][] roomsByModules =
    {
         new RoomInfo[] {   CannonUpInfo,       CannonRightInfo,        CannonDownInfo,         CannonLeftInfo},
         new RoomInfo[] {   CargoBayInfo,       CargoBayInfo,           CargoBayInfo,           CargoBayInfo},
         new RoomInfo[] {   CloakUpInfo,        CloakRightInfo,         CloakDownInfo,          CloakLeftInfo},
         new RoomInfo[] {   HyperdriveUpInfo,   HyperdriveRightInfo,    HyperdriveDownInfo,     HyperdriveLeftInfo},
         new RoomInfo[] {   LifeSupportInfo,    LifeSupportInfo,        LifeSupportInfo,        LifeSupportInfo},
         new RoomInfo[] {   MineLayerUpInfo,    MineLayerRightInfo,     MineLayerDownInfo,      MineLayerLeftInfo},
         new RoomInfo[] {   MissileBayUpInfo,   MissileBayRightInfo,    MissileBayDownInfo,     MissileBayLeftInfo},
         new RoomInfo[] {   ScienceUpInfo,      ScienceRightInfo,       ScienceDownInfo,        ScienceLeftInfo},
         new RoomInfo[] {   SickBayUpInfo,      SickBayUpInfo,          SickBayDownInfo,        SickBayDownInfo},
         new RoomInfo[] {   TeleporterUpInfo,   TeleporterUpInfo,       TeleporterDownInfo,     TeleporterDownInfo},
         new RoomInfo[] {   EngineUpInfo,       EngineRightInfo,        EngineDownInfo,         EngineLeftInfo},
    };

}

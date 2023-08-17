

/// <summary>
/// An enumeration of pooled object names. 
/// Make sure the enum values mirror prefab paths.
/// i.e. all prefabs that will be loaded as terrain impacts must be in the folder
/// "Resources/Prefabs/Effects/Impacts/Terrain" and the enum value has to 
/// match this path after Prefabs/ , with / chars replaced with _
/// </summary>
public enum PooledObjectType 
{
    Effects_Impacts_Terrain,      
    Effects_Impacts_Solid,    // so this loads every prefab in "Resources/Prefabs/Effects/Impacts/Solid" folder
    Effects_Impacts_Missile,
    Effects_Impacts_MiniRocket,
    Effects_Explosions_Vehicle
}

import { TwoWayBinding } from "utils/bidirectionalBinding";


// Tool options (bitflags)
export enum ToolOptions {
    None = 0,
    // SubLanes - Boundaries
    NoBoundaryFence = 1 << 0,
    NoBoundaryHedge = 1 << 1,
    NoBoundaryAll = NoBoundaryFence | NoBoundaryHedge,
    // SubLanes - Markings
    NoMarkingLane = 1 << 2,
    NoMarkingAll = NoMarkingLane,
    // SubAreas
    NoSurfaceGrass = 1 << 3,
    NoSurfacePavement = 1 << 4,
    NoSurfaceAll = NoSurfaceGrass | NoSurfacePavement,
    // SubObjects - Vegetation
    NoVegetationTree = 1 << 5,
    NoVegetationShrub = 1 << 6,
    NoVegetationAll = NoVegetationTree | NoVegetationShrub,
    // SubObjects - Elements
    NoElementParking = 1 << 7,
    NoElementLights = 1 << 8,
    NoElementAll = NoElementParking | NoElementLights,
    // Random seed
    FixedRandomSeed = 1 << 9,
    // Meta
    All = NoBoundaryAll | NoMarkingAll | NoSurfaceAll | NoVegetationAll | NoElementAll | FixedRandomSeed,
}

export const GAME_BINDINGS = {
    ENABLE_TOOL_BUTTONS: new TwoWayBinding<boolean>("ENABLE_TOOL_BUTTONS", false),
    TOOL_OPTIONS: new TwoWayBinding<number>("TOOL_OPTIONS", ToolOptions.None),
};

import { TwoWayBinding } from "utils/bidirectionalBinding";


// Tool options (bitflags)
export enum ToolOptions {
    None = 0,
    NoFence = 1 >> 0,
    NoSurfaceGrass = 1 << 1,
    NoSurfacePavement = 1 << 2,
    NoSurface = 1 << 3,
    NoVegetation = 1 << 4,
    FixedRandomSeed = 1 << 5,
    NoHedge = 1 << 6,
    All = NoFence | NoSurfaceGrass | NoSurface | NoSurfacePavement | NoVegetation | FixedRandomSeed | NoHedge,
}

export const GAME_BINDINGS = {
    ENABLE_TOOL_BUTTONS: new TwoWayBinding<boolean>("ENABLE_TOOL_BUTTONS", false),
    TOOL_OPTIONS: new TwoWayBinding<number>("TOOL_OPTIONS", ToolOptions.None),
};

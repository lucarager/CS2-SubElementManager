import { TwoWayBinding } from "utils/bidirectionalBinding";

export const GAME_BINDINGS = {
    PANEL_OPEN: new TwoWayBinding<boolean>("PANEL_OPEN", false),
};

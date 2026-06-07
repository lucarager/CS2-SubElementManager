import { ToolOptionsPanel } from "components/toolOptionsPanel";
import { ModRegistrar } from "cs2/modding";
import { initialize } from "vanilla/Components";

const register: ModRegistrar = (moduleRegistry) => {
    // Resolve the shared base set of vanilla components/themes/focus.
    initialize(
        moduleRegistry,
        [],
        [
            {
                path: "game-ui/game/components/tool-options/tool-options-panel.module.scss",
                name: "toolOptionsPanel",
            },
        ],
    );

    moduleRegistry.extend(
        "game-ui/game/components/tool-options/tool-options-panel.tsx",
        "ToolOptionsPanel",
        ToolOptionsPanel,
    );
};

export default register;

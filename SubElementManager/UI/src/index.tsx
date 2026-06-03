import { ModRegistrar } from "cs2/modding";
import { initialize } from "vanilla/Components";

const register: ModRegistrar = (moduleRegistry) => {
    // Resolve the shared base set of vanilla components/themes/focus.
    initialize(moduleRegistry);
};

export default register;

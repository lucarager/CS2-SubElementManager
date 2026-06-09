import React, { useEffect, useRef, useState } from "react";
import { ModuleRegistryExtend } from "cs2/modding";
import { GAME_BINDINGS, ToolOptions } from "gameBindings";
import styles from "./toolOptionsPanel.module.scss";
import { c } from "utils/classes";
import { FocusDisabled } from "cs2/input";
import { useValue } from "cs2/api";
import { useLocalization } from "cs2/l10n";
import { VC, VF, VT } from "vanilla/Components";
import { Button } from "cs2/ui";

export const ToolOptionsPanel: ModuleRegistryExtend = (Component: any) => {
    const ToolOptionsPanelComponentWrapper = (props: any) => {
        const enabledBinding = useValue(
            GAME_BINDINGS.ENABLE_TOOL_BUTTONS.binding,
        );

        return (
            <>
                {enabledBinding && <ToolPanel />}
                <Component {...props} />
            </>
        );
    };

    return ToolOptionsPanelComponentWrapper;
};

let memoryState = false;

function usePersistentToggleState(): [boolean, React.Dispatch<React.SetStateAction<boolean>>] {
    const [value, setValue] = useState(false);

    useEffect(() => {
        memoryState = value;
    }, [value]);

    return [value, setValue];
}

const ToolPanel = function ToolPanel() {
    const [enabled, setEnabled] = usePersistentToggleState();
    const { translate } = useLocalization();

    return (
        <FocusDisabled>
            <div className={styles.wrapper}>
                <div
                    className={c(
                        VT.toolOptionsPanel.toolOptionsPanel,
                        styles.moddedSection,
                    )}
                >
                    <div className={styles.sectionHeader}>
                        <span className={styles.sectionTitle}>
                            {translate("SubElementManager.UI.ToolOptions.Title")}
                        </span>
                        <Button
                            variant="icon"
                            src={enabled ? "coui://uil/Standard/ArrowUpThickStroke.svg" : "coui://uil/Standard/ArrowDownThickStroke.svg"}
                            onSelect={() => {
                                setEnabled(!enabled);
                            }}
                        />
                    </div>
                    {enabled && (
                        <div className={styles.sectionContent}>
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.FixedRandomSeed"
                                group={ToolOptionGroups.FixedRandomSeed}
                            />
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.Decorations"
                                group={ToolOptionGroups.Decorations}
                            />
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.Markings"
                                group={ToolOptionGroups.Markings}
                            />
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.Surface"
                                group={ToolOptionGroups.Surfaces}
                            />
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.Vegetation"
                                group={ToolOptionGroups.Vegetation}
                            />
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.Elements"
                                group={ToolOptionGroups.Elements}
                            />
                        </div>
                    )}
                </div>
            </div>
        </FocusDisabled>
    );
};

enum ToolOptionGroups {
    Decorations,
    Markings,
    Surfaces,
    Vegetation,
    Elements,
    FixedRandomSeed,
}

/** Human-readable metadata for each flag. */
const AVAILABLE_TOOL_OPTIONS: {
    flag: ToolOptions;
    localeKey: string;
    icon: string;
    group: ToolOptionGroups;
}[] = [
    {
        flag: ToolOptions.NoBoundaryFence,
        localeKey: "SubElementManager.UI.ToolOptions.NoBoundaryFence",
        icon: "coui://sem/ToolOptions/NoBoundaryFence.svg",
        group: ToolOptionGroups.Decorations,
    },
    {
        flag: ToolOptions.NoBoundaryHedge,
        localeKey: "SubElementManager.UI.ToolOptions.NoBoundaryHedge",
        icon: "coui://sem/ToolOptions/NoBoundaryHedge.svg",
        group: ToolOptionGroups.Decorations,
    },
    {
        flag: ToolOptions.NoMarkingLane,
        localeKey: "SubElementManager.UI.ToolOptions.NoMarkingLane",
        icon: "coui://sem/ToolOptions/NoMarkingLane.svg",
        group: ToolOptionGroups.Markings,
    },
    {
        flag: ToolOptions.NoSurfaceGrass,
        localeKey: "SubElementManager.UI.ToolOptions.NoSurfaceGrass",
        icon: "coui://sem/ToolOptions/NoSurfaceGrass.svg",
        group: ToolOptionGroups.Surfaces,
    },
    {
        flag: ToolOptions.NoSurfacePavement,
        localeKey: "SubElementManager.UI.ToolOptions.NoSurfacePavement",
        icon: "coui://sem/ToolOptions/NoSurfacePavement.svg",
        group: ToolOptionGroups.Surfaces,
    },
    {
        flag: ToolOptions.NoVegetationTree,
        localeKey: "SubElementManager.UI.ToolOptions.NoVegetationTree",
        icon: "coui://sem/ToolOptions/NoVegetationTree.svg",
        group: ToolOptionGroups.Vegetation,
    },
    {
        flag: ToolOptions.NoVegetationShrub,
        localeKey: "SubElementManager.UI.ToolOptions.NoVegetationShrub",
        icon: "coui://sem/ToolOptions/NoVegetationShrub.svg",
        group: ToolOptionGroups.Vegetation,
    },
    {
        flag: ToolOptions.NoElementParking,
        localeKey: "SubElementManager.UI.ToolOptions.NoElementParking",
        icon: "coui://sem/ToolOptions/NoElementParking.svg",
        group: ToolOptionGroups.Elements,
    },
    {
        flag: ToolOptions.NoElementLights,
        localeKey: "SubElementManager.UI.ToolOptions.NoElementLights",
        icon: "coui://sem/ToolOptions/NoElementLights.svg",
        group: ToolOptionGroups.Elements,
    },
    {
        flag: ToolOptions.FixedRandomSeed,
        localeKey: "SubElementManager.UI.ToolOptions.FixedRandomSeed",
        icon: "coui://sem/ToolOptions/FixedRandomSeed.svg",
        group: ToolOptionGroups.FixedRandomSeed,
    },
];

export const ToolRow: React.FC<{ label: string; group: ToolOptionGroups }> = ({
    label,
    group,
}) => {
    const { translate } = useLocalization();
    const selected = useValue(GAME_BINDINGS.TOOL_OPTIONS.binding);
    const available = AVAILABLE_TOOL_OPTIONS.filter(
        (option) => option.group === group,
    );
    const showAllButton = available.length > 1;

    const setSelected = (value: number) => {
        GAME_BINDINGS.TOOL_OPTIONS.set(value);
    };

    const groupMask = available.reduce((acc, option) => acc | option.flag, 0);
    const allInGroupSelected = (selected & groupMask) === groupMask;

    const hasFlag = (flag: ToolOptions): boolean => (selected & flag) !== 0;

    const toggleFlag = (flag: ToolOptions) => {
        setSelected(selected ^ flag);
    };

    const handleToggleAll = () => {
        setSelected(allInGroupSelected ? (selected & ~groupMask) : (selected | groupMask));
    };

    return (
        <VC.Section
            focusKey={VF.FOCUS_DISABLED}
            title={translate(label, group.toString())}
        >
            <>
                {showAllButton && (
                    <VC.ToolButton
                        className={c(VT.toolButton.button, styles.iconButton)}
                        src={"coui://sem/ToolOptions/All.svg"}
                        onSelect={handleToggleAll}
                        selected={allInGroupSelected}
                        multiSelect={true}
                        disabled={false}
                        focusKey={VF.FOCUS_DISABLED}
                        tooltip={translate(
                            "SubElementManager.UI.ToolOptions.ToggleAll",
                            "Toggle All",
                        )}
                    />
                )}
                {available.map((toolOption) => (
                    <VC.ToolButton
                        key={toolOption.flag}
                        className={c(VT.toolButton.button, styles.iconButton)}
                        src={toolOption.icon}
                        onSelect={() => toggleFlag(toolOption.flag)}
                        selected={hasFlag(toolOption.flag)}
                        multiSelect={true}
                        disabled={false}
                        focusKey={VF.FOCUS_DISABLED}
                        tooltip={translate(
                            toolOption.localeKey,
                            toolOption.localeKey,
                        )}
                    />
                ))}
            </>
        </VC.Section>
    );
};

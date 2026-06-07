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
                                label="SubElementManager.UI.ToolOptions.Surface"
                                group={ToolOptionGroups.Surfaces}
                            />
                            <ToolRow
                                label="SubElementManager.UI.ToolOptions.Vegetation"
                                group={ToolOptionGroups.Vegetation}
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
    Surfaces,
    Vegetation,
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
        flag: ToolOptions.NoFence,
        localeKey: "SubElementManager.UI.ToolOptions.NoFence",
        icon: "coui://sem/ToolOptions/NoFence.svg",
        group: ToolOptionGroups.Decorations,
    },
    {
        flag: ToolOptions.NoHedge,
        localeKey: "SubElementManager.UI.ToolOptions.NoHedge",
        icon: "coui://sem/ToolOptions/NoHedge.svg",
        group: ToolOptionGroups.Decorations,
    },
    {
        flag: ToolOptions.NoSurfaceGrass,
        localeKey: "SubElementManager.UI.ToolOptions.NoSurfaceGrass",
        icon: "coui://sem/ToolOptions/NoGrass.svg",
        group: ToolOptionGroups.Surfaces,
    },
    {
        flag: ToolOptions.NoSurfacePavement,
        localeKey: "SubElementManager.UI.ToolOptions.NoSurfacePavement",
        icon: "coui://sem/ToolOptions/NoPavement.svg",
        group: ToolOptionGroups.Surfaces,
    },
    {
        flag: ToolOptions.FixedRandomSeed,
        localeKey: "SubElementManager.UI.ToolOptions.FixedRandomSeed",
        icon: "coui://sem/ToolOptions/FixedRandomSeed.svg",
        group: ToolOptionGroups.FixedRandomSeed,
    },
    {
        flag: ToolOptions.NoVegetation,
        localeKey: "SubElementManager.UI.ToolOptions.NoVegetation",
        icon: "coui://sem/ToolOptions/NoVegetation.svg",
        group: ToolOptionGroups.Vegetation,
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

    const allSelected = selected === ToolOptions.All;

    const hasFlag = (flag: ToolOptions): boolean => (selected & flag) !== 0;

    const toggleFlag = (flag: ToolOptions) => {
        setSelected(selected ^ flag);
    };

    const handleToggleAll = () => {
        setSelected(allSelected ? ToolOptions.None : ToolOptions.All);
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
                        selected={allSelected}
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

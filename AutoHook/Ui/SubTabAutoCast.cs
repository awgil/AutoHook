﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AutoHook.Classes;
using AutoHook.Classes.AutoCasts;
using AutoHook.Configurations;
using AutoHook.Resources.Localization;
using AutoHook.Utils;
using Dalamud.Interface.Colors;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ImGuiNET;

namespace AutoHook.Ui;

public class SubTabAutoCast
{
    public bool IsDefaultPreset { get; set; }

    private List<BaseActionCast> actionsAvailable = new();
    public void DrawAutoCastTab(AutoCastsConfig acCfg)
    {
        actionsAvailable = new()
        {
            acCfg.CastLine,
            acCfg.CastMooch,
            acCfg.CastChum,
            acCfg.CastCollect,
            acCfg.CastCordial,
            acCfg.CastFishEyes,
            //acCfg.CastIdenticalCast,
            acCfg.CastMakeShiftBait,
            acCfg.CastPatience,
            acCfg.CastPrizeCatch,
            //acCfg.CastReleaseFish,
            //acCfg.CastSurfaceSlap,
            acCfg.CastThaliaksFavor,
        };
        
        DrawHeader(acCfg);
        DrawBody(acCfg);
    }

    private void DrawHeader(AutoCastsConfig acCfg)
    {
        ImGui.Spacing();

        if (DrawUtil.Checkbox(UIStrings.Enable_Auto_Casts, ref acCfg.EnableAll))
        {
            if (acCfg.EnableAll)
            {
                if (IsDefaultPreset && (Service.Configuration.HookPresets.SelectedPreset?.AutoCastsCfg.EnableAll ?? false))
                {
                    Service.Configuration.HookPresets.SelectedPreset.AutoCastsCfg.EnableAll = false;
                }
                else if (!IsDefaultPreset)
                {
                    Service.Configuration.HookPresets.DefaultPreset.AutoCastsCfg.EnableAll = false;
                }
            }
            Service.Save();
        }

        if (acCfg.EnableAll)
        {
            ImGui.SameLine();
            if (DrawUtil.Checkbox(UIStrings.Dont_Cancel_Mooch, ref acCfg.DontCancelMooch,
                    UIStrings.TabAutoCasts_DrawHeader_HelpText))
            {
                foreach (var action in actionsAvailable.Where(action => action != null))
                {
                    action.DontCancelMooch = acCfg.DontCancelMooch;

                    Service.PrintDebug($"{action.Name} DontCancelMooch: {action.DontCancelMooch}");
                }

                Service.Save();
            }
            
        }

        if (!IsDefaultPreset)
        {
            if (Service.Configuration.HookPresets.DefaultPreset.AutoCastsCfg.EnableAll && !acCfg.EnableAll)
                ImGui.TextColored(ImGuiColors.DalamudViolet, UIStrings.Default_AutoCast_Being_Used);
            else if (!acCfg.EnableAll)
                ImGui.TextColored(ImGuiColors.ParsedBlue, UIStrings.SubAuto_Disabled);
        }
        else
        {
            if (Service.Configuration.HookPresets.SelectedPreset?.AutoCastsCfg.EnableAll ?? false)
                ImGui.TextColored(ImGuiColors.DalamudViolet,
                    string.Format(UIStrings.Custom_AutoCast_Being_Used, Service.Configuration.HookPresets.SelectedPreset.PresetName));
            else if (!acCfg.EnableAll)
                ImGui.TextColored(ImGuiColors.ParsedBlue, UIStrings.SubAuto_Disabled);
        }
        
        
        ImGui.Spacing();

        ImGui.Separator();
    }

    private void DrawBody(AutoCastsConfig acCfg)
    {
        if (!acCfg.EnableAll)
            return;
        
        ImGui.TextColored(ImGuiColors.HealerGreen, UIStrings.Auto_Cast_Alert_Manual_Hook);
        ImGui.TextColored(ImGuiColors.DalamudOrange, UIStrings.Auto_Cast_Sort_Notice);
        foreach (var action in actionsAvailable.OrderBy(x => x.GetType() == typeof(AutoCastLine)).ThenBy(x => x.GetType() == typeof(AutoMooch)).ThenBy(x => x.GetType() == typeof(AutoCollect)).ThenBy(x => x.Priority))
        {
            try
            {
                ImGui.PushID(action.GetType().ToString());
                action.DrawConfig(actionsAvailable);
                ImGui.PopID();
            }
            catch (Exception e)
            {
                Service.PrintDebug(e.ToString());
            }
        }
    }
    
}
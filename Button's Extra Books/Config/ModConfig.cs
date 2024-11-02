﻿using System;
using System.Net.Mime;
using System.Text.Json.Serialization;
using ButtonsExtraBooks.Helpers;
using GenericModConfigMenu;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;

namespace ButtonsExtraBooks.Config;

public sealed class ModConfig
{
    public bool AlwaysAvailable { get; set; } = false;
    
    public bool EnableLuck { get; set; } = true;
    public int LuckPrice { get; set; } = 25000;
    public float LuckAmount { get; set; } = 0.025f;
    
    public bool EnableExtraGifts { get; set; } = true;
    public  int ExtraGiftsPrice { get; set; } = 50000;
    public int ExtraGiftsBonus { get; set; } = 7;
    
    public bool EnableTreesIgnoreSeason { get; set; } = true;
    public int TreesIgnoreSeasonPrice { get; set; } = 20000;
    
    public bool EnableArtisanMachines { get; set; } = true;
    public int ArtisanMachinesPrice { get; set; } = 100000;
    public bool ArtisanMachinesGrangeMustWin { get; set; } = true;
    public int ArtisanMachinesPercentDecrease { get; set; } = 10;
    
    public bool EnableGiantCrops { get; set; } = true;
    public int GiantCropsPrice { get; set; } = 30000;
    public int GiantCropsPercent { get; set; } = 5;
    
    public bool EnablePopularity { get; set; } = true;
    public int PopularityPrice { get; set; } = 35000;
    
    public bool EnableOptimization { get; set; } = true;
    public int OptimizationPrice { get; set; } = 30000;
    public int OptimizationPercent { get; set; } = 2;

    public bool EnableBusDriving { get; set; } = true;
    public int BusDrivingPrice { get; set; } = 42500;
    
    public bool EnableQiNotebook { get; set; } = true;
    public int QiNotebookPrice { get; set; } = 10;
    public float QiNotebookPercent { get; set; } = 10f;
    
    public bool EnablePetGifts { get; set; } = true;
    public int PetGiftsPrice { get; set; } = 15000;
    
    public bool EnableCheatCodes { get; set; } = true;
    public int CheatCodesPrice { get; set; } = 65536;
    public int CheatCodesRequirement { get; set; } = 30;
    public int CheatCodesLives { get; set; } = 1;
    
    public bool EnableJunimoScrap { get; set; } = true;
    public int JunimoScrapPrice { get; set; } = 25000;
    public int JunimoChatter { get; set; } = 15;
    public bool JunimoRandomItems { get; set; } = false;
    
    public bool EnableCarols { get; set; } = true;
    public int CarolsPrice { get; set; } = 65000;
    public int CarolsBonus { get; set; } = 15;
    
    public bool EnableCoffee { get; set; } = true;
    public int CoffeePrice { get; set; } = 75000;
    public float CoffeeBonus { get; set; } = 3f;
    
    public bool EnableSketchbook { get; set; } = true;
    public int SketchbookPrice { get; set; } = 30000;
    
    public bool DebugBook { get; set; } = false;

    public ModConfig()
    {
        Init();
    }

    private void Init()
    {
        ResetAllEnabled();
        ResetAllPrice();
        AlwaysAvailable = false;
        LuckAmount = 0.025f;
        ExtraGiftsBonus = 7;
        ArtisanMachinesPercentDecrease = 10;
        ArtisanMachinesGrangeMustWin = true;
        GiantCropsPercent = 5;
        OptimizationPercent = 2;
        QiNotebookPercent = 10f;
        CheatCodesRequirement = 30;
        CheatCodesLives = 1;
        JunimoChatter = 15;
        JunimoRandomItems = false;
        CarolsBonus = 15;
        DebugBook = false;
    }

    public void ResetAllEnabled()
    {
        foreach (var property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(bool) && property.Name.StartsWith("Enable"))
            {
                property.SetValue(this, true);
            }
        }
    }

    public void ResetAllPrice()
    {
        LuckPrice = 25000;
        ExtraGiftsPrice = 50000;
        TreesIgnoreSeasonPrice = 20000;
        ArtisanMachinesPrice = 100000;
        GiantCropsPrice = 30000;
        PopularityPrice = 35000;
        OptimizationPrice = 30000;
        BusDrivingPrice = 42500;
        QiNotebookPrice = 10;
        PetGiftsPrice = 15000;
        CheatCodesPrice = 65536;
        JunimoScrapPrice = 25000;
        CarolsPrice = 65000;
        CoffeePrice = 75000;
        SketchbookPrice = 30000;
    }
    
    public bool GetPowerEnabled(string powerName)
    {
        return (bool)GetType().GetProperty($"Enable{powerName}")?.GetValue(this)!;
    }
    
    public int GetBookPrice(string bookName)
    {
        return (int)GetType().GetProperty($"{bookName}Price")?.GetValue(this)!;
    }

    public void SetupConfig(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper, Harmony harmony)
    {
        configMenu.Register(
            mod: ModManifest,
            reset: Init,
            save: () => Helper.WriteConfig(this)
        );
        SetupPages(configMenu, ModManifest, Helper, harmony);
        SetupGeneral(configMenu, ModManifest, Helper, harmony);
        SetupEnableDisable(configMenu, ModManifest);
        SetupPrices(configMenu, ModManifest);
        SetupAdjustments(configMenu, ModManifest, harmony);
        SetupDebug(configMenu, ModManifest, Helper, harmony);
    }
    
    private static void SetupPages(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.General",
            text: Utils.TryGetI18n("Config.SectionTitle.General")
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.EnableDisable",
            text: Utils.TryGetI18n("Config.SectionTitle.EnableDisable")
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Prices",
            text: Utils.TryGetI18n("Config.SectionTitle.Prices")
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Adjustments",
            text: Utils.TryGetI18n("Config.SectionTitle.Adjustments")
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "Config.Pages.Debug",
            text: Utils.TryGetI18n("Config.SectionTitle.Debug")
        );
    }
    
    private void SetupGeneral(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.General",
            pageTitle: Utils.TryGetI18n("Config.SectionTitle.General")
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.General.AlwaysAvailable.Name"),
            tooltip: Utils.TryGetI18n("Config.General.AlwaysAvailable.Description"),
            getValue: () => AlwaysAvailable,
            setValue: value => AlwaysAvailable = value
        );
    }

    private void SetupEnableDisable(IGenericModConfigMenuApi configMenu, IManifest ModManifest)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.EnableDisable",
            pageTitle: Utils.TryGetI18n("Config.SectionTitle.EnableDisable")
        );
        foreach (var property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(bool) && property.Name.StartsWith("Enable"))
            {
                string bookName = property.Name.Substring(6);
                configMenu.AddBoolOption(
                    mod: ModManifest,
                    name: Utils.TryGetI18n($"{bookName}.Book.Name"),
                    tooltip: Utils.TryGetI18n("Config.General.Enabled.Description"),
                    getValue: () => (bool)property.GetValue(this)!,
                    setValue: value => property.SetValue(this, value)
                );
            }
        }
    }
    
    private void SetupPrices(IGenericModConfigMenuApi configMenu, IManifest ModManifest)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Prices",
            pageTitle: Utils.TryGetI18n("Config.SectionTitle.Prices")
        );
        foreach (var property in GetType().GetProperties())
        {
            if (property.PropertyType == typeof(int) && property.Name.EndsWith("Price"))
            {
                string bookName = property.Name.Substring(0, property.Name.Length - 5);
                configMenu.AddNumberOption(
                    mod: ModManifest,
                    name: Utils.TryGetI18n($"{bookName}.Book.Name"),
                    tooltip: Utils.TryGetI18n("Config.General.Price.Description"),
                    getValue: () => (int)property.GetValue(this)!,
                    setValue: value => property.SetValue(this, value)
                );
            }
        }
    }

    private void SetupAdjustments(IGenericModConfigMenuApi configMenu, IManifest ModManifest, Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Adjustments",
            pageTitle: Utils.TryGetI18n("Config.SectionTitle.Adjustments")
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("Luck.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.Luck.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.Luck.Bonus.Description"),
            getValue: () => LuckAmount,
            setValue: value => LuckAmount = value
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("ExtraGifts.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.ExtraGifts.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.ExtraGifts.Bonus.Description"),
            getValue: () => ExtraGiftsBonus,
            setValue: value => ExtraGiftsBonus = value,
            min: 3,
            max: 7,
            interval: 1
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("ArtisanMachines.Book.Name")
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.ArtisanMachines.GrangeMustWin.Name"),
            tooltip: Utils.TryGetI18n("Config.ArtisanMachines.GrangeMustWin.Description"),
            getValue: () => ArtisanMachinesGrangeMustWin,
            setValue: value => ArtisanMachinesGrangeMustWin = value
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.ArtisanMachines.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.ArtisanMachines.Bonus.Description"),
            getValue: () => ArtisanMachinesPercentDecrease,
            setValue: (value) =>
            {
                ArtisanMachinesPercentDecrease = value;
                harmony.UnpatchAll(ModManifest.UniqueID);
                harmony.PatchAll();
            },
            min: 1,
            max: 50,
            interval: 1,
            formatValue: (value) => $"{value}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("GiantCrops.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.GiantCrops.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.GiantCrops.Bonus.Description"),
            getValue: () => GiantCropsPercent,
            setValue: value => GiantCropsPercent = value,
            min: 2,
            max: 100,
            interval: 1,
            formatValue: value => $"{value}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => Utils.SanitizeString(Utils.TryGetI18n("Optimization.Book.Name")())
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.Optimization.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.Optimization.Bonus.Description"),
            getValue: () => OptimizationPercent,
            setValue: value => OptimizationPercent = value,
            min: 1,
            max: 100,
            interval: 1,
            formatValue: value => $"{value}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("QiNotebook.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.QiNotebook.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.QiNotebook.Bonus.Description"),
            getValue: () => QiNotebookPercent,
            setValue: value => QiNotebookPercent = value,
            min: 1,
            max: 100,
            interval: 1,
            formatValue: value => $"{value / 10}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("CheatCodes.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.CheatCodes.Requirement.Name"),
            tooltip: Utils.TryGetI18n("Config.CheatCodes.Requirement.Description"),
            getValue: () => CheatCodesRequirement,
            setValue: value => CheatCodesRequirement = value,
            min: 1,
            max: 100,
            interval: 1
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.CheatCodes.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.CheatCodes.Bonus.Description"),
            getValue: () => CheatCodesLives,
            setValue: value => CheatCodesLives = value,
            min: 1,
            max: 100,
            interval: 1
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("JunimoScrap.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.JunimoScrap.Chatter.Name"),
            tooltip: Utils.TryGetI18n("Config.JunimoScrap.Chatter.Description"),
            getValue: () => JunimoChatter,
            setValue: value => JunimoChatter = value,
            min: 0,
            max: 100,
            interval: 1,
            formatValue: (value) =>
            {
                if (value == 0)
                    return Utils.TryGetI18n("Config.JunimoScrap.Chatter.Never")();
                if (value >= 45 && value <= 55)
                    return Utils.TryGetI18n("Config.JunimoScrap.Chatter.Normal")();
                if (value == 100)
                    return Utils.TryGetI18n("Config.JunimoScrap.Chatter.Always")();
                return $"{value}";
            }
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.JunimoScrap.RandomItems.Name"),
            tooltip: Utils.TryGetI18n("Config.JunimoScrap.RandomItems.Description"),
            getValue: () => JunimoRandomItems,
            setValue: value => JunimoRandomItems = value
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("Carols.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.Carols.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.Carols.Bonus.Description"),
            getValue: () => CarolsBonus,
            setValue: value =>
            {
                CarolsBonus = value;
                ModEntry.ModHelper.GameContent.InvalidateCache("Data/Shops");
            },
            min: 1,
            max: 100,
            interval: 1,
            formatValue: (value) => $"{value}%"
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: Utils.TryGetI18n("Coffee.Book.Name")
        );
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.Coffee.Bonus.Name"),
            tooltip: Utils.TryGetI18n("Config.Coffee.Bonus.Description"),
            getValue: () => CoffeeBonus,
            setValue: value => CoffeeBonus = value,
            min: 1f,
            interval: 0.1f,
            formatValue: (value) => $"x{value}"
        );
    }
    
    private void SetupDebug(IGenericModConfigMenuApi configMenu, IManifest ModManifest, IModHelper Helper,
        Harmony harmony)
    {
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "Config.Pages.Debug",
            pageTitle: Utils.TryGetI18n("Config.SectionTitle.Debug")
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: Utils.TryGetI18n("Config.Debug.EnableDebugBook.Name"),
            tooltip: Utils.TryGetI18n("Config.Debug.EnableDebugBook.Description"),
            getValue: () => DebugBook,
            setValue: value => DebugBook = value
        );
    }
}
using System.IO;
using UnityEngine;
using LibAssloud = XTC.FMP.MOD.Assloud.LIB.Unity;
using LibWingMenu = XTC.FMP.MOD.WingMenu.LIB.Unity;

/// <summary>
/// 根程序类
/// </summary>
/// <remarks>
/// 不参与模块编译，仅用于在编辑器中开发调试
/// </remarks>
public class Root : RootBase
{
    protected LibAssloud.MyEntry entryAssloud_ { get; set; } = new LibAssloud.MyEntry();

    private void Awake()
    {
        setupSettings();

        string xml = File.ReadAllText(UnityEngine.Application.dataPath + string.Format("/Exports/{0}.xml", LibWingMenu.MyEntryBase.ModuleName));
        config_.MergeKV(LibWingMenu.MyEntryBase.ModuleName, xml);
        string xmlAssloud = File.ReadAllText(UnityEngine.Application.persistentDataPath + string.Format("/data/configs/{0}.xml", LibAssloud.MyEntryBase.ModuleName));
        config_.MergeKV(LibAssloud.MyEntryBase.ModuleName, xmlAssloud);

        initFramework();

        entry_ = new LibWingMenu.DebugEntry();
        var options = entry_.NewOptions();
        entry_.Inject(framework_, options);
        entry_.UniInject(this, options, logger_, config_, settings_);
        framework_.setUserData("XTC.FMP.MOD.LibWingMenu.LIB.MVCS.Entry", entry_);
        entry_.RegisterDummy();

        entryAssloud_ = new LibAssloud.MyEntry();
        var optionsAssloud = entryAssloud_.NewOptions();
        entryAssloud_.Inject(framework_, optionsAssloud);
        entryAssloud_.UniInject(this, optionsAssloud, logger_, config_, settings_);
        framework_.setUserData("XTC.FMP.MOD.Assloud.LIB.MVCS.Entry", entryAssloud_);
        entryAssloud_.RegisterDummy();

        setupFramework();

    }

    private void Start()
    {
        entryAssloud_.Preload((_percentage) =>
        {
        }, (_module) =>
         {
             entry_.__DebugPreload(exportRoot);
         });
    }

    private void OnDestroy()
    {
        doDestroy();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 60, 30), "Create"))
        {
            entry_.__DebugCreate("test", "default");
        }

        if (GUI.Button(new Rect(0, 30, 60, 30), "Open"))
        {
            entry_.__DebugOpen("test", "file", "", 0.5f);
        }

        if (GUI.Button(new Rect(0, 60, 60, 30), "Close"))
        {
            entry_.__DebugClose("test", 0.5f);
        }

        if (GUI.Button(new Rect(0, 90, 60, 30), "Delete"))
        {
            entry_.__DebugDelete("test");
        }
    }
}


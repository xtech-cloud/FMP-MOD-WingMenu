
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.20.0.  DO NOT EDIT!
//*************************************************************************************

using Microsoft.Extensions.Options;
using XTC.FMP.MOD.WingMenu.App.Service;

public class DatabaseOptions : IOptions<DatabaseSettings>
{
    public DatabaseSettings Value
    {
        get
        {
            return new DatabaseSettings
            {
                ConnectionString = "mongodb://root:mongo%40XTC@localhost:27017",
                DatabaseName = "XTC_FMP_WingMenu_TEST",
            };
        }
    }
}
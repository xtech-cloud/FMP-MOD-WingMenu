
using System;
using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.WingMenu.LIB.Bridge;
using XTC.FMP.MOD.WingMenu.LIB.MVCS;

using AssloudMVCS = XTC.FMP.MOD.Assloud.LIB.MVCS;
using AssloudBridge = XTC.FMP.MOD.Assloud.LIB.Bridge;
using AssloudProto = XTC.FMP.MOD.Assloud.LIB.Proto;
using AssloudUnity = XTC.FMP.MOD.Assloud.LIB.Unity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XTC.FMP.MOD.WingMenu.LIB.Unity
{
    /// <summary>
    /// 虚拟视图，用于处理消息订阅
    /// </summary>
    public class DummyView : DummyViewBase
    {
        public DummyView(string _uid) : base(_uid)
        {
        }

        protected override void setup()
        {
            base.setup();
            addSubscriber(AssloudMVCS.Subjects.OnMountDisk, handleAssloudOnMountDisk);
        }

        private void handleAssloudOnMountDisk(LibMVCS.Model.Status _status, object _data)
        {
            string gid = "";
            try
            {
                var data = _data as Dictionary<string, object>;
                gid = (string)data["gid"];
            }
            catch (System.Exception ex)
            {
                getLogger().Exception(ex);
            }

            MyInstance instance;
            if (!runtime.instances.TryGetValue(gid, out instance))
                return;

            // 替换Assloud的和模块同名(XTC_WingMenu)的实例中的ContentUiBridge，需要Assloud配置文件中有对应名的实例
            // gid等于实例的uid
            var facadeUID = AssloudMVCS.ContentFacade.NAME + "." + gid;
            var facade = findFacade(facadeUID);
            if (null == facade)
            {
                getLogger().Error("facade:{0} not found", facadeUID);
                return;
            }
            getLogger().Info("replace ContentUiBridge of facade:{0}", facadeUID);
            var uiBridge = new AssloudContentUiBridge(getLogger(), instance, model_);
            facade.setUiBridge(uiBridge);

            // 查询
            var req = new AssloudProto.ContentMatchRequest();
            req.Patterns.Add("tech.meex+");
            var dto = new AssloudMVCS.ContentMatchRequestDTO(req);
            var viewBridge = facade?.getViewBridge() as AssloudBridge.IContentViewBridge;
            SynchronizationContext context = SynchronizationContext.Current;
            Task.Run(async () =>
            {
                try
                {
                    var result = await viewBridge.OnMatchSubmit(dto, context);
                    if (!LibMVCS.Error.IsOK(result))
                    {
                        getLogger().Error(result.getMessage());
                    }

                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                }
            });
        }
    }
}


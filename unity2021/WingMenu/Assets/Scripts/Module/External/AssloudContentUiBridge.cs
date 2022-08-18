using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibMVCS = XTC.FMP.LIB.MVCS;
using XTC.FMP.MOD.Assloud.LIB.MVCS;
using XTC.FMP.MOD.Assloud.LIB.Bridge;
using XTC.FMP.MOD.Assloud.LIB.Proto;
using XTC.FMP.MOD.Assloud.LIB.Unity;
using UnityEngine;
using System.Threading;
using XTC.FMP.LIB.MVCS;

namespace XTC.FMP.MOD.WingMenu.LIB.Unity
{
    public class AssloudContentUiBridge : IContentUiBridge
    {
        public AssloudContentUiBridge(LibMVCS.Logger _logger, MyInstance _instance, DummyModel _model)
        {
            logger_ = _logger;
            instance_ = _instance;
            model_ = _model;
        }

        protected LibMVCS.Logger logger_ { get; set; }
        protected MyInstance instance_ { get; set; }
        protected DummyModel model_ { get; set; }

        public void Alert(string _code, string _message, SynchronizationContext _context)
        {
            logger_.Error(_message);
        }

        public void RefreshMatch(IDTO _dto, SynchronizationContext _context)
        {
            _context.Post(_ =>
            {
                var signal = new Signal(model_);
                //signal.Connect(instance_.SlotRefreshHotspots);
                signal.Emit(_dto);
            }, null);
        }
    }
}


//*************************************************************************************
//   !!! Generated by the fmp-cli 1.20.0.  DO NOT EDIT!
//*************************************************************************************

public abstract class IntegrationTestBase : IClassFixture<TestFixture>
{
    /// <summary>
    /// 测试上下文
    /// </summary>
    protected TestFixture fixture_ { get; set; }

    public IntegrationTestBase(TestFixture _testFixture)
    {
        fixture_ = _testFixture;
    }

    [Fact]
    public abstract Task Test();
}

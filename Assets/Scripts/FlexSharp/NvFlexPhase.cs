namespace FlexSharp
{
    public enum NvFlexPhase
    {
        eNvFlexPhaseGroupMask = 0x000fffff,
        eNvFlexPhaseFlagsMask = 0x00f00000,
        eNvFlexPhaseShapeChannelMask = 0x7f000000,
        eNvFlexPhaseSelfCollide = 1 << 20,
        eNvFlexPhaseSelfCollideFilter = 1 << 21,
        eNvFlexPhaseFluid = 1 << 22,
        eNvFlexPhaseUnused = 1 << 23,
        eNvFlexPhaseShapeChannel0 = 1 << 24,
        eNvFlexPhaseShapeChannel1 = 1 << 25,
        eNvFlexPhaseShapeChannel2 = 1 << 26,
        eNvFlexPhaseShapeChannel3 = 1 << 27,
        eNvFlexPhaseShapeChannel4 = 1 << 28,
        eNvFlexPhaseShapeChannel5 = 1 << 29,
        eNvFlexPhaseShapeChannel6 = 1 << 30,
    }
}

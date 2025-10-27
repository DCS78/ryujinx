using NUnit.Framework;
using Ryujinx.Audio.Renderer.Server;

namespace Ryujinx.Tests.Audio.Renderer.Server
{
    public class BehaviourInfoTests
    {
        [Test]
        public void TestCheckFeature()
        {
            int latestRevision = BehaviourInfo.BaseRevisionMagic + BehaviourInfo.LastRevision;
            int previousRevision = BehaviourInfo.BaseRevisionMagic + (BehaviourInfo.LastRevision - 1);
            int invalidRevision = BehaviourInfo.BaseRevisionMagic + (BehaviourInfo.LastRevision + 1);

            Assert.That(BehaviourInfo.CheckFeatureSupported(latestRevision, latestRevision), Is.True);
            Assert.That(BehaviourInfo.CheckFeatureSupported(previousRevision, latestRevision), Is.False);
            Assert.That(BehaviourInfo.CheckFeatureSupported(latestRevision, previousRevision), Is.True);
            // In case we get an invalid revision, this is supposed to auto default to REV1 internally.. idk what the hell Nintendo was thinking here..
            Assert.That(BehaviourInfo.CheckFeatureSupported(invalidRevision, latestRevision), Is.True);
        }

        [Test]
        public void TestsMemoryPoolForceMappingEnabled()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision1);

            Assert.That(behaviourInfo.IsMemoryPoolForceMappingEnabled(), Is.False);

            behaviourInfo.UpdateFlags(0x1);

            Assert.That(behaviourInfo.IsMemoryPoolForceMappingEnabled(), Is.True);
        }

        [Test]
        public void TestRevision1()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision1);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.False);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.False);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.False);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.False);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.False);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.False);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.70f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(1));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(1));
        }

        [Test]
        public void TestRevision2()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision2);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.False);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.False);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.False);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.False);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.False);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.70f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(1));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(1));
        }

        [Test]
        public void TestRevision3()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision3);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.False);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.False);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.False);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.False);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.70f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(1));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(1));
        }

        [Test]
        public void TestRevision4()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision4);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.False);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.False);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.False);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.75f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(1));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(1));
        }

        [Test]
        public void TestRevision5()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision5);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.False);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.False);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(2));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision6()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision6);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.False);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(2));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision7()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision7);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(2));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision8()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision8);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.False);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(3));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision9()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision9);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.False);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(3));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision10()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision10);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.True);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.False);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(4));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision11()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision11);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.True);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.False);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(5));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision12()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision12);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.True);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.True);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.False);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(5));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }

        [Test]
        public void TestRevision13()
        {
            BehaviourInfo behaviourInfo = new();

            behaviourInfo.SetUserRevision(BehaviourInfo.BaseRevisionMagic + BehaviourInfo.Revision13);

            Assert.That(behaviourInfo.IsAdpcmLoopContextBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsSplitterSupported(), Is.True);
            Assert.That(behaviourInfo.IsLongSizePreDelaySupported(), Is.True);
            Assert.That(behaviourInfo.IsAudioUsbDeviceOutputSupported(), Is.True);
            Assert.That(behaviourInfo.IsFlushVoiceWaveBuffersSupported(), Is.True);
            Assert.That(behaviourInfo.IsSplitterBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsElapsedFrameCountSupported(), Is.True);
            Assert.That(behaviourInfo.IsDecodingBehaviourFlagSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterEffectStateClearBugFixed(), Is.True);
            Assert.That(behaviourInfo.IsMixInParameterDirtyOnlyUpdateSupported(), Is.True);
            Assert.That(behaviourInfo.IsWaveBufferVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.IsEffectInfoVersion2Supported(), Is.True);
            Assert.That(behaviourInfo.UseMultiTapBiquadFilterProcessing(), Is.True);
            Assert.That(behaviourInfo.IsNewEffectChannelMappingSupported(), Is.True);
            Assert.That(behaviourInfo.IsBiquadFilterParameterForSplitterEnabled(), Is.True);
            Assert.That(behaviourInfo.IsSplitterPrevVolumeResetSupported(), Is.True);

            Assert.That(behaviourInfo.GetAudioRendererProcessingTimeLimit(), Is.EqualTo(0.80f));
            Assert.That(behaviourInfo.GetCommandProcessingTimeEstimatorVersion(), Is.EqualTo(5));
            Assert.That(behaviourInfo.GetPerformanceMetricsDataFormat(), Is.EqualTo(2));
        }
    }
}

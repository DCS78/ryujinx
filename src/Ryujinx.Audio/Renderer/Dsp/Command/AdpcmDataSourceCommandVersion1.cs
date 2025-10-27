using Ryujinx.Audio.Common;
using Ryujinx.Audio.Renderer.Common;
using Ryujinx.Audio.Renderer.Parameter;
using Ryujinx.Audio.Renderer.Server.Voice;
using System;
using WaveBuffer = Ryujinx.Audio.Renderer.Common.WaveBuffer;

namespace Ryujinx.Audio.Renderer.Dsp.Command
{
    public class AdpcmDataSourceCommandVersion1 : ICommand
    {
        public bool Enabled { get; set; }

        public int NodeId { get; private set; }

        public CommandType CommandType => CommandType.AdpcmDataSourceVersion1;

        public uint EstimatedProcessingTime { get; set; }

        public ushort OutputBufferIndex { get; private set; }
        public uint SampleRate { get; private set; }

        public float Pitch { get; private set; }

        public WaveBuffer[] WaveBuffers { get; }

        public Memory<VoiceState> State { get; private set; }

        public ulong AdpcmParameter { get; private set; }
        public ulong AdpcmParameterSize { get; private set; }

        public DecodingBehaviour DecodingBehaviour { get; private set; }

        public AdpcmDataSourceCommandVersion1()
        {
            WaveBuffers = new WaveBuffer[Constants.VoiceWaveBufferCount];
        }

        public AdpcmDataSourceCommandVersion1 Initialize(ref VoiceInfo serverInfo, Memory<VoiceState> state, ushort outputBufferIndex, int nodeId)
        {
            Enabled = true;
            NodeId = nodeId;

            OutputBufferIndex = outputBufferIndex;
            SampleRate = serverInfo.SampleRate;
            Pitch = serverInfo.Pitch;

            Span<Server.Voice.WaveBuffer> waveBufferSpan = serverInfo.WaveBuffers.AsSpan();

            for (int i = 0; i < WaveBuffers.Length; i++)
            {
                ref Server.Voice.WaveBuffer voiceWaveBuffer = ref waveBufferSpan[i];

                WaveBuffers[i] = voiceWaveBuffer.ToCommon(1);
            }

            AdpcmParameter = serverInfo.DataSourceStateAddressInfo.GetReference(true);
            AdpcmParameterSize = serverInfo.DataSourceStateAddressInfo.Size;
            State = state;
            DecodingBehaviour = serverInfo.DecodingBehaviour;

            return this;
        }

        public void Process(CommandList context)
        {
            Span<float> outputBuffer = context.GetBuffer(OutputBufferIndex);

            DataSourceHelper.WaveBufferInformation info = new()
            {
                SourceSampleRate = SampleRate,
                SampleFormat = SampleFormat.Adpcm,
                Pitch = Pitch,
                DecodingBehaviour = DecodingBehaviour,
                ExtraParameter = AdpcmParameter,
                ExtraParameterSize = AdpcmParameterSize,
                ChannelIndex = 0,
                ChannelCount = 1,
            };

            DataSourceHelper.ProcessWaveBuffers(context.MemoryManager, outputBuffer, ref info, WaveBuffers, ref State.Span[0], context.SampleRate, (int)context.SampleCount);
        }
    }
}

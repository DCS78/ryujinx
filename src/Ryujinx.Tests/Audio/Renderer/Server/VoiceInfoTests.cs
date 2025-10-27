using NUnit.Framework;
using Ryujinx.Audio.Renderer.Server.Voice;
using System.Runtime.CompilerServices;

namespace Ryujinx.Tests.Audio.Renderer.Server
{
    class VoiceInfoTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.That(Unsafe.SizeOf<VoiceInfo>(), Is.LessThanOrEqualTo(0x238));
        }
    }
}

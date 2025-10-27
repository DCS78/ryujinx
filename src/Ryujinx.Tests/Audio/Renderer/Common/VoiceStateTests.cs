using NUnit.Framework;
using Ryujinx.Audio.Renderer.Common;
using System.Runtime.CompilerServices;

namespace Ryujinx.Tests.Audio.Renderer.Common
{
    class VoiceStateTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.That(Unsafe.SizeOf<VoiceState>(), Is.LessThanOrEqualTo(0x100));
        }
    }
}

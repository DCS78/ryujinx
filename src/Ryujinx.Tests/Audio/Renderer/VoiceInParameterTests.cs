using NUnit.Framework;
using Ryujinx.Audio.Renderer.Parameter;
using System.Runtime.CompilerServices;

namespace Ryujinx.Tests.Audio.Renderer
{
    class VoiceInParameterTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.That(0x170, Is.EqualTo(Unsafe.SizeOf<VoiceInParameter1>()));
            Assert.That(0x188, Is.EqualTo(Unsafe.SizeOf<VoiceInParameter2>()));
        }
    }
}

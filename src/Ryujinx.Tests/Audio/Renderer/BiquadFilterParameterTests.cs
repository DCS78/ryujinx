using NUnit.Framework;
using Ryujinx.Audio.Renderer.Parameter;
using System.Runtime.CompilerServices;

namespace Ryujinx.Tests.Audio.Renderer
{
    class BiquadFilterParameterTests
    {
        [Test]
        public void EnsureTypeSize()
        {
            Assert.That(0xC, Is.EqualTo(Unsafe.SizeOf<BiquadFilterParameter1>()));
            Assert.That(0x18, Is.EqualTo(Unsafe.SizeOf<BiquadFilterParameter2>()));
        }
    }
}

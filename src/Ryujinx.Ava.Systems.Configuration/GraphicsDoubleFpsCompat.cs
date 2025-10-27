using System;

namespace Ryujinx.Ava.Systems.Configuration
{
 internal static class GraphicsDoubleFpsCompat
 {
 // Try to read DoubleFps from a graphics config object supporting either:
 // - public bool DoubleFps
 // - public SomeWrapper DoubleFps { public bool Value { get; set; } }
 public static bool TryRead(object gfx, out bool value)
 {
 value = false;

 if (gfx == null)
 {
 return false;
 }

 var t = gfx.GetType();
 var p = t.GetProperty("DoubleFps");

 if (p == null)
 {
 return false;
 }

 object raw = p.GetValue(gfx);

 if (raw is bool b)
 {
 value = b;
 return true;
 }

 if (raw != null)
 {
 var valProp = p.PropertyType.GetProperty("Value");

 if (valProp != null)
 {
 object inner = valProp.GetValue(raw);

 if (inner is bool ib)
 {
 value = ib;
 return true;
 }
 }
 }

 return false;
 }

 // Try to write DoubleFps to the graphics object (direct bool or wrapper.Value)
 public static bool TryWrite(object gfx, bool value)
 {
 if (gfx == null)
 {
 return false;
 }

 var t = gfx.GetType();
 var p = t.GetProperty("DoubleFps");

 if (p == null)
 {
 return false;
 }

 if (p.PropertyType == typeof(bool))
 {
 p.SetValue(gfx, value);
 return true;
 }

 object raw = p.GetValue(gfx);

 if (raw != null)
 {
 var valProp = p.PropertyType.GetProperty("Value");

 if (valProp != null && valProp.PropertyType == typeof(bool))
 {
 valProp.SetValue(raw, value);
 return true;
 }
 }

 return false;
 }
 }
}

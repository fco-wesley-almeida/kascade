namespace Kascade.Proxy.Extensions;

/// <summary>
/// Provides extension methods for byte data.
/// </summary>
public static class ByteExtensions
{
	/// <summary>
	/// Checks whether the given byte represents a numeric character ('0' to '9').
	/// </summary>
	/// <param name="byte">The byte to check.</param>
	/// <returns>True if the byte represents a numeric character; otherwise, false.</returns>
	public static bool IsNumeric(this byte @byte) =>
					@byte == '0' || @byte == '1' || 
					@byte == '2' || @byte == '3' || 
					@byte == '4' || @byte == '5' || 
					@byte == '6' || @byte == '7' || 
					@byte == '8' || @byte == '9';
}
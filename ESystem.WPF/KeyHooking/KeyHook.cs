using ESystem.Asserting;
using ESystem.WPF.KeyHooking.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ESystem.WPF.KeyHooking;

/// <summary>
/// Provides global low-level keyboard hook functionality to register and detect
/// keyboard shortcuts (single key + modifiers) and key chords (sequence of keys with modifiers).
/// </summary>
public class KeyHook : IDisposable
{
  private record KeyShortcutAction(KeyShortcut Shortcut, Action Callback);
  private record KeyChordAction(KeyChord Chord, Action Callback)
  {
    public override string ToString() => $"{Chord} (+ action)";
  }
  private class ActiveChord
  {
    public readonly KeyChordAction chordAction;
    private int index;

    private ActiveChord(KeyChordAction chord)
    {
      chordAction = chord;
      index = 0;
    }
    public override string ToString() => $"CurrentChord: {chordAction}, Index: {index}";
    public static ActiveChord CreateAfterFirstKey(KeyChordAction chord)
    {
      var ret = new ActiveChord(chord);
      ret.index = 1;
      return ret;
    }
    public bool IsNextKeyMatching(ModifierKeys mods, Key key)
    {
      if (index >= chordAction.Chord.Keys.Count)
        throw new UnreachableException("No next key to match; chord already finished.");
      return chordAction.Chord.Keys[index++] == key && chordAction.Chord.Modifiers == mods;
    }
    public bool IsFinished => index >= chordAction.Chord.Keys.Count;
    public override bool Equals(object? obj)
    {
      if (obj is not ActiveChord) return false;
      var other = (ActiveChord)obj;
      return other.chordAction.Chord == this.chordAction.Chord;
    }
    public override int GetHashCode()
    {
      return chordAction.Chord.GetHashCode();
    }
  }


  private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  private static extern bool UnhookWindowsHookEx(IntPtr hhk);

  [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

  [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
  private static extern IntPtr GetModuleHandle(string lpModuleName);

  // WinAPI constants
  private const int WH_KEYBOARD_LL = 13;
  private const int WM_KEYDOWN = 0x0100;
  private const int WM_KEYUP = 0x0101;
  private const int WM_SYSKEYDOWN = 0x0104;
  private const int WM_SYSKEYUP = 0x0105;

  // Hook handle and delegate - instance fields so each KeyHook has its own hook and state
  private readonly LowLevelKeyboardProc lowLevelKeyboardProc;
  private readonly object syncLock = new();
  private readonly List<KeyShortcutAction> keyShortcutCallbacks = [];
  private readonly List<KeyChordAction> keyChordCallbacks = [];
  private readonly HashSet<ActiveChord> currentChords = [];
  private readonly HashSet<Key> pressedModifiers = [];
  private readonly ESystem.Logging.Logger logger = ESystem.Logging.Logger.Create(nameof(KeyHook));
  private IntPtr hookID = IntPtr.Zero;
  private bool disposed;

  private static IntPtr SetHook(LowLevelKeyboardProc proc)
  {
    using Process currentProcess = Process.GetCurrentProcess();
    if (currentProcess.MainModule == null)
      throw new InternalKeyHooKException("Current process's main module unexpectingly null. Cannot set window hook.");

    using ProcessModule currentModule = currentProcess.MainModule;
    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModule.ModuleName), 0);
  }

  private void EnsureHookInstalled()
  {
    if (hookID == IntPtr.Zero)
      hookID = SetHook(lowLevelKeyboardProc);
  }

  /// <summary>
  /// Initializes a new instance of <see cref="KeyHook"/>.
  /// </summary>
  public KeyHook()
  {
    lowLevelKeyboardProc = HookCallback;
  }

  private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
  {
    if (nCode >= 0)
    {
      int vkCode = Marshal.ReadInt32(lParam);
      Key key = KeyInterop.KeyFromVirtualKey(vkCode);

      try
      {
        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
        {
          if (IsModifier(key))
            pressedModifiers.Add(key);
          else
          {
            ModifierKeys mods = GetModifiers(pressedModifiers);
            HandleKeyShortcutLookup(key, mods);
            HandleKeyChordLookup(key, mods);
          }
        }
        else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
        {
          if (IsModifier(key))
            pressedModifiers.Remove(key);
          this.logger.Log(Logging.LogLevel.TRACE, $"Key released: {key}");
        }
      }
      catch (Exception ex)
      {
        this.logger.Log(Logging.LogLevel.TRACE, "HookCallback error: " + ex);
      }
    }
    return CallNextHookEx(hookID, nCode, wParam, lParam);
  }

  private void HandleKeyChordLookup(Key key, ModifierKeys mods)
  {
    var potentialNewChords = keyChordCallbacks
      .Where(c => c.Chord.Modifiers == mods)
      .Where(c => c.Chord.Keys.Count > 0 && c.Chord.Keys[0] == key)
      .Select(ActiveChord.CreateAfterFirstKey)
      .ToHashSet();

    currentChords.RemoveWhere(q => q.IsNextKeyMatching(mods, key) == false);
    currentChords.UnionWith(potentialNewChords);

    var finishedChords = currentChords.Where(q => q.IsFinished).ToList();
    currentChords.ExceptWith(finishedChords);

    foreach (var fc in finishedChords)
    {
      this.logger.Log(Logging.LogLevel.DEBUG, $"Invoking chord callback for {fc.chordAction.Chord}");
      try
      {
        fc.chordAction.Callback?.Invoke();
      }
      catch (Exception ex)
      {
        throw new KeyChordCallbackFailedException(this, fc.chordAction.Chord, ex);
      }
    }
  }

  private void HandleKeyShortcutLookup(Key key, ModifierKeys mods)
  {
    var shortcutMatches = keyShortcutCallbacks.Where(e => e.Shortcut.Key == key && e.Shortcut.Modifiers == mods).ToList();
    this.logger.Log(Logging.LogLevel.TRACE, $"Key pressed: {key} (mods: {mods}), shortcut matches {shortcutMatches.Count} of {keyShortcutCallbacks.Count}");
    foreach (var m in shortcutMatches)
    {
      this.logger.Log(Logging.LogLevel.DEBUG, $"Invoking shortcut callback for {m.Shortcut}");
      try
      {
        m.Callback?.Invoke();
      }
      catch (Exception ex)
      {
        throw new KeyShortcutCallbackFailedException(this, m.Shortcut, ex);
      }
    }
  }

  private void TracePrintList(string msg, IEnumerable<KeyChordAction> keyChordCallbacks)
  {
    this.logger.Log(Logging.LogLevel.TRACE, msg);
    foreach (var item in keyChordCallbacks)
    {
      this.logger.Log(Logging.LogLevel.TRACE, $" - {item}");
    }
  }
  private void TracePrintList(string msg, IEnumerable<ActiveChord> keyChordCallbacks)
  {
    this.logger.Log(Logging.LogLevel.TRACE, msg);
    foreach (var item in keyChordCallbacks)
    {
      this.logger.Log(Logging.LogLevel.TRACE, $" - {item}");
    }
  }

  /// <summary>
  /// Registers a callback to be invoked when the specified shortcut is pressed.
  /// </summary>
  /// <param name="keyShortcut">The shortcut to register.</param>
  /// <param name="action">The callback to invoke when the shortcut is activated.</param>
  /// <param name="overwrite">If true, an existing registration for the same shortcut will be replaced. Otherwise an exception is thrown.</param>
  public void RegisterKeyShortcut(KeyShortcut keyShortcut, Action action, bool overwrite = false)
  {
    ArgumentNullException.ThrowIfNull(action);
    var existing = keyShortcutCallbacks.FirstOrDefault(q => q.Shortcut.Key == keyShortcut.Key && q.Shortcut.Modifiers == keyShortcut.Modifiers);
    if (existing is not null)
    {
      if (overwrite)
        UnregisterKeyShortcutInternal(new[] { existing });
      else
        throw new KeyShortcutAlreadyRegisteredException(keyShortcut);
    }

    keyShortcutCallbacks.Add(new KeyShortcutAction(keyShortcut, action));
    EnsureHookInstalled();
  }

  /// <summary>
  /// Unregisters a previously registered shortcut.
  /// </summary>
  /// <param name="keyShortcut">The shortcut to unregister.</param>
  /// <param name="failIfNotFound">When true, throw if the shortcut is not found. Otherwise silent skip.</param>
  public void UnregisterKeyShortcut(KeyShortcut keyShortcut, bool failIfNotFound = false)
  {
    var toRem = keyShortcutCallbacks.Where(e => e.Shortcut.Key == keyShortcut.Key && e.Shortcut.Modifiers == keyShortcut.Modifiers).ToList();
    if (toRem.Count == 0 && failIfNotFound)
      throw new KeyShortcutNotFoundException(keyShortcut);
    UnregisterKeyShortcutInternal(toRem);
  }

  /// <summary>
  /// Registers a callback to be invoked when the specified key chord is entered.
  /// </summary>
  /// <param name="keyChord">The chord (sequence of keys + modifiers).</param>
  /// <param name="action">The callback to invoke when the chord is completed.</param>
  /// <param name="overwrite">If true, an existing registration for the same chord will be replaced.</param>
  public void RegisterKeyChord(KeyChord keyChord, Action action, bool overwrite = false)
  {
    ArgumentNullException.ThrowIfNull(action);
    // normalize null Keys
    var keys = keyChord.Keys ?? [];

    var existing = keyChordCallbacks.FirstOrDefault(k => k.Chord.Modifiers == keyChord.Modifiers && KeysEqual(k.Chord.Keys, keys));
    if (existing is not null)
    {
      if (overwrite)
        UnregisterKeyChordInternal([existing]);
      else
        throw new KeyChordAlreadyRegisteredException(keyChord);
    }

    // store a copy of keys to avoid external modifications
    var stored = new List<Key>(keys);
    keyChordCallbacks.Add(new KeyChordAction(new KeyChord { Keys = stored, Modifiers = keyChord.Modifiers }, action));

    EnsureHookInstalled();
  }

  /// <summary>
  /// Unregisters a previously registered key chord.
  /// </summary>
  /// <param name="keyChord">The chord to unregister.</param>
  /// <param name="failIfNotFound">When true, throw if the chord is not found. Otherwise silent skip.</param>
  public void UnregisterKeyChord(KeyChord keyChord, bool failIfNotFound = false)
  {
    var toRem = keyChordCallbacks.Where(e => e.Chord.Equals(keyChord)).ToList();
    if (toRem.Count == 0 && failIfNotFound)
      throw new KeyChordNotFoundException(keyChord);
    UnregisterKeyChordInternal(toRem);
  }

  /// <summary>
  /// Detects a single key shortcut pressed by the user and returns it.
  /// The returned shortcut contains the non-modifier key and the modifiers that were held.
  /// </summary>
  /// <returns>A task that completes with the detected <see cref="KeyShortcut"/>.</returns>
  public async Task<KeyShortcut> DetectKeyShortcutAsync()
  {
    var tcs = new TaskCompletionSource<KeyShortcut>();

    HashSet<Key> pressed = [];
    bool captured = false;
    Key capturedKey = Key.None;
    ModifierKeys capturedMods = ModifierKeys.None;

    IntPtr localHook = IntPtr.Zero;

    LowLevelKeyboardProc detectProc = (nCode, wParam, lParam) =>
    {
      if (nCode >= 0)
      {
        int vk = Marshal.ReadInt32(lParam);
        Key key = KeyInterop.KeyFromVirtualKey(vk);

        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
        {
          pressed.Add(key);
        }
        else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
        {
          if (!IsModifier(key) && !captured)
          {
            captured = true;
            capturedKey = key;
            capturedMods = GetModifiers(pressed);
          }

          pressed.Remove(key);

          if (captured && !HasModifiers(pressed))
          {
            var result = new KeyShortcut { Key = capturedKey, Modifiers = capturedMods };
            tcs.TrySetResult(result);

            if (localHook != IntPtr.Zero)
            {
              UnhookWindowsHookEx(localHook);
              localHook = IntPtr.Zero;
            }
          }
        }
      }
      return CallNextHookEx(localHook != IntPtr.Zero ? localHook : hookID, nCode, wParam, lParam);
    };

    localHook = SetHook(detectProc);
    return await tcs.Task.ConfigureAwait(false);
  }

  /// <summary>
  /// Attempts to detect a key chord entered by the user. Returns null if detection was interrupted
  /// (for example by changing modifiers mid-chord).
  /// </summary>
  /// <returns>A task that completes with the detected <see cref="KeyChord"/>, or null if detection failed.</returns>
  public async Task<KeyChord?> TryDetectKeyChordAsync()
  {
    var tcs = new TaskCompletionSource<KeyChord?>();
    HashSet<Key> currentlyPressed = [];
    List<Key> storedKeys = [];
    ModifierKeys initialModifiers = ModifierKeys.None;
    bool chordStarted = false;

    IntPtr localHook = IntPtr.Zero;

    LowLevelKeyboardProc detectProc = (nCode, wParam, lParam) =>
    {
      if (nCode >= 0)
      {
        int vk = Marshal.ReadInt32(lParam);
        Key key = KeyInterop.KeyFromVirtualKey(vk);

        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
        {
          currentlyPressed.Add(key);
          var currentMods = GetModifiers(currentlyPressed);

          if (!chordStarted && !IsModifier(key))
          {
            chordStarted = true;
            initialModifiers = currentMods;
          }

          if (chordStarted && currentMods != initialModifiers)
          {
            // modifiers changed during chord -> cancel
            tcs.TrySetResult(null);
            if (localHook != IntPtr.Zero)
            {
              UnhookWindowsHookEx(localHook);
              localHook = IntPtr.Zero;
            }
          }

          if (!IsModifier(key))
            storedKeys.Add(key);
        }
        else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
        {
          currentlyPressed.Remove(key);

          if (chordStarted && !HasModifiers(currentlyPressed))
          {
            var result = new KeyChord { Keys = [.. storedKeys], Modifiers = initialModifiers };
            this.logger.Log(Logging.LogLevel.TRACE, $"Detected chord: {result}");
            tcs.TrySetResult(result);

            if (localHook != IntPtr.Zero)
            {
              UnhookWindowsHookEx(localHook);
              localHook = IntPtr.Zero;
            }
          }
        }
      }
      return CallNextHookEx(localHook != IntPtr.Zero ? localHook : hookID, nCode, wParam, lParam);
    };

    localHook = SetHook(detectProc);
    return await tcs.Task.ConfigureAwait(false);
  }

  /// <summary>
  /// Detects a key chord entered by the user. Throws <see cref="KeyChordDetectionInterruptedException"/>
  /// if detection was interrupted (for example by changing modifiers mid-chord).
  /// </summary>
  /// <returns>A task that completes with the detected <see cref="KeyChord"/>.</returns>
  public async Task<KeyChord> DetectKeyChordAsync()
  {
    return (await TryDetectKeyChordAsync()) ?? throw new KeyChordDetectionInterruptedException();
  }

  /// <summary>
  /// Unregisters all registered shortcuts and chords.
  /// </summary>
  public void UnregisterAll()
  {
    lock (syncLock)
    {
      UnregisterKeyShortcutInternal(keyShortcutCallbacks.ToList());
      UnregisterKeyChordInternal(keyChordCallbacks.ToList());
      keyShortcutCallbacks.Clear();
      keyChordCallbacks.Clear();
    }
  }

  /// <summary>
  /// Dispose pattern: unregister all callbacks and unhook native hook.
  /// </summary>
  public void Dispose()
  {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  private void UnregisterKeyShortcutInternal(IEnumerable<KeyShortcutAction> shortcuts)
  {
    lock (syncLock)
    {
      foreach (var shortcut in shortcuts)
        keyShortcutCallbacks.Remove(shortcut);

      // if no callbacks left, remove hook
      if (keyShortcutCallbacks.Count == 0 && keyChordCallbacks.Count == 0)
      {
        if (hookID != IntPtr.Zero)
        {
          UnhookWindowsHookEx(hookID);
          hookID = IntPtr.Zero;
        }
      }
    }
  }

  private void UnregisterKeyChordInternal(IEnumerable<KeyChordAction> chords)
  {
    lock (syncLock)
    {
      foreach (var chord in chords)
        keyChordCallbacks.Remove(chord);

      if (keyShortcutCallbacks.Count == 0 && keyChordCallbacks.Count == 0)
      {
        if (hookID != IntPtr.Zero)
        {
          UnhookWindowsHookEx(hookID);
          hookID = IntPtr.Zero;
        }
      }
    }
  }

  private static ModifierKeys GetModifiers(HashSet<Key> keys)
  {
    ModifierKeys mods = ModifierKeys.None;
    if (keys.Contains(Key.LeftCtrl) || keys.Contains(Key.RightCtrl)) mods |= ModifierKeys.Control;
    if (keys.Contains(Key.LeftAlt) || keys.Contains(Key.RightAlt)) mods |= ModifierKeys.Alt;
    if (keys.Contains(Key.LeftShift) || keys.Contains(Key.RightShift)) mods |= ModifierKeys.Shift;
    if (keys.Contains(Key.LWin) || keys.Contains(Key.RWin) || keys.Contains(Key.Apps)) mods |= ModifierKeys.Windows;
    return mods;
  }

  private static bool IsModifier(Key key) => key == Key.LeftCtrl || key == Key.RightCtrl ||
                                       key == Key.LeftAlt || key == Key.RightAlt ||
                                       key == Key.LeftShift || key == Key.RightShift ||
                                       key == Key.LWin || key == Key.RWin || key == Key.Apps;

  private static bool HasModifiers(HashSet<Key> keys) => keys.Any(IsModifier);

  private static bool KeysEqual(List<Key> a, List<Key> b)
  {
    if (a == null && b == null) return true;
    if (a == null || b == null) return false;
    return new HashSet<Key>(a).SetEquals(b);
  }

  /// <summary>
  /// Releases resources used by this instance.
  /// When <paramref name="disposing"/> is true, managed resources (registered callbacks) are cleared.
  /// The native keyboard hook is always uninstalled.
  /// </summary>
  /// <param name="disposing">True when called from <see cref="Dispose()"/>; false when called from a finalizer.</param>
  protected virtual void Dispose(bool disposing)
  {
    if (disposed) return;
    lock (syncLock)
    {
      // unregister managed callbacks when disposing
      if (disposing)
      {
        try
        {
          keyShortcutCallbacks.Clear();
          keyChordCallbacks.Clear();
        }
        catch { }
      }

      // always ensure native hook is released
      try
      {
        if (hookID != IntPtr.Zero)
        {
          UnhookWindowsHookEx(hookID);
          hookID = IntPtr.Zero;
        }
      }
      catch { }

      disposed = true;
    }
  }
}

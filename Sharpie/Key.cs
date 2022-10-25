namespace Sharpie;

public enum Key
{
    ///.<summary>A wchar_t contains a key code.</summary>
    KEY_CODE_YES = 0400,

    ///.<summary>Minimum curses key.</summary>
    KEY_MIN = 0401,

    ///.<summary>Break key (unreliable).</summary>
    KEY_BREAK = 0401,

    ///.<summary>Soft (partial) reset (unreliable).</summary>
    KEY_SRESET = 0530,

    ///.<summary>Reset or hard reset (unreliable).</summary>
    KEY_RESET = 0531,

    ///.<summary>Down-arrow key.</summary>
    KEY_DOWN = 0402,

    ///.<summary>Up-arrow key.</summary>
    KEY_UP = 0403,

    ///.<summary>Left-arrow key.</summary>
    KEY_LEFT = 0404,

    ///.<summary>Right-arrow key.</summary>
    KEY_RIGHT = 0405,

    ///.<summary>Home key.</summary>
    KEY_HOME = 0406,

    ///.<summary>backspace key.</summary>
    KEY_BACKSPACE = 0407,

    ///.<summary>Function keys. Space for 64. </summary>
    KEY_F0 = 0410,

    ///.<summary>Function key 1.</summary>
    KEY_F1 = KEY_F0 + 1,

    ///.<summary>Function key 2.</summary>
    KEY_F2 = KEY_F0 + 2,

    ///.<summary>Function key 3.</summary>
    KEY_F3 = KEY_F0 + 3,

    ///.<summary>Function key 4.</summary>
    KEY_F4 = KEY_F0 + 4,

    ///.<summary>Function key 5.</summary>
    KEY_F5 = KEY_F0 + 5,

    ///.<summary>Function key 6.</summary>
    KEY_F6 = KEY_F0 + 6,

    ///.<summary>Function key 7.</summary>
    KEY_F7 = KEY_F0 + 7,

    ///.<summary>Function key 8.</summary>
    KEY_F8 = KEY_F0 + 8,

    ///.<summary>Function key 9.</summary>
    KEY_F9 = KEY_F0 + 9,

    ///.<summary>Function key 10.</summary>
    KEY_F10 = KEY_F0 + 10,

    ///.<summary>Function key 11.</summary>
    KEY_F11 = KEY_F0 + 11,

    ///.<summary>Function key 12.</summary>
    KEY_F12 = KEY_F0 + 12,

    ///.<summary>Delete-line key.</summary>
    KEY_DL = 0510,

    ///.<summary>Insert-line key.</summary>
    KEY_IL = 0511,

    ///.<summary>Delete-character key.</summary>
    KEY_DC = 0512,

    ///.<summary>Insert-character key.</summary>
    KEY_IC = 0513,

    ///.<summary>Sent by rmir or smir in insert mode.</summary>
    KEY_EIC = 0514,

    ///.<summary>Clear-screen or erase key.</summary>
    KEY_CLEAR = 0515,

    ///.<summary>Clear-to-end-of-screen key.</summary>
    KEY_EOS = 0516,

    ///.<summary>Clear-to-end-of-line key.</summary>
    KEY_EOL = 0517,

    ///.<summary>Scroll-forward key.</summary>
    KEY_SF = 0520,

    ///.<summary>Scroll-backward key.</summary>
    KEY_SR = 0521,

    ///.<summary>Next-page key.</summary>
    KEY_NPAGE = 0522,

    ///.<summary>Previous-page key.</summary>
    KEY_PPAGE = 0523,

    ///.<summary>Set-tab key.</summary>
    KEY_STAB = 0524,

    ///.<summary>Clear-tab key.</summary>
    KEY_CTAB = 0525,

    ///.<summary>Clear-all-tabs key.</summary>
    KEY_CATAB = 0526,

    ///.<summary>Enter/send key.</summary>
    KEY_ENTER = 0527,

    ///.<summary>Print key.</summary>
    KEY_PRINT = 0532,

    ///.<summary>Lower-left key (home down).</summary>
    KEY_LL = 0533,

    ///.<summary>Upper left of keypad.</summary>
    KEY_A1 = 0534,

    ///.<summary>Upper right of keypad.</summary>
    KEY_A3 = 0535,

    ///.<summary>Center of keypad.</summary>
    KEY_B2 = 0536,

    ///.<summary>Lower left of keypad.</summary>
    KEY_C1 = 0537,

    ///.<summary>Lower right of keypad.</summary>
    KEY_C3 = 0540,

    ///.<summary>Back-tab key.</summary>
    KEY_BTAB = 0541,

    ///.<summary>Begin key.</summary>
    KEY_BEG = 0542,

    ///.<summary>Cancel key.</summary>
    KEY_CANCEL = 0543,

    ///.<summary>Close key.</summary>
    KEY_CLOSE = 0544,

    ///.<summary>Command key.</summary>
    KEY_COMMAND = 0545,

    ///.<summary>Copy key.</summary>
    KEY_COPY = 0546,

    ///.<summary>Create key.</summary>
    KEY_CREATE = 0547,

    ///.<summary>End key.</summary>
    KEY_END = 0550,

    ///.<summary>Exit key.</summary>
    KEY_EXIT = 0551,

    ///.<summary>Find key.</summary>
    KEY_FIND = 0552,

    ///.<summary>Help key.</summary>
    KEY_HELP = 0553,

    ///.<summary>Mark key.</summary>
    KEY_MARK = 0554,

    ///.<summary>Message key.</summary>
    KEY_MESSAGE = 0555,

    ///.<summary>Move key.</summary>
    KEY_MOVE = 0556,

    ///.<summary>Next key.</summary>
    KEY_NEXT = 0557,

    ///.<summary>Open key.</summary>
    KEY_OPEN = 0560,

    ///.<summary>Options key.</summary>
    KEY_OPTIONS = 0561,

    ///.<summary>Previous key.</summary>
    KEY_PREVIOUS = 0562,

    ///.<summary>Redo key.</summary>
    KEY_REDO = 0563,

    ///.<summary>Reference key.</summary>
    KEY_REFERENCE = 0564,

    ///.<summary>Refresh key.</summary>
    KEY_REFRESH = 0565,

    ///.<summary>Replace key.</summary>
    KEY_REPLACE = 0566,

    ///.<summary>Restart key.</summary>
    KEY_RESTART = 0567,

    ///.<summary>Resume key.</summary>
    KEY_RESUME = 0570,

    ///.<summary>Save key.</summary>
    KEY_SAVE = 0571,

    ///.<summary>Shifted begin key.</summary>
    KEY_SBEG = 0572,

    ///.<summary>Shifted cancel key.</summary>
    KEY_SCANCEL = 0573,

    ///.<summary>Shifted command key.</summary>
    KEY_SCOMMAND = 0574,

    ///.<summary>Shifted copy key.</summary>
    KEY_SCOPY = 0575,

    ///.<summary>Shifted create key.</summary>
    KEY_SCREATE = 0576,

    ///.<summary>Shifted delete-character key.</summary>
    KEY_SDC = 0577,

    ///.<summary>Shifted delete-line key.</summary>
    KEY_SDL = 0600,

    ///.<summary>Select key.</summary>
    KEY_SELECT = 0601,

    ///.<summary>Shifted end key.</summary>
    KEY_SEND = 0602,

    ///.<summary>Shifted clear-to-end-of-line key.</summary>
    KEY_SEOL = 0603,

    ///.<summary>Shifted exit key.</summary>
    KEY_SEXIT = 0604,

    ///.<summary>Shifted find key.</summary>
    KEY_SFIND = 0605,

    ///.<summary>Shifted help key.</summary>
    KEY_SHELP = 0606,

    ///.<summary>Shifted home key.</summary>
    KEY_SHOME = 0607,

    ///.<summary>Shifted insert-character key.</summary>
    KEY_SIC = 0610,

    ///.<summary>Shifted left-arrow key.</summary>
    KEY_SLEFT = 0611,

    ///.<summary>Shifted message key.</summary>
    KEY_SMESSAGE = 0612,

    ///.<summary>Shifted move key.</summary>
    KEY_SMOVE = 0613,

    ///.<summary>Shifted next key.</summary>
    KEY_SNEXT = 0614,

    ///.<summary>Shifted options key.</summary>
    KEY_SOPTIONS = 0615,

    ///.<summary>Shifted previous key.</summary>
    KEY_SPREVIOUS = 0616,

    ///.<summary>Shifted print key.</summary>
    KEY_SPRINT = 0617,

    ///.<summary>Shifted redo key.</summary>
    KEY_SREDO = 0620,

    ///.<summary>Shifted replace key.</summary>
    KEY_SREPLACE = 0621,

    ///.<summary>Shifted right-arrow key.</summary>
    KEY_SRIGHT = 0622,

    ///.<summary>Shifted resume key.</summary>
    KEY_SRSUME = 0623,

    ///.<summary>Shifted save key.</summary>
    KEY_SSAVE = 0624,

    ///.<summary>Shifted suspend key.</summary>
    KEY_SSUSPEND = 0625,

    ///.<summary>Shifted undo key.</summary>
    KEY_SUNDO = 0626,

    ///.<summary>Suspend key.</summary>
    KEY_SUSPEND = 0627,

    ///.<summary>Undo key.</summary>
    KEY_UNDO = 0630,

    ///.<summary>Mouse event has occurred.</summary>
    KEY_MOUSE = 0631,

    ///.<summary>Terminal resize event.</summary>
    KEY_RESIZE = 0632,

    ///.<summary>We were interrupted by an event.</summary>
    KEY_EVENT = 0633,
}

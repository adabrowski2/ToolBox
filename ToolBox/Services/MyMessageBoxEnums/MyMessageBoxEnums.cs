namespace Praca_Inżynierska_v1.Services.MyMessageBoxEnums
{
    // Rodzaj komunikatu (okna)
    public enum MyMessageBoxType
    {
        Info,
        Error,
        Question,
        Success
    }

    // Rodzaj przycisków
    public enum MyMessageBoxButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    // Wynik (co wcisnął user)
    public enum MyMessageBoxResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }
}

